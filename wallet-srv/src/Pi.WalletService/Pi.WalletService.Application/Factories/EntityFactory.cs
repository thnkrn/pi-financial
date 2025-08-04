using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Queries.Filters;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.RefundAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using CashDepositMachineState = Pi.WalletService.IntegrationEvents.Models.CashDepositState;
using CashWithdrawState = Pi.WalletService.IntegrationEvents.Models.CashWithdrawState;
using DepositMachineState = Pi.WalletService.IntegrationEvents.Models.DepositState;
using WithdrawMachineState = Pi.WalletService.IntegrationEvents.Models.WithdrawState;
using GlobalWalletTransferMachineState = Pi.WalletService.IntegrationEvents.Models.GlobalWalletTransferState;
using RefundMachineState = Pi.WalletService.IntegrationEvents.Models.RefundState;
using DepositEntrypointMachineState = Pi.WalletService.IntegrationEvents.Models.DepositEntrypointState;
using WithdrawEntrypointMachineState = Pi.WalletService.IntegrationEvents.Models.WithdrawEntrypointState;
using QrDepositMachineState = Pi.WalletService.IntegrationEvents.Models.QrDepositState;
using OddDepositMachineState = Pi.WalletService.IntegrationEvents.Models.OddDepositState;
using AtsDepositMachineState = Pi.WalletService.IntegrationEvents.Models.AtsDepositState;
using OddWithdrawMachineState = Pi.WalletService.IntegrationEvents.Models.OddWithdrawState;
using AtsWithdrawMachineState = Pi.WalletService.IntegrationEvents.Models.AtsWithdrawState;
using UpBackMachineState = Pi.WalletService.IntegrationEvents.Models.UpBackState;
using GlobalTransferMachineState = Pi.WalletService.IntegrationEvents.Models.GlobalTransferState;
using RecoveryMachineState = Pi.WalletService.IntegrationEvents.Models.RecoveryState;

namespace Pi.WalletService.Application.Factories;

public static class EntityFactory
{
    public static T NewTransactionFilters<T>(TransactionFilters filters) where T : EntityTransactionFilters, new()
    {
        return new T()
        {
            Channel = filters.Channel,
            State = filters.State,
            Status = filters.Status,
            BankCode = filters.BankCode,
            BankAccountNo = filters.BankAccountNo,
            CustomerCode = filters.CustomerCode,
            AccountCode = filters.AccountCode,
            TransactionNo = filters.TransactionNo,
            EffectiveDateFrom = filters.EffectiveDateFrom,
            EffectiveDateTo = filters.EffectiveDateTo,
            CreatedAtFrom = filters.CreatedAtFrom,
            CreatedAtTo = filters.CreatedAtTo,
        };
    }

    public static T NewTransactionV2Filters<T>(TransactionFilterV2 filters) where T : EntityTransactionV2Filters, new()
    {
        return new T()
        {
            Channel = filters.Channel,
            State = filters.State,
            Status = filters.Status,
            Product = filters.Product,
            UserId = filters.UserId,
            BankCode = filters.BankCode,
            BankName = filters.BankName,
            BankAccountNo = filters.BankAccountNo,
            CustomerCode = filters.CustomerCode,
            AccountCode = filters.AccountCode,
            TransactionNo = filters.TransactionNo,
            CreatedAtFrom = filters.CreatedAtFrom,
            CreatedAtTo = filters.CreatedAtTo,
            EffectiveDateFrom = filters.EffectiveDateFrom,
            EffectiveDateTo = filters.EffectiveDateTo,
            NotStates = filters.NotStates
        };
    }

    public static DepositTransactionFilters NewDepositTransactionV2Filters(TransactionFilterV2 filters)
    {
        var result = NewTransactionV2Filters<DepositTransactionFilters>(filters);
        result.PaymentReceivedDateTimeFrom = filters.PaymentReceivedFrom;
        result.PaymentReceivedDateTimeTo = filters.PaymentReceivedTo;

        return result;
    }

