using Pi.BackofficeService.Application.Factories;
using Pi.BackofficeService.Application.Models.Commons;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Application.Services.WalletService;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;
using Status = Pi.BackofficeService.Application.Services.WalletService.Status;

namespace Pi.BackofficeService.Application.Queries;

public class BackofficeQueries : IBackofficeQueries
{
    private readonly IDepositWithdrawService _depositWithdrawService;
    private readonly ITransferCashService _transferCashService;
    private readonly IResponseCodeActionRepository _responseCodeActionRepository;
    private readonly IResponseCodeRepository _responseCodeRepository;
    private readonly ITransactionRepository _transactionRepository;

    public BackofficeQueries(IDepositWithdrawService depositWithdrawService, ITransferCashService transferCashService,
        ITransactionRepository transactionRepository, IResponseCodeRepository responseCodeRepository,
        IResponseCodeActionRepository responseCodeActionRepository)
    {
        _depositWithdrawService = depositWithdrawService;
        _transferCashService = transferCashService;
        _transactionRepository = transactionRepository;
        _responseCodeRepository = responseCodeRepository;
        _responseCodeActionRepository = responseCodeActionRepository;
    }

    public async Task<TransactionDetailResult<TransactionV2>?> GetTransactionV2ByTransactionNo(string transactionNo, CancellationToken cancellationToken = default)
    {
        var transaction = await _depositWithdrawService.GetTransactionV2ByTransactionNo(transactionNo, cancellationToken);
        if (transaction == null) return null;

        return await FindTransactionV2ResponseCodeDetail(transaction, transaction.TransactionType);
    }

    public async Task<PaginateResult<TransactionResult<TransactionHistoryV2>>> GetTransactionsV2Paginate(int? page, int? pageSize,
        string? orderBy, string? orderDir, TransactionFilter? filters, CancellationToken cancellationToken = default)
    {
        var filter = await CreateTransactionHistoryV2Filter(page, pageSize, orderBy, orderDir, filters);
        var response = await _depositWithdrawService.GetTransactionHistoriesV2(filter, cancellationToken);
        var transactions = await FindTransactionHistoryV2ResponseCode(response.Data, (TransactionType)filters!.TransactionType!);

        return new PaginateResult<TransactionResult<TransactionHistoryV2>>(transactions, response.Page, response.PageSize, response.Total, response.OrderBy, response.OrderDir);
    }

    public async Task<TransactionDetailResult<TransferCash>?> GetTransferCashByTransactionNo(string transactionNo,
        CancellationToken cancellationToken = default)
    {
        var transaction = await _transferCashService.GetTransferCashByTransactionNo(transactionNo, cancellationToken);
        if (transaction == null) return null;

        return await FindTransferCashResponseCodeDetail(transaction);
    }

    public async Task<PaginateResult<TransactionResult<TransferCash>>?> GetTransferCashPaginate(int? pageNum, int? pageSize, string? orderBy, string? orderDir,
        TransferCashFilter? filters, CancellationToken cancellationToken = default)
    {
        var filter = CreateTransferCashFilter(pageNum, pageSize, orderBy, orderDir, filters);
        var response = await _transferCashService.GetTransferCashHistory(filter, cancellationToken);
        if (response == null) return null;
        var transactions = await FindTransferCashResponseCode(response.Data);

        return new PaginateResult<TransactionResult<TransferCash>>(transactions, response.Page, response.PageSize, response.Total, response.OrderBy, response.OrderDir);
    }

    public Task<List<Product>> GetProducts(ProductType? productType)
    {
        var products = productType switch
        {
            ProductType.GlobalEquity => new List<Product>() { Product.GlobalEquity },
            ProductType.ThaiEquity => new List<Product>() { Product.Cash, Product.Funds, Product.Margin, Product.CashBalance, Product.TFEX },
            _ => Enum.GetValues(typeof(Product)).Cast<Product>().ToList()
        };

        return Task.FromResult(products);
    }

    public Task<List<Models.DepositChannel>> GetDepositChannels()
    {
        var channels = Enum.GetValues(typeof(DepositChannel))
            .Cast<Models.DepositChannel>()
            .Where(q => q != Models.DepositChannel.SetTrade)
            .Where(q => !int.TryParse(q.ToString(), out _))
            .ToList();

        return Task.FromResult(channels);
    }

    public Task<List<Models.WithdrawChannel>> GetWithdrawChannels()
    {
        var channels = Enum.GetValues(typeof(WithdrawChannel))
            .Cast<Models.WithdrawChannel>()
            .ToList();

        return Task.FromResult(channels);
    }

    public async Task<List<Bank>> GetBanks(string? channel)
    {
        if (channel != null) return await _transactionRepository.GetBanksByChannelAsync(channel);

        return await _transactionRepository.GetBanks();
    }

