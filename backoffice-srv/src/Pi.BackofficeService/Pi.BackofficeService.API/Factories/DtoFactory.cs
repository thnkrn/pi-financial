using System.ComponentModel;
using System.Reflection;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Domain.AggregateModels.ResponseCodeAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;
using Pi.BackofficeService.Domain.AggregateModels.User;
using DepositTransaction = Pi.BackofficeService.Application.Models.DepositTransaction;
using WithdrawTransaction = Pi.BackofficeService.Application.Models.WithdrawTransaction;

namespace Pi.BackofficeService.API.Factories;

public static class DtoFactory
{
    public static OnboardingAccountListFilter NewOnboardAccountFilter(OnBoardingFilterRequest filterRequest)
    {
        return new OnboardingAccountListFilter(
            filterRequest?.UserId,
            filterRequest?.Status.ToString(),
            filterRequest?.CitizenId,
            filterRequest?.Custcode,
            filterRequest?.Date,
            filterRequest?.BpmReceived
        );
    }


    public static OnboardingOpenAccountResponse NewOnboardingAccountResponse(OpenAccountInfoDto account)
    {
        return new OnboardingOpenAccountResponse
        {
            Id = account.Id,
            Identification = new IdentificationResponse(account.Identification),
            Documents = NewDocumentsResponse(account.Documents),

            Status = account.Status,
            CreatedDate = account.CreatedDate,
            UpdatedDate = account.UpdatedDate,
            BpmReceived = account.BpmReceived,
            CustCode = account.CustCode,
            ReferId = account.ReferId,
            TransId = account.TransId,
        };
    }

    public static TransactionFilter NewTransactionFilter(TransactionFilterRequest filterRequest)
    {
        return new TransactionFilter(
            filterRequest?.Channel,
            filterRequest?.TransactionType,
            filterRequest?.AccountType,
            filterRequest?.ProductType,
            filterRequest?.ResponseCodeId,
            filterRequest?.BankCode,
            filterRequest?.AccountNumber,
            filterRequest?.CustomerCode,
            filterRequest?.AccountCode,
            filterRequest?.TransactionNumber,
            filterRequest?.Status,
            filterRequest?.EffectiveDateFrom?.ToDateTime(new TimeOnly(0, 0, 0)),
            filterRequest?.EffectiveDateTo?.AddDays(1).ToDateTime(new TimeOnly(0, 0, 0))
                .Subtract(new TimeSpan(0, 0, 0, 0, 1)),
            filterRequest?.PaymentReceivedDateFrom?.ToDateTime(new TimeOnly(0, 0, 0)),
            filterRequest?.PaymentReceivedDateTo?.AddDays(1).ToDateTime(new TimeOnly(0, 0, 0))
                .Subtract(new TimeSpan(0, 0, 0, 0, 1)),
            filterRequest?.CreatedAtFrom?.ToDateTime(new TimeOnly(0, 0, 0)),
            filterRequest?.CreatedAtTo?.AddDays(1).ToDateTime(new TimeOnly(0, 0, 0))
                .Subtract(new TimeSpan(0, 0, 0, 0, 1))
        );
    }

    public static DepositTransactionResponse NewDepositTransactionResponse(TransactionResult<DepositTransaction> transactionResult)
    {
        return new DepositTransactionResponse(transactionResult);
    }

    public static DepositTransactionDetailResponse NewDepositTransactionDetailResponse(TransactionDetailResult<DepositTransaction> transactionResult)
    {
        return new DepositTransactionDetailResponse(transactionResult);
    }


    public static WithdrawTransactionResponse NewWithdrawTransactionResponse(TransactionResult<WithdrawTransaction> transactionResult)
    {
        return new WithdrawTransactionResponse(transactionResult);
    }

    public static WithdrawTransactionDetailResponse NewWithdrawTransactionDetailResponse(TransactionDetailResult<WithdrawTransaction> transactionResult)
    {
        return new WithdrawTransactionDetailResponse(transactionResult);
    }

    public static TransactionV2DetailResponse NewTransactionV2DetailResponse(
        TransactionDetailResult<TransactionV2> transactionResult)
    {
        return new TransactionV2DetailResponse(transactionResult);
    }

    public static TransactionHistoryV2Response NewTransactionHistoryV2Response(
        TransactionResult<TransactionHistoryV2> transactionResult)
    {
        return new TransactionHistoryV2Response(transactionResult);
    }