    public static WithdrawTransactionFilters NewWithdrawTransactionV2Filters(TransactionFilterV2 filters)
    {
        var result = NewTransactionV2Filters<WithdrawTransactionFilters>(filters);
        result.PaymentDisbursedDateTimeFrom = filters.EffectiveDateFrom;
        result.PaymentDisbursedDateTimeTo = filters.EffectiveDateTo;

        return result;
    }

    public static SetTradeEPayTransactionFilters NewSetTradeEPayFilter(TransactionFilterV2 filters)
    {
        var result = new SetTradeEPayTransactionFilters
        {
            Product = filters.Product,
            UserId = filters.UserId,
            AccountCode = filters.AccountCode,
            BankName = filters.BankName,
            CustomerCode = filters.CustomerCode,
            CreatedAtFrom = filters.CreatedAtFrom,
            CreatedAtTo = filters.CreatedAtTo,
            Status = filters.Status,
            TransactionNo = filters.TransactionNo
        };

        return result;
    }

    public static CashDepositFilters NewCashDepositFilters(TransactionFilters filters)
    {
        var result = NewTransactionFilters<CashDepositFilters>(filters);
        result.Product = filters.Product;
        result.PaymentReceivedFrom = filters.PaymentReceivedFrom;
        result.PaymentReceivedTo = filters.PaymentReceivedTo;
        result.UserId = filters.UserId;
        result.BankName = filters.BankName;

        return result;
    }

    public static GlobalDepositFilters NewGlobalDepositFilters(TransactionFilters filters)
    {
        var result = NewTransactionFilters<GlobalDepositFilters>(filters);
        result.PaymentReceivedFrom = filters.PaymentReceivedFrom;
        result.PaymentReceivedTo = filters.PaymentReceivedTo;
        result.BankName = filters.BankName;

        return result;
    }

    public static DepositStateFilters NewDepositStateFilters(TransactionFilters filters)
    {
        return new DepositStateFilters
        {
            Channel = filters.Channel,
            Product = filters.Product,
            Products = filters.ProductType != null ? GetProducts((ProductType)filters.ProductType) : null,
            State = filters.State,
            States = filters.Status != null ? GetDepositStates((TransactionStatus)filters.Status) : null,
            BankCode = filters.BankCode,
            BankAccountNo = filters.BankAccountNo,
            CustomerCode = filters.CustomerCode,
            AccountCode = filters.AccountCode,
            TransactionNo = filters.TransactionNo,
            PaymentReceivedFrom = filters.PaymentReceivedFrom,
            PaymentReceivedTo = filters.PaymentReceivedTo,
            CreatedAtFrom = filters.CreatedAtFrom,
            CreatedAtTo = filters.CreatedAtTo,
            UserId = filters.UserId,
            BankName = filters.BankName
        };
    }

    public static WithdrawStateFilters NewWithdrawStateFilters(TransactionFilters filters)
    {
        return new WithdrawStateFilters
        {
            Channel = filters.Channel,
            Product = filters.Product,
            Products = filters.ProductType != null ? GetProducts((ProductType)filters.ProductType) : null,
            State = filters.State,
            States = filters.Status != null ? GetWithdrawStates((TransactionStatus)filters.Status) : null,
            BankCode = filters.BankCode,
            BankAccountNo = filters.BankAccountNo,
            CustomerCode = filters.CustomerCode,
            AccountCode = filters.AccountCode,
            TransactionNo = filters.TransactionNo,
            PaymentDisbursedDateTimeFrom = filters.EffectiveDateFrom,
            PaymentDisbursedDateTimeTo = filters.EffectiveDateTo,
            CreatedAtFrom = filters.CreatedAtFrom,
            CreatedAtTo = filters.CreatedAtTo,
            UserId = filters.UserId
        };
    }

