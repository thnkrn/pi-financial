using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Models.Customer;
using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Domain.AggregateModels.TransferCashAggregate;
using Pi.Client.OnboardService.Model;
using Pi.Client.UserService.Model;
using Pi.Client.WalletService.Model;
using Product = Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate.Product;
using WithdrawChannel = Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate.WithdrawChannel;
using DepositChannel = Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate.DepositChannel;

namespace Pi.BackofficeService.Infrastructure.Factories;

public static class EntityFactory
{
    public static TransactionHistoryV2 NewTransactionHistoryV2(
        PiWalletServiceAPIModelsTransactionHistoryV2 transaction)
    {
        return new TransactionHistoryV2
        {
            State = transaction.State,
            Product = NewProduct((PiWalletServiceIntegrationEventsAggregatesModelProduct)transaction.Product!)!,
            AccountCode = transaction.AccountCode,
            CustomerName = transaction.CustomerName,
            BankAccountNo = transaction.BankAccountNo,
            BankAccountName = transaction.BankAccountName,
            BankName = transaction.BankName,
            EffectiveDate = DateOnly.FromDateTime(transaction.EffectiveDate!.Value),
            PaymentDateTime = transaction.PaymentDateTime,
            GlobalAccount = transaction.GlobalAccount,
            TransactionNo = transaction.TransactionNo,
            TransactionType = (TransactionType)transaction.TransactionType!,
            RequestedAmount = transaction.RequestedAmount,
            RequestedCurrency = (Currency)NewCurrency((PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency)transaction.RequestedCurrency!)!,
            Status = transaction.Status,
            CreatedAt = transaction.CreatedAt ?? DateTime.Now,
            ToCurrency = transaction.ToCurrency != null ? NewCurrency((PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency)transaction.ToCurrency) : null,
            TransferAmount = transaction.TransferAmount,
            Channel = NewChannel((PiWalletServiceIntegrationEventsAggregatesModelChannel)transaction.Channel!),
            BankAccount = transaction.BankAccount,
            Fee = transaction.Fee,
            TransferFee = transaction.TransferFee
        };
    }

    public static TransactionV2 NewTransactionV2(
        PiWalletServiceAPIModelsTransactionDetailsDto transaction)
    {
        return new TransactionV2
        {
            Id = transaction.CorrelationId,
            State = transaction.CurrentState,
            Status = transaction.Status.ToString(),
            TransactionNo = transaction.TransactionNo,
            TransactionType = (TransactionType)transaction.TransactionType!,
            Product = transaction.Product != null ? NewProduct((PiWalletServiceIntegrationEventsAggregatesModelProduct)transaction.Product) : null,
            Channel = transaction.Channel != null ? NewChannel((PiWalletServiceIntegrationEventsAggregatesModelChannel)transaction.Channel) : null,
            AccountCode = transaction.AccountCode,
            CustomerCode = transaction.CustomerCode,
            CustomerName = transaction.CustomerName,
            BankAccountNo = transaction.BankAccountNo,
            BankAccountName = transaction.BankAccountName,
            BankName = transaction.BankName,
            RequestedAmount = transaction.RequestedAmount,
            RequestedCurrency = transaction.RequestedCurrency != null ? (Currency)NewCurrency((PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency)transaction.RequestedCurrency!)! : Currency.THB,
            ToCurrency = transaction.ToCurrency != null ? NewCurrency((PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency)transaction.ToCurrency) : null,
            PaymentReceivedAmount = transaction.PaymentReceivedAmount,
            PaymentDisbursedAmount = transaction.PaymentDisbursedAmount,
            ConfirmedAmount = transaction.ConfirmedAmount,
            TransferAmount = transaction.TransferAmount,
            Fee = transaction.Fee,
            FailedReason = transaction.FailedReason,
            TransferFee = transaction.GlobalTransfer?.TransferFee?.ToString("0.00"),
            EffectiveDate = DateOnly.FromDateTime(transaction.PaymentAt),
            CreatedAt = transaction.CreatedAt,
            QrDeposit = NewQrDepositState(transaction.QrDeposit),
            OddDeposit = NewOddDepositState(transaction.OddDeposit),
            AtsDeposit = NewAtsDepositState(transaction.AtsDeposit),
            OddWithdraw = NewOddWithdrawState(transaction.OddWithdraw),
            AtsWithdraw = NewAtsWithdrawState(transaction.AtsWithdraw),
            GlobalTransfer = NewGlobalTransferState(transaction.GlobalTransfer),
            UpBack = NewUpBackState(transaction.UpBack),
            Recovery = NewRecoveryState(transaction.Recovery),
            Refund = NewRefundInfo(transaction.RefundInfo),
            BillPayment = NewBillPaymentState(transaction.BillPayment)
        };
    }