    public static TransferCashDetailResponse NewTransferCashDetailResponse(
        TransactionDetailResult<TransferCash> transactionResult)
    {
        return new TransferCashDetailResponse(transactionResult);
    }

    public static TicketResponse NewTicketResponse(TicketState ticket)
    {
        return new TicketResponse(
            ticket.CorrelationId,
            ticket.TicketNo!,
            ticket.TransactionId,
            ticket.TransactionNo,
            ticket.TransactionType,
            ticket.CustomerName,
            ticket.CustomerCode,
            ticket.Status,
            ticket.RequestAction,
            ticket.RequestedAt,
            ticket.MakerId,
            ticket.MakerRemark,
            ticket.CheckerAction,
            ticket.CheckedAt,
            ticket.CheckerId,
            ticket.CheckerRemark,
            null
        );
    }

    public static TicketDetailResponse NewTicketResponse(TicketResult ticket)
    {
        return new TicketDetailResponse(
            ticket.CorrelationId,
            ticket.TicketNo!,
            ticket.TransactionId,
            ticket.TransactionNo,
            ticket.TransactionType,
            ticket.CustomerName,
            ticket.CustomerCode,
            ticket.Status,
            ticket.RequestAction != null ? NewGenericNameAliasResponse((TicketAction)ticket.RequestAction) : null,
            ticket.RequestedAt,
            ticket.Maker != null ? NewUserResponse(ticket.Maker) : null,
            ticket.MakerRemark,
            ticket.CheckerAction != null ? NewGenericNameAliasResponse((TicketAction)ticket.CheckerAction) : null,
            ticket.CheckedAt,
            ticket.Checker != null ? NewUserResponse(ticket.Checker) : null,
            ticket.CheckerRemark,
            ticket.ResponseCode != null ? NewResponseCodeResponse(ticket.ResponseCode) : null,
            ticket.CreatedAt
        );
    }

    public static ReportResponse NewReportResponse(ReportHistory reportQueryResult)
    {
        return new ReportResponse
        {
            Id = reportQueryResult.Id,
            Status = reportQueryResult.Status.ToString(),
            Name = reportQueryResult.Name,
            Type = reportQueryResult.Type,
            UserName = reportQueryResult.UserName,
            DateFrom = reportQueryResult.DateFrom,
            DateTo = reportQueryResult.DateTo,
            GeneratedAt = reportQueryResult.GeneratedAt,
            CreatedAt = reportQueryResult.CreatedAt
        };
    }

    public static List<DocumentResponse> NewDocumentsResponse(IList<Document>? documents)
    {
        if (documents == null)
            return new List<DocumentResponse>();

        return documents.Select(NewDocumentResponse).ToList();
    }

    public static DocumentResponse NewDocumentResponse(Document doc)
    {
        return new DocumentResponse(doc.Url, doc.FileName, doc.DocumentType);
    }


    public static UserResponse NewUserResponse(User user)
    {
        return new UserResponse(user.Id, user.FirstName, user.LastName, user.Email);
    }

    public static NameAliasResponse<T> NewGenericNameAliasResponse<T>(T data) where T : Enum
    {
        return new NameAliasResponse<T>(data);
    }

    public static NameAliasResponse NewNameAliasResponse<T>(T data) where T : Enum
    {
        var field = typeof(T).GetField(data.ToString());
        var attribute = field?.GetCustomAttribute<DescriptionAttribute>();

        return new NameAliasResponse(attribute?.Description ?? data.ToString(), data.ToString());
    }

    public static ResponseCodeResponse NewResponseCodeResponse(ResponseCode responseCode)
    {
        return new ResponseCodeResponse(
            responseCode.Id,
            responseCode.Machine,
            responseCode.ProductType,
            responseCode.Suggestion,
            responseCode.Description,
            responseCode.State
        );
    }

    public static ResponseCodeDetailResponse NewResponseCodeDetailResponse(ResponseCodeDetail responseCodeMapping)
    {
        return new ResponseCodeDetailResponse(
            responseCodeMapping.Id,
            responseCodeMapping.Suggestion,
            responseCodeMapping.Description,
            responseCodeMapping.Actions.Select(q => NewResponseCodeActionsResponse(q)).ToList()
        );
    }

    public static ResponseCodeActionsResponse NewResponseCodeActionsResponse(ResponseCodeAction responseCodeAction)
    {
        return new ResponseCodeActionsResponse(
            responseCodeAction.Id,
            (TicketAction)responseCodeAction.Action
        );
    }
}