    public static CashWithdrawStateFilters NewCashWithdrawStateFilters(TransactionFilters filters)
    {
        return new CashWithdrawStateFilters
        {
            Channel = filters.Channel,
            Product = filters.Product,
            Products = filters.ProductType != null ? GetProducts((ProductType)filters.ProductType) : null,
            State = filters.State,
            States = filters.Status != null ? GetWithdrawStates((TransactionStatus)filters.Status) : null,
            BankCode = filters.BankCode,
            BankAccountNo = filters.BankAccountNo,
            CustomerCode = filters.CustomerCode,
            AccountCode = filters.AccountCode,
            TransactionNo = filters.TransactionNo,
            CreatedAtFrom = filters.CreatedAtFrom,
            CreatedAtTo = filters.CreatedAtTo,
            UserId = filters.UserId
        };
    }

    public static GlobalTransferStateFilter NewGlobalStateFilter(TransactionFilters filters)
    {
        return new GlobalTransferStateFilter
        {
            State = filters.State,
            States = filters.Status != null ? GetGlobalStates((TransactionStatus)filters.Status, filters.TransactionType) : null,
            CustomerCode = filters.CustomerCode,
            TransactionNo = filters.TransactionNo,
            CreatedAtFrom = filters.CreatedAtFrom,
            CreatedAtTo = filters.CreatedAtTo,
            UserId = filters.UserId,
            NotStates = filters.NotStates,
            TransactionType = filters.TransactionType
        };
    }

    public static RefundStateFilters NewRefundStateFilters(TransactionFilters filters)
    {
        return new RefundStateFilters
        {
            Channel = filters.Channel,
            Product = filters.Product,
            Products = filters.ProductType != null ? GetProducts((ProductType)filters.ProductType) : null,
            State = filters.State,
            States = filters.Status != null ? GetRefundStates((TransactionStatus)filters.Status) : null,
            BankCode = filters.BankCode,
            BankAccountNo = filters.BankAccountNo,
            CustomerCode = filters.CustomerCode,
            AccountCode = filters.AccountCode,
            TransactionNo = filters.TransactionNo,
            RefundAtFrom = filters.EffectiveDateFrom,
            RefundAtTo = filters.EffectiveDateTo,
            CreatedAtFrom = filters.CreatedAtFrom,
            CreatedAtTo = filters.CreatedAtTo,
            UserId = filters.UserId,
            DepositTransactionNo = filters.ReferenceTransactionNo
        };
    }

    public static ThaiDepositFilters NewThaiDepositFilters(TransactionFilters filters)
    {
        var result = NewTransactionFilters<ThaiDepositFilters>(filters);
        result.PaymentReceivedFrom = filters.PaymentReceivedFrom;
        result.PaymentReceivedTo = filters.PaymentReceivedTo;
        result.BankName = filters.BankName;
        result.Product = filters.Product;

        return result;
    }

    public static ThaiWithdrawFilters NewThaiWithdrawFilters(TransactionFilters filters)
    {
        var result = NewTransactionFilters<ThaiWithdrawFilters>(filters);
        result.Product = filters.Product;
        result.UserId = filters.UserId;

        return result;
    }

    public static Product[] GetProducts(ProductType productType)
    {
        return productType switch
        {
            ProductType.GlobalEquity => new[] { Product.GlobalEquities },
            ProductType.ThaiEquity => new[] { Product.Cash, Product.Derivatives, Product.CashBalance, Product.CreditBalance, Product.CreditBalanceSbl, Product.Funds },
            _ => throw new ArgumentOutOfRangeException(nameof(productType), productType, null)
        };
    }