    private static QrDepositState? NewQrDepositState(
        PiWalletServiceDomainAggregatesModelQrDepositAggregateQrDepositState? state)
    {
        if (state == null)
        {
            return null;
        }
        return new QrDepositState
        {
            State = state.CurrentState,
            DepositQrGenerateDateTime = state.DepositQrGenerateDateTime,
            QrTransactionNo = state.QrTransactionNo,
            QrTransactionRef = state.QrTransactionRef,
            QrValue = state.QrValue,
            PaymentReceivedAmount = state.PaymentReceivedAmount,
            PaymentReceivedDateTime = state.PaymentReceivedDateTime,
            Fee = state.Fee,
            FailedReason = state.FailedReason,
            CreatedAt = state.CreatedAt,
            UpdatedAt = state.UpdatedAt
        };
    }

    private static OddDepositState? NewOddDepositState(
        PiWalletServiceDomainAggregatesModelOddDepositAggregateOddDepositState? state)
    {
        if (state == null)
        {
            return null;
        }
        return new OddDepositState
        {
            State = state.CurrentState,
            Fee = state.Fee,
            OtpRequestId = state.OtpRequestId,
            OtpRequestRef = state.OtpRequestRef,
            OtpConfirmedDateTime = state.OtpConfirmedDateTime,
            PaymentReceivedAmount = state.PaymentReceivedAmount,
            PaymentReceivedDateTime = state.PaymentReceivedDateTime,
            FailedReason = state.FailedReason,
            CreatedAt = state.CreatedAt,
            UpdatedAt = state.UpdatedAt
        };
    }

    private static AtsDepositState? NewAtsDepositState(
        PiWalletServiceDomainAggregatesModelAtsDepositAggregateAtsDepositState? state)
    {
        if (state == null)
        {
            return null;
        }
        return new AtsDepositState
        {
            State = state.CurrentState,
            OtpRequestId = state.OtpRequestId,
            OtpRequestRef = state.OtpRequestRef,
            OtpConfirmedDateTime = state.OtpConfirmedDateTime,
            PaymentReceivedAmount = state.PaymentReceivedAmount,
            PaymentReceivedDateTime = state.PaymentReceivedDateTime,
            Fee = state.Fee,
            FailedReason = state.FailedReason,
            CreatedAt = state.CreatedAt,
            UpdatedAt = state.UpdatedAt
        };
    }

    private static BillPaymentState? NewBillPaymentState(
        PiWalletServiceDomainAggregatesModelBillPaymentAggregateBillPaymentState? state)
    {
        if (state == null)
        {
            return null;
        }
        return new BillPaymentState
        {
            State = state.CurrentState,
            PaymentReceivedAmount = state.PaymentReceivedAmount,
            PaymentReceivedDateTime = state.PaymentReceivedDateTime,
            Reference1 = state.Reference1,
            Reference2 = state.Reference2,
            CustomerPaymentName = state.CustomerPaymentName,
            CustomerPaymentBankCode = state.CustomerPaymentBankCode,
            BillPaymentTransactionRef = state.BillPaymentTransactionRef,
            Fee = state.Fee,
            FailedReason = state.FailedReason,
        };
    }

    private static OddWithdrawState? NewOddWithdrawState(
        PiWalletServiceDomainAggregatesModelOddWithdrawAggregateOddWithdrawState? state)
    {
        if (state == null)
        {
            return null;
        }
        return new OddWithdrawState
        {
            State = state.CurrentState,
            Fee = state.Fee,
            OtpRequestId = state.OtpRequestId,
            OtpRequestRef = state.OtpRequestRef,
            OtpConfirmedDateTime = state.OtpConfirmedDateTime,
            PaymentDisbursedAmount = state.PaymentDisbursedAmount,
            PaymentDisbursedDateTime = state.PaymentDisbursedDateTime,
            FailedReason = state.FailedReason,
            CreatedAt = state.CreatedAt,
            UpdatedAt = state.UpdatedAt
        };
    }