    public async Task<List<ResponseCode>> GetResponseCodes(ResponseCodeFilter filter)
    {
        var responseCodes = await _responseCodeRepository.Get(filter);

        if (filter.ProductType != ProductType.GlobalEquity) return responseCodes;

        var excludeStates = new[] { "DepositRefundSucceed", "DepositRefundFailed", "DepositRefunding" };
        return responseCodes.Where(q => !excludeStates.Contains(q.State)).ToList();
    }

    public Task<List<ResponseCodeAction>> GetResponseCodeAction(Guid id)
    {
        return _responseCodeActionRepository.GetByGuid(id);
    }

    private async Task<TransactionDetailResult<T>> FindTransferCashResponseCodeDetail<T>(T transaction)
        where T : TransferCash
    {
        ResponseCodeDetail? responseCodeDetail = null;
        if (transaction.State != null)
        {
            var responseCode = await _responseCodeRepository.GetByStateMachine(Machine.TransferCash, transaction.State, null);
            if (responseCode != null)
            {
                var actions = await _responseCodeActionRepository.GetByGuid(responseCode.Id);
                responseCodeDetail = QueriesFactory.NewResponseCodeDetail(responseCode, actions);
            }
        }

        responseCodeDetail = await AddDefaultAction(Machine.TransferCash, transaction.Status, responseCodeDetail);

        return new TransactionDetailResult<T>
        {
            Transaction = transaction,
            ResponseCodeDetail = responseCodeDetail
        };
    }