    public static string[] GetDepositEntrypointStates(Status status)
    {
        return status switch
        {
            Status.Success => new[]
            {
                "Final"
            },
            Status.Fail => new[]
            {
                DepositEntrypointMachineState.GetName(() => DepositEntrypointMachineState.DepositFailed),
                DepositEntrypointMachineState.GetName(() => DepositEntrypointMachineState.DepositPaymentNotReceived),
            },
            Status.Pending => new string[] { },
            Status.Processing => new[]
            {
                DepositEntrypointMachineState.GetName(() => DepositEntrypointMachineState.Received),
                DepositEntrypointMachineState.GetName(() => DepositEntrypointMachineState.Initiate),
                DepositEntrypointMachineState.GetName(() => DepositEntrypointMachineState.TransactionNoGenerating),
                DepositEntrypointMachineState.GetName(() => DepositEntrypointMachineState.DepositProcessing),
                DepositEntrypointMachineState.GetName(() => DepositEntrypointMachineState.GlobalTransferProcessing),
                DepositEntrypointMachineState.GetName(() => DepositEntrypointMachineState.UpBackProcessing),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetWithdrawEntrypointStates(Status status)
    {
        return status switch
        {
            Status.Success => new[]
            {
                WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.WithdrawSucceed),
                "Final"
            },
            Status.Fail => new[]
            {
                WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.WithdrawFailed),
                WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.OtpValidationNotReceived),
            },
            Status.Pending => new string[]
            {
                WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.WithdrawFailedRequireActionRecovery),
            },
            Status.Processing => new[]
            {
                WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.Received),
                WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.Initiate),
                WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.TransactionNoGenerating),
                WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.WithdrawValidating),
                WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.WithdrawProcessing),
                WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.GlobalTransferProcessing),
                WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.UpBackProcessing)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetQrDepositStates(Status status)
    {
        return status switch
        {
            Status.Success => new[]
            {
                QrDepositMachineState.GetName(() => QrDepositMachineState.QrDepositCompleted),
                "Final"
            },
            Status.Fail => new[]
            {
                QrDepositMachineState.GetName(() => QrDepositMachineState.QrDepositFailed),
                QrDepositMachineState.GetName(() => QrDepositMachineState.RefundSucceed),
                QrDepositMachineState.GetName(() => QrDepositMachineState.PaymentNotReceived),
            },
            Status.Pending => new[]
            {
                QrDepositMachineState.GetName(() => QrDepositMachineState.RefundFailed),
                QrDepositMachineState.GetName(() => QrDepositMachineState.DepositFailedNameMismatch),
                QrDepositMachineState.GetName(() => QrDepositMachineState.DepositFailedAmountMismatch),
                QrDepositMachineState.GetName(() => QrDepositMachineState.DepositFailedInvalidSource),
            },
            Status.Processing => new[]
            {
                QrDepositMachineState.GetName(() => QrDepositMachineState.Received),
                QrDepositMachineState.GetName(() => QrDepositMachineState.QrCodeGenerating),
                QrDepositMachineState.GetName(() => QrDepositMachineState.WaitingForPayment),
                QrDepositMachineState.GetName(() => QrDepositMachineState.DepositEntrypointUpdating),
                QrDepositMachineState.GetName(() => QrDepositMachineState.PaymentSourceValidating),
                QrDepositMachineState.GetName(() => QrDepositMachineState.PaymentNameValidating),
                QrDepositMachineState.GetName(() => QrDepositMachineState.PaymentAmountValidating),
                QrDepositMachineState.GetName(() => QrDepositMachineState.NameMismatchApproved),
                QrDepositMachineState.GetName(() => QrDepositMachineState.Refunding)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetOddDepositStates(Status status)
    {
        return status switch
        {
            Status.Success => new[]
            {
                OddDepositMachineState.GetName(() => OddDepositMachineState.OddDepositCompleted),
                "Final"
            },
            Status.Fail => new[]
            {
                OddDepositMachineState.GetName(() => OddDepositMachineState.OddDepositFailed),
                OddDepositMachineState.GetName(() => OddDepositMachineState.RequestingOtpValidationFailed),
                OddDepositMachineState.GetName(() => OddDepositMachineState.OtpValidationNotReceived)
            },
            Status.Pending => new string[] { },
            Status.Processing => new[]
            {
                OddDepositMachineState.GetName(() => OddDepositMachineState.Received),
                OddDepositMachineState.GetName(() => OddDepositMachineState.RequestingOtpValidation),
                OddDepositMachineState.GetName(() => OddDepositMachineState.WaitingForOtpValidation),
                OddDepositMachineState.GetName(() => OddDepositMachineState.OddDepositProcessing)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetAtsDepositStates(Status status)
    {
        return status switch
        {
            Status.Success => new[]
            {
                AtsDepositMachineState.GetName(() => AtsDepositMachineState.AtsDepositCompleted),
                "Final"
            },
            Status.Fail => new[]
            {
                AtsDepositMachineState.GetName(() => AtsDepositMachineState.AtsDepositFailed),
                AtsDepositMachineState.GetName(() => AtsDepositMachineState.RequestingOtpValidationFailed),
                AtsDepositMachineState.GetName(() => AtsDepositMachineState.OtpValidationNotReceived)
            },
            Status.Pending => new string[]
            {
                AtsDepositMachineState.GetName(() => AtsDepositMachineState.WaitingForAtsGatewayConfirmation)
            },
            Status.Processing => new[]
            {
                AtsDepositMachineState.GetName(() => AtsDepositMachineState.Received),
                AtsDepositMachineState.GetName(() => AtsDepositMachineState.RequestingOtpValidation),
                AtsDepositMachineState.GetName(() => AtsDepositMachineState.WaitingForOtpValidation),
                AtsDepositMachineState.GetName(() => AtsDepositMachineState.RequestingDepositAts)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetOddWithdrawStates(Status status)
    {
        return status switch
        {
            Status.Success => new[]
            {
                OddWithdrawMachineState.GetName(() => OddWithdrawMachineState.OddWithdrawCompleted),
                "Final"
            },
            Status.Fail => new[]
            {
                OddWithdrawMachineState.GetName(() => OddWithdrawMachineState.RequestingOtpValidationFailed),
                OddWithdrawMachineState.GetName(() => OddWithdrawMachineState.OtpValidationNotReceived)
            },
            Status.Pending => new[]
            {
                OddWithdrawMachineState.GetName(() => OddWithdrawMachineState.OddWithdrawFailed),
            },
            Status.Processing => new[]
            {
                OddWithdrawMachineState.GetName(() => OddWithdrawMachineState.Received),
                OddWithdrawMachineState.GetName(() => OddWithdrawMachineState.WithdrawalInitiating),
                OddWithdrawMachineState.GetName(() => OddWithdrawMachineState.RequestingOtpValidation),
                OddWithdrawMachineState.GetName(() => OddWithdrawMachineState.WaitingForOtpValidation),
                OddWithdrawMachineState.GetName(() => OddWithdrawMachineState.WaitingForConfirmation),
                OddWithdrawMachineState.GetName(() => OddWithdrawMachineState.OddWithdrawProcessing)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetAtsWithdrawStates(Status status)
    {
        return status switch
        {
            Status.Success => new[]
            {
                AtsWithdrawMachineState.GetName(() => AtsWithdrawMachineState.AtsWithdrawCompleted),
                "Final"
            },
            Status.Fail => new[]
            {
                AtsWithdrawMachineState.GetName(() => AtsWithdrawMachineState.AtsWithdrawFailed),
                AtsWithdrawMachineState.GetName(() => AtsWithdrawMachineState.RequestingOtpValidationFailed),
                AtsWithdrawMachineState.GetName(() => AtsWithdrawMachineState.OtpValidationNotReceived)
            },
            Status.Pending => new[]
            {
                AtsWithdrawMachineState.GetName(() => AtsWithdrawMachineState.WaitingForAtsGatewayConfirmation),
            },
            Status.Processing => new[]
            {
                AtsWithdrawMachineState.GetName(() => AtsWithdrawMachineState.Received),
                AtsWithdrawMachineState.GetName(() => AtsWithdrawMachineState.RequestingOtpValidation),
                AtsWithdrawMachineState.GetName(() => AtsWithdrawMachineState.WaitingForOtpValidation),
                AtsWithdrawMachineState.GetName(() => AtsWithdrawMachineState.WaitingForConfirmation),
                AtsWithdrawMachineState.GetName(() => AtsWithdrawMachineState.RequestingWithdrawAts)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetUpBackStates(Status status)
    {
        return status switch
        {
            Status.Success => new[]
            {
                UpBackMachineState.GetName(() => UpBackMachineState.UpBackCompleted),
                "Final"
            },
            Status.Fail => new[]
            {
                UpBackMachineState.GetName(() => UpBackMachineState.UpBackFailed),
            },
            Status.Pending => new[]
            {
                UpBackMachineState.GetName(() => UpBackMachineState.UpBackFailedRequireActionRevert),
                UpBackMachineState.GetName(() => UpBackMachineState.UpBackFailedRequireActionSba),
                UpBackMachineState.GetName(() => UpBackMachineState.UpBackFailedRequireActionSetTrade),
                UpBackMachineState.GetName(() => UpBackMachineState.DepositWaitingForGateway),
                UpBackMachineState.GetName(() => UpBackMachineState.WithdrawWaitingForGateway),
            },
            Status.Processing => new[]
            {
                UpBackMachineState.GetName(() => UpBackMachineState.DepositReceived),
                UpBackMachineState.GetName(() => UpBackMachineState.DepositUpdatingAccountBalance),
                UpBackMachineState.GetName(() => UpBackMachineState.DepositUpdatingTradingPlatform),
                UpBackMachineState.GetName(() => UpBackMachineState.WithdrawReceived),
                UpBackMachineState.GetName(() => UpBackMachineState.WithdrawUpdatingAccountBalance),
                UpBackMachineState.GetName(() => UpBackMachineState.WithdrawUpdatingTradingPlatform),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetGlobalTransferStates(Status status)
    {
        return status switch
        {
            Status.Success => new[]
            {
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.GlobalTransferCompleted),
                "Final"
            },
            Status.Fail => new[]
            {
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.RefundSucceed),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.GlobalTransferFailed),
            },
            Status.Pending => new[]
            {
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.FxFailed),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.FxRateCompareFailed),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.FxTransferFailed),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.RefundFailed),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.FxTransferInsufficientBalance)
            },
            Status.Processing => new[]
            {
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.Received),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.PaymentReceivedDataPreparing),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.FxInitiating),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.FxConfirming),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.FxValidating),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.DepositFxTransferring),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.FxQueryTransaction),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.TransferRequestValidating),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.GlobalTransferPaymentValidating),
                GlobalTransferMachineState.GetName(() => GlobalTransferMachineState.WithdrawFxTransferring),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetGlobalStates(TransactionStatus status, TransactionType? transactionType)
    {
        return status switch
        {
            TransactionStatus.Success => new[]
            {
                "Final"
            },
            TransactionStatus.Fail => new[]
            {
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.DepositFailed),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.TransferRequestFailed),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.RefundSucceed),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.RevertTransferSuccess),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.GlobalWithdrawOtpValidationNotReceived),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.GlobalDepositPaymentNotReceived),
            },
            TransactionStatus.Pending => new[]
            {
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.Received),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.TransactionNoGenerating),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.TransferRequestValidating),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.GlobalTransferPaymentValidating),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.GlobalWithdrawInitiating),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.AwaitingOtpValidation),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.FxInitiating),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.FxValidating),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.FxConfirming),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.FxQueryTransaction),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.FxTransferring),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.FxTransferFailed),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.FxTransferInsufficientBalance),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.WithdrawalProcessing),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.RevertingTransfer),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.FxFailed),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.FxRateCompareFailed),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.RevertTransferFailed),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.ManualAllocationInprogress),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.Refunding),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.RefundFailed),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.DepositProcessing),
                GlobalWalletTransferMachineState.GetName(() => GlobalWalletTransferMachineState.ManualAllocationFailed),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetRefundStates(TransactionStatus status)
    {
        return status switch
        {
            TransactionStatus.Success => new[]
            {
                RefundMachineState.GetName(() => RefundMachineState.RefundSucceed),
            },
            TransactionStatus.Fail => new[]
            {
                RefundMachineState.GetName(() => RefundMachineState.RefundFailed),
            },
            TransactionStatus.Pending => new[]
            {
                RefundMachineState.GetName(() => RefundMachineState.Received),
                RefundMachineState.GetName(() => RefundMachineState.Refunding),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetWithdrawStates(TransactionStatus status)
    {
        return status switch
        {
            TransactionStatus.Success => new[]
            {
                "Final"
            },
            TransactionStatus.Fail => new[]
            {
                WithdrawMachineState.GetName(() => WithdrawMachineState.WithdrawalFailed),
                WithdrawMachineState.GetName(() => WithdrawMachineState.OtpValidationNotReceived),
            },
            TransactionStatus.Pending => new[]
            {
                WithdrawMachineState.GetName(() => WithdrawMachineState.Received),
                WithdrawMachineState.GetName(() => WithdrawMachineState.TransactionNoGenerating),
                WithdrawMachineState.GetName(() => WithdrawMachineState.WithdrawalInitiating),
                WithdrawMachineState.GetName(() => WithdrawMachineState.RequestingOtpValidation),
                WithdrawMachineState.GetName(() => WithdrawMachineState.WaitingForOtpValidation),
                WithdrawMachineState.GetName(() => WithdrawMachineState.WaitingForConfirmation),
                WithdrawMachineState.GetName(() => WithdrawMachineState.WithdrawalProcessing),
                WithdrawMachineState.GetName(() => WithdrawMachineState.RequestingOtpValidationFailed),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetDepositStates(TransactionStatus status)
    {
        return status switch
        {
            TransactionStatus.Success => new[]
            {
                DepositMachineState.GetName(() => DepositMachineState.DepositCompleted)
            },
            TransactionStatus.Fail => new[]
            {
                DepositMachineState.GetName(() => DepositMachineState.DepositRefundSucceed),
                DepositMachineState.GetName(() => DepositMachineState.DepositFailed),
                DepositMachineState.GetName(() => DepositMachineState.DepositPaymentNotReceived),
            },
            TransactionStatus.Pending => new[]
            {
                DepositMachineState.GetName(() => DepositMachineState.Received),
                DepositMachineState.GetName(() => DepositMachineState.TransactionNoGenerating),
                DepositMachineState.GetName(() => DepositMachineState.DepositQrCodeGenerating),
                DepositMachineState.GetName(() => DepositMachineState.DepositWaitingForPayment),
                DepositMachineState.GetName(() => DepositMachineState.DepositPaymentReceived),
                DepositMachineState.GetName(() => DepositMachineState.DepositPaymentSourceValidating),
                DepositMachineState.GetName(() => DepositMachineState.DepositPaymentNameValidating),
                DepositMachineState.GetName(() => DepositMachineState.DepositFailedNameMismatch),
                DepositMachineState.GetName(() => DepositMachineState.DepositFailedAmountMismatch),
                DepositMachineState.GetName(() => DepositMachineState.NameMismatchApproved),
                DepositMachineState.GetName(() => DepositMachineState.DepositRefunding),
                DepositMachineState.GetName(() => DepositMachineState.DepositRefundFailed),
                DepositMachineState.GetName(() => DepositMachineState.DepositFailedInvalidSource)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetSetTradeEPayStatus(Status status)
    {
        return status switch
        {
            Status.Success => new[]
            {
                CashDepositMachineState.GetName(() => CashDepositMachineState.CashDepositCompleted)
            },
            Status.Fail => new[]
            {
                CashDepositMachineState.GetName(() => CashDepositMachineState.CashDepositPaymentNotReceived)
            },
            Status.Pending => new[]
            {
                CashDepositMachineState.GetName(() => CashDepositMachineState.CashDepositWaitingForGateway),
                CashDepositMachineState.GetName(() => CashDepositMachineState.CashDepositFailed),
                CashDepositMachineState.GetName(() => CashDepositMachineState.TfexCashDepositFailed)
            },
            Status.Processing => new[]
            {
                CashDepositMachineState.GetName(() => CashDepositMachineState.Received),
                CashDepositMachineState.GetName(() => CashDepositMachineState.CashDepositTradingPlatformUpdating),
                CashDepositMachineState.GetName(() => CashDepositMachineState.CashDepositWaitingForTradingPlatform),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetCashDepositStates(TransactionStatus status)
    {
        return status switch
        {
            TransactionStatus.Success => new[]
            {
                CashDepositMachineState.GetName(() => CashDepositMachineState.CashDepositCompleted)
            },
            TransactionStatus.Fail => new[]
            {
                CashDepositMachineState.GetName(() => CashDepositMachineState.CashDepositPaymentNotReceived)
            },
            TransactionStatus.Pending => new[]
            {
                CashDepositMachineState.GetName(() => CashDepositMachineState.Received),
                CashDepositMachineState.GetName(() => CashDepositMachineState.CashDepositTradingPlatformUpdating),
                CashDepositMachineState.GetName(() => CashDepositMachineState.CashDepositWaitingForGateway),
                CashDepositMachineState.GetName(() => CashDepositMachineState.CashDepositWaitingForTradingPlatform),
                CashDepositMachineState.GetName(() => CashDepositMachineState.CashDepositFailed),
                CashDepositMachineState.GetName(() => CashDepositMachineState.TfexCashDepositFailed)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetCashWithdrawStates(TransactionStatus status)
    {
        return status switch
        {
            TransactionStatus.Success => new[]
            {
                "Final"
            },
            TransactionStatus.Fail => new[]
            {
                CashWithdrawState.GetName(() => CashWithdrawState.RevertTransferSuccess),
                CashWithdrawState.GetName(() => CashWithdrawState.CashWithdrawOtpValidationNotReceived),
            },
            TransactionStatus.Pending => new[]
            {
                CashWithdrawState.GetName(() => CashWithdrawState.TransferRequestFailed),
                CashWithdrawState.GetName(() => CashWithdrawState.Received),
                CashWithdrawState.GetName(() => CashWithdrawState.TransactionNoGenerating),
                CashWithdrawState.GetName(() => CashWithdrawState.CashWithdrawWaitingForOtpValidation),
                CashWithdrawState.GetName(() => CashWithdrawState.CashWithdrawWaitingForTradingPlatform),
                CashWithdrawState.GetName(() => CashWithdrawState.CashWithdrawWaitingForTFexPlatform),
                CashWithdrawState.GetName(() => CashWithdrawState.CashWithdrawTradingPlatformUpdating),
                CashWithdrawState.GetName(() => CashWithdrawState.WithdrawalProcessing),
                CashWithdrawState.GetName(() => CashWithdrawState.RevertTfexTransfer),
                CashWithdrawState.GetName(() => CashWithdrawState.RevertPlatformTransfer),
                CashWithdrawState.GetName(() => CashWithdrawState.RevertWaitingForPlatformCallback),
                CashWithdrawState.GetName(() => CashWithdrawState.RevertTransferFailed)
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }

    public static string[] GetRecoveryStates(Status status)
    {
        return status switch
        {
            Status.Success => new[]
            {
                RecoveryMachineState.GetName(() => RecoveryMachineState.RevertTransferSuccess),
            },
            Status.Fail => Array.Empty<string>(),
            Status.Pending => new[]
            {
                RecoveryMachineState.GetName(() => RecoveryMachineState.RevertTransferFailed)
            },
            Status.Processing => new[]
            {
                RecoveryMachineState.GetName(() => RecoveryMachineState.RevertRequestReceived),
                RecoveryMachineState.GetName(() => RecoveryMachineState.RevertTransferInitiate),
                RecoveryMachineState.GetName(() => RecoveryMachineState.RevertingTransfer),
            },
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}