    private static AtsWithdrawState? NewAtsWithdrawState(
        PiWalletServiceDomainAggregatesModelAtsWithdrawAggregateAtsWithdrawState? state)
    {
        if (state == null)
        {
            return null;
        }
        return new AtsWithdrawState
        {
            State = state.CurrentState,
            OtpRequestId = state.OtpRequestId,
            OtpRequestRef = state.OtpRequestRef,
            OtpConfirmedDateTime = state.OtpConfirmedDateTime,
            PaymentDisbursedAmount = state.PaymentDisbursedAmount,
            PaymentDisbursedDateTime = state.PaymentDisbursedDateTime,
            Fee = state.Fee,
            FailedReason = state.FailedReason,
            CreatedAt = state.CreatedAt,
            UpdatedAt = state.UpdatedAt
        };
    }

    private static GlobalTransferState? NewGlobalTransferState(
        PiWalletServiceDomainAggregatesModelGlobalTransferGlobalTransferState? state)
    {
        if (state == null)
        {
            return null;
        }
        return new GlobalTransferState
        {
            State = state.CurrentState,
            GlobalAccount = state.GlobalAccount,
            RequestedCurrency = state.RequestedCurrency != null ? NewCurrency((PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency)state.RequestedCurrency) : null,
            RequestedFxRate = state.RequestedFxRate,
            RequestedFxCurrency = state.RequestedFxCurrency != null ? NewCurrency((PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency)state.RequestedFxCurrency) : null,
            PaymentReceivedAmount = state.ExchangeAmount,
            PaymentReceivedCurrency = state.ExchangeCurrency != null ? NewCurrency((PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency)state.ExchangeCurrency) : null,
            FxInitiateRequestDateTime = state.FxInitiateRequestDateTime,
            FxTransactionId = state.FxTransactionId,
            FxConfirmedAmount = state.FxConfirmedAmount,
            FxConfirmedExchangeRate = state.FxConfirmedExchangeRate,
            FxConfirmedCurrency = state.FxConfirmedCurrency != null ? NewCurrency((PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency)state.FxConfirmedCurrency) : null,
            FxConfirmedDateTime = state.FxConfirmedDateTime,
            TransferAmount = state.TransferAmount,
            TransferCurrency = state.TransferCurrency != null ? NewCurrency((PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency)state.TransferCurrency) : null,
            TransferFee = state.TransferFee,
            TransferFromAccount = state.TransferFromAccount,
            TransferToAccount = state.TransferToAccount,
            TransferRequestTime = state.TransferRequestTime,
            TransferCompleteTime = state.TransferCompleteTime,
            FailedReason = state.FailedReason,
            CreatedAt = state.CreatedAt,
            UpdatedAt = state.UpdatedAt
        };
    }

    private static UpBackState? NewUpBackState(
        PiWalletServiceDomainAggregatesModelUpBackAggregateUpBackState? state)
    {
        if (state == null)
        {
            return null;
        }
        return new UpBackState
        {
            State = state.CurrentState,
            FailedReason = state.FailedReason,
            CreatedAt = state.CreatedAt,
            UpdatedAt = state.UpdatedAt
        };
    }

    private static RecoveryState? NewRecoveryState(
        PiWalletServiceDomainAggregatesModelRecoveryAggregateRecoveryState? state)
    {
        if (state == null)
        {
            return null;
        }
        return new RecoveryState
        {
            State = state.CurrentState,
            GlobalAccount = state.GlobalAccount,
            TransferAmount = state.TransferAmount,
            TransferCurrency = state.TransferCurrency != null ?
                NewCurrency((PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency)state.TransferCurrency!) : null,
            TransferFromAccount = state.TransferFromAccount,
            TransferToAccount = state.TransferToAccount,
            TransferRequestTime = state.TransferRequestTime,
            TransferCompleteTime = state.TransferCompleteTime,
            FailedReason = state.FailedReason,
            CreatedAt = state.CreatedAt,
            UpdatedAt = state.UpdatedAt
        };
    }

    private static RefundInfo? NewRefundInfo(
        PiWalletServiceDomainAggregatesModelRefundInfoAggregateRefundInfo? refundInfo)
    {
        if (refundInfo == null)
        {
            return null;
        }
        return new RefundInfo
        {
            RefundId = refundInfo.Id.ToString(),
            Amount = refundInfo.Amount,
            TransferToAccountNo = refundInfo.TransferToAccountNo,
            TransferToAccountName = refundInfo.TransferToAccountName,
            Fee = refundInfo.Fee,
            CreatedAt = refundInfo.CreatedAt
        };
    }