    private async Task<TransactionDetailResult<T>> FindTransactionV2ResponseCodeDetail<T>(T transaction,
        TransactionType type)
    where T : TransactionV2
    {
        var machineType = type switch
        {
            TransactionType.Deposit => Machine.Deposit,
            TransactionType.Withdraw => Machine.Withdraw,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        ResponseCodeDetail? responseCodeDetail = null;
        if (transaction.State != null)
        {
            var responseCode = await _responseCodeRepository.GetByStateMachine(machineType, transaction.State, EntityFactory.NewProductType((Product)transaction.Product!)) ??
                await _responseCodeRepository.GetByStateMachine(machineType, transaction.State, null);
            if (responseCode != null)
            {
                var actions = await _responseCodeActionRepository.GetByGuid(responseCode.Id);
                responseCodeDetail = QueriesFactory.NewResponseCodeDetail(responseCode, actions);
            }
        }

        responseCodeDetail = await AddDefaultAction(machineType, transaction.Status, responseCodeDetail);

        return new TransactionDetailResult<T>
        {
            Transaction = transaction,
            ResponseCodeDetail = responseCodeDetail
        };
    }

    private static Status? GetStatus(string? status)
    {
        return status?.ToLower() switch
        {
            "success" => Status.Success,
            "fail" => Status.Fail,
            "processing" => Status.Processing,
            "pending" => Status.Pending,
            _ => null
        };
    }

    private async Task<TransactionHistoryV2Filter> CreateTransactionHistoryV2Filter(int? page, int? pageSize,
        string? orderBy, string? orderDir, TransactionFilter? filters)
    {
        List<Product>? products = null;
        string? filterState = null;

        if (filters != null)
        {
            if (filters.Product != null)
            {
                products = new List<Product> { (Product)filters.Product };
            }
            else
            {
                products = await GetProducts(filters.ProductType);
            }
            if (filters.ResponseCodeId != null)
            {
                var responseCode = await _responseCodeRepository.Get((Guid)filters.ResponseCodeId);
                filterState = responseCode?.State;
            }
        }

        return new TransactionHistoryV2Filter(
            products,
            filters?.TransactionNumber,
            GetStatus(filters?.Status),
            filterState,
            filters?.TransactionType,
            filters?.Channel,
            filters?.CreatedAtFrom,
            filters?.CreatedAtTo,
            filters?.EffectiveDateFrom,
            filters?.EffectiveDateTo,
            filters?.PaymentReceivedDateFrom,
            filters?.PaymentReceivedDateTo,
            filters?.CustomerCode,
            filters?.AccountCode,
            null,
            filters?.BankCode,
            null,
            null,
            page,
            pageSize,
            orderBy,
            orderDir
        );
    }

    private static TransferCashTransactionFilter CreateTransferCashFilter(int? page, int? pageSize,
        string? orderBy, string? orderDir, TransferCashFilter? filters)
    {
        return new TransferCashTransactionFilter(
            GetStatus(filters?.Status),
            filters?.State,
            filters?.TransactionNo,
            filters?.TransferFromAccountCode,
            filters?.TransferToAccountCode,
            filters?.TransferFromExchangeMarket,
            filters?.TransferToExchangeMarket,
            filters?.OtpConfirmedDateFrom,
            filters?.OtpConfirmedDateTo,
            filters?.CreatedAtFrom,
            filters?.CreatedAtTo,
            page,
            pageSize,
            orderBy,
            orderDir);
    }

    private async Task<List<TransactionResult<TransactionHistoryV2>>> FindTransactionHistoryV2ResponseCode(
        List<TransactionHistoryV2> transactions, TransactionType transactionType)
    {
        var machine = (Machine)transactionType;
        var states = transactions
            .Where(q => q.State != null)
            .Select(q => q.State!)
            .Distinct()
            .ToArray();
        var responseCodes = await _responseCodeRepository.GetByStates(machine, states);

        var transactionsResult = transactions.Select(t =>
        {
            var productType = EntityFactory.NewProductType((Product)t.Product!);
            var responseCode = responseCodes.Find(r =>
                                   string.Equals(r.State, t.State, StringComparison.CurrentCultureIgnoreCase) &&
                                   r.ProductType == productType) ??
                               responseCodes.Find(r =>
                                   string.Equals(r.State, t.State, StringComparison.CurrentCultureIgnoreCase) &&
                                   r.ProductType == null);

            return new TransactionResult<TransactionHistoryV2>
            {
                Transaction =
                    new TransactionHistoryV2
                    {
                        State = t.State,
                        Product = t.Product,
                        AccountCode = t.AccountCode,
                        CustomerName = t.CustomerName,
                        BankAccountNo = t.BankAccountNo,
                        BankAccountName = t.BankAccountName,
                        BankName = t.BankName,
                        EffectiveDate = t.EffectiveDate,
                        PaymentDateTime = t.PaymentDateTime,
                        GlobalAccount = t.GlobalAccount,
                        TransactionNo = t.TransactionNo,
                        TransactionType = t.TransactionType,
                        RequestedAmount = t.RequestedAmount,
                        RequestedCurrency = t.RequestedCurrency,
                        Status = t.Status,
                        CreatedAt = t.CreatedAt,
                        ToCurrency = t.ToCurrency,
                        TransferAmount = t.TransferAmount,
                        Channel = t.Channel,
                        BankAccount = t.BankAccount,
                        Fee = t.Fee,
                        TransferFee = t.TransferFee
                    },
                ResponseCode = responseCode
            };
        }).ToList();

        return transactionsResult;
    }

    private async Task<List<TransactionResult<TransferCash>>> FindTransferCashResponseCode(
        List<TransferCash> transactions)
    {
        var states = transactions
            .Where(q => q.State != null)
            .Select(q => q.State!)
            .Distinct()
            .ToArray();
        var responseCodes = await _responseCodeRepository.GetByStates(Machine.TransferCash, states);

        var transferCashResult = transactions.Select(t =>
        {
            var responseCode = responseCodes.Find(r =>
                                   string.Equals(r.State, t.State, StringComparison.CurrentCultureIgnoreCase) &&
                                   r.ProductType == null);

            return new TransactionResult<TransferCash>
            {
                Transaction =
                    new TransferCash
                    {
                        State = t.State,
                        TransactionNo = t.TransactionNo,
                        Status = t.Status,
                        CustomerName = t.CustomerName,
                        TransferFromAccountCode = t.TransferFromAccountCode,
                        TransferToAccountCode = t.TransferToAccountCode,
                        TransferFromExchangeMarket = t.TransferFromExchangeMarket,
                        TransferToExchangeMarket = t.TransferToExchangeMarket,
                        Amount = t.Amount,
                        FailedReason = t.FailedReason,
                        OtpConfirmedDateTime = t.OtpConfirmedDateTime,
                        CreatedAt = t.CreatedAt,
                    },
                ResponseCode = responseCode
            };
        }).ToList();

        return transferCashResult;
    }

    private async Task<ResponseCodeDetail?> AddDefaultAction(Machine machineType, string? transactionStatus, ResponseCodeDetail? responseCodeDetail)
    {
        if (transactionStatus == "Success")
        {
            return responseCodeDetail;
        }

        var defaultResponseCode = await _responseCodeRepository.GetByStateMachine(machineType, string.Empty, null);
        if (defaultResponseCode == null)
        {
            return responseCodeDetail;
        }

        var defaultActions = await _responseCodeActionRepository.GetByGuid(defaultResponseCode.Id);
        defaultActions = transactionStatus switch
        {
            "Fail" => defaultActions.Where(a => a.Action == Method.ChangeStatusToSuccess).ToList(),
            _ => defaultActions
        };

        if (responseCodeDetail == null)
        {
            responseCodeDetail = new ResponseCodeDetail(
                defaultResponseCode.Id,
                defaultResponseCode.Machine,
                defaultResponseCode.State,
                defaultResponseCode.Suggestion,
                defaultResponseCode.Description,
                defaultActions);
        }
        else
        {
            defaultActions.ForEach(action =>
            {
                var isExist = responseCodeDetail.Actions.Exists(d => d.Action == action.Action);
                if (!isExist)
                {
                    responseCodeDetail.Actions.Add(action);
                }
            });
        }

        return responseCodeDetail;
    }
}