    public static TransferCash? NewTransferCash(
        PiWalletServiceAPIModelsTransferCashDto? transaction)
    {
        if (transaction == null)
        {
            return null;
        }

        return new TransferCash
        {
            Id = transaction.CorrelationId,
            TransactionNo = transaction.TransactionNo,
            State = transaction.State,
            Status = transaction.Status.ToString(),
            CustomerName = transaction.CustomerName,
            Amount = transaction.Amount,
            TransferFromAccountCode = transaction.TransferFromAccountCode,
            TransferFromExchangeMarket = transaction.TransferFromExchangeMarket != null
                ? NewProduct(
                    (PiWalletServiceIntegrationEventsAggregatesModelProduct)transaction.TransferFromExchangeMarket)
                : null,
            TransferToAccountCode = transaction.TransferToAccountCode,
            TransferToExchangeMarket = transaction.TransferToExchangeMarket != null
                ? NewProduct(
                    (PiWalletServiceIntegrationEventsAggregatesModelProduct)transaction.TransferToExchangeMarket)
                : null,
            OtpConfirmedDateTime = transaction.OtpConfirmedDateTime,
            FailedReason = transaction.FailedReason,
            CreatedAt = transaction.CreatedAt
        };
    }

    public static WithdrawChannel? NewWithdrawChannel(
        PiWalletServiceIntegrationEventsAggregatesModelChannel channel)
    {
        return channel switch
        {
            PiWalletServiceIntegrationEventsAggregatesModelChannel.OnlineViaKKP => WithdrawChannel
                .OnlineTransfer,
            PiWalletServiceIntegrationEventsAggregatesModelChannel.ATS => WithdrawChannel.AtsBatch,
            _ => null
        };
    }

    public static Product? NewProduct(PiWalletServiceIntegrationEventsAggregatesModelProduct product)
    {
        return product switch
        {
            PiWalletServiceIntegrationEventsAggregatesModelProduct.Funds => Product.Funds,
            PiWalletServiceIntegrationEventsAggregatesModelProduct.Derivatives => Product.TFEX,
            PiWalletServiceIntegrationEventsAggregatesModelProduct.Cash => Product.Cash,
            PiWalletServiceIntegrationEventsAggregatesModelProduct.Crypto => Product.Crypto,
            PiWalletServiceIntegrationEventsAggregatesModelProduct.CashBalance => Product.CashBalance,
            PiWalletServiceIntegrationEventsAggregatesModelProduct.CreditBalanceSbl => Product.Margin,
            PiWalletServiceIntegrationEventsAggregatesModelProduct.GlobalEquities => Product.GlobalEquity,
            _ => null
        };
    }

    public static PiWalletServiceIntegrationEventsAggregatesModelProduct MapProductToWalletModelProduct(Product product)
    {
        return product switch
        {
            Product.Funds => PiWalletServiceIntegrationEventsAggregatesModelProduct.Funds,
            Product.TFEX => PiWalletServiceIntegrationEventsAggregatesModelProduct.Derivatives,
            Product.Cash => PiWalletServiceIntegrationEventsAggregatesModelProduct.Cash,
            Product.Crypto => PiWalletServiceIntegrationEventsAggregatesModelProduct.Crypto,
            Product.CashBalance => PiWalletServiceIntegrationEventsAggregatesModelProduct.CashBalance,
            Product.Margin => PiWalletServiceIntegrationEventsAggregatesModelProduct.CreditBalanceSbl,
            Product.GlobalEquity => PiWalletServiceIntegrationEventsAggregatesModelProduct.GlobalEquities,
            _ => throw new ArgumentOutOfRangeException(nameof(product))
        };
    }

    public static DepositChannel? NewDepositChannel(
        PiWalletServiceIntegrationEventsAggregatesModelChannel channel)
    {
        return channel switch
        {
            PiWalletServiceIntegrationEventsAggregatesModelChannel.QR => DepositChannel.QR,
            PiWalletServiceIntegrationEventsAggregatesModelChannel.ATS => DepositChannel.AtsBatch,
            PiWalletServiceIntegrationEventsAggregatesModelChannel.BillPayment => DepositChannel.BillPayment,
            PiWalletServiceIntegrationEventsAggregatesModelChannel.SetTrade => DepositChannel.SetTrade,
            _ => null
        };
    }

    private static Channel? NewChannel(
        PiWalletServiceIntegrationEventsAggregatesModelChannel channel)
    {
        return channel switch
        {
            PiWalletServiceIntegrationEventsAggregatesModelChannel.QR => Channel.QR,
            PiWalletServiceIntegrationEventsAggregatesModelChannel.ATS => Channel.AtsBatch,
            PiWalletServiceIntegrationEventsAggregatesModelChannel.SetTrade => Channel.SetTrade,
            PiWalletServiceIntegrationEventsAggregatesModelChannel.BillPayment => Channel.BillPayment,
            PiWalletServiceIntegrationEventsAggregatesModelChannel.OnlineViaKKP => Channel.OnlineTransfer,
            PiWalletServiceIntegrationEventsAggregatesModelChannel.ODD => Channel.ODD,
            _ => null
        };
    }

    private static Currency? NewCurrency(PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency currency)
    {
        return currency switch
        {
            PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency.THB => Currency.THB,
            PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency.USD => Currency.USD,
            _ => null
        };
    }

    public static List<OpenAccountInfoDto> NewOpenAccounts(
        List<PiOnboardServiceAPIModelsOpenAccountOpenAccountInfoDto> responseObj)
    {
        var result = new List<OpenAccountInfoDto>();

        foreach (var item in responseObj)
            result.Add(new OpenAccountInfoDto
            {
                Id = item.Id,
                CustCode = item.CustCode,
                Status = item.Status,
                BpmReceived = item.BpmReceived,
                CreatedDate = item.CreatedDate,
                UpdatedDate = item.UpdatedDate,
                ReferId = item.ReferId,
                TransId = item.TransId,

                Identification = NewOpenAccountIdentification(item.Identification),
                Documents = NewOpenAccountDocuments(item.Documents)
            });
        return result;
    }

    private static List<Document> NewOpenAccountDocuments(
        List<PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto> docs)
    {
        return docs.Select(item => new Document(item.FileUrl, item.FileName,
            item.DocumentType.HasValue ? item.DocumentType.Value.ToString() : "")).ToList();
    }

    private static Identification NewOpenAccountIdentification(
        PiOnboardServiceApplicationModelsOpenAccountIdentification identification)
    {
        return new Identification
        {
            UserId = identification.UserId,
            CitizenId = identification.CitizenId,
            Title = identification.Title,
            FirstNameTh = identification.FirstNameTh,
            LastNameTh = identification.LastNameTh,
            FirstNameEn = identification.FirstNameEn,
            LastNameEn = identification.LastNameEn,
            DateOfBirth = identification.FormattedDateOfBirth,
            IdCardExpiryDate = identification.FormattedIdCardExpiryDate,
            LaserCode = identification.LaserCode,
            Nationality = identification.Nationality,
            Email = identification.Email,
            Phone = identification.Phone
        };
    }

    public static CustomerDto NewUser(PiUserApplicationModelsUser data)
    {
        return new CustomerDto(
            data.Id.ToString(),
            data.Devices.Select(NewDevice).ToList(),
            data.CustomerCodes.Select(NewCustomerCode).ToList(),
            data.TradingAccounts.Select(NewTradingAccount).ToList(),
            data.FirstnameTh,
            data.LastnameTh,
            data.FirstnameEn,
            data.LastnameEn,
            data.PhoneNumber,
            data.GlobalAccount,
            data.Email
        );
    }

    public static TradingAccountDto NewTradingAccount(PiUserDomainAggregatesModelUserInfoAggregateTradingAccount data)
    {
        return new TradingAccountDto(data.TradingAccountId);
    }

    public static CustomerCodeDto NewCustomerCode(PiUserApplicationModelsCustomerCode data)
    {
        return new CustomerCodeDto(
            data.Code,
            data.TradingAccounts
        );
    }

    public static DeviceDto NewDevice(PiUserApplicationModelsDevice data)
    {
        return new DeviceDto(
            data.DeviceId,
            data.DeviceToken,
            data.DeviceIdentifier,
            data.Language,
            data.Platform,
            data.NotificationPreference == null ? null : NewNotificationPreference(data.NotificationPreference)
        );
    }

    public static NotificationPreferenceDto NewNotificationPreference(PiUserApplicationModelsNotificationPreference data)
    {
        return new NotificationPreferenceDto(
            data.Important,
            data.Order,
            data.Portfolio,
            data.Wallet,
            data.Market
        );
    }
}
