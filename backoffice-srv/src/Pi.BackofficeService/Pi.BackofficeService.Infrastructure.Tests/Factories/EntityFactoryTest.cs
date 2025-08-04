using Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate;
using Pi.BackofficeService.Infrastructure.Factories;
using Pi.Client.OnboardService.Model;
using Pi.Client.UserService.Model;
using Pi.Client.WalletService.Model;
using ProductAggregate = Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate.Product;
using CurrencyAggregate = Pi.BackofficeService.Domain.AggregateModels.TransactionAggregate.Currency;

namespace Pi.BackofficeService.Infrastructure.Tests.Factories;

public class EntityFactoryTest
{
    [Theory]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelChannel.QR, DepositChannel.QR)]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelChannel.BillPayment, DepositChannel.BillPayment)]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelChannel.ATS, DepositChannel.AtsBatch)]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelChannel.SetTrade, DepositChannel.SetTrade)]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelChannel.OnlineViaKKP, null)]
    public void Should_Return_ExpectedDepositChannel_When_NewDepositChannel(
        PiWalletServiceIntegrationEventsAggregatesModelChannel channel, DepositChannel? expected)
    {
        // Act
        var actual = EntityFactory.NewDepositChannel(channel);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelChannel.QR, null)]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelChannel.ATS, WithdrawChannel.AtsBatch)]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelChannel.OnlineViaKKP,
        WithdrawChannel.OnlineTransfer)]
    public void Should_Return_ExpectedWithdrawChannel_When_NewWithdrawChannel(
        PiWalletServiceIntegrationEventsAggregatesModelChannel channel, WithdrawChannel? expected)
    {
        // Act
        var actual = EntityFactory.NewWithdrawChannel(channel);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelProduct.Funds, ProductAggregate.Funds)]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelProduct.Derivatives, ProductAggregate.TFEX)]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelProduct.Cash, ProductAggregate.Cash)]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelProduct.Crypto, ProductAggregate.Crypto)]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelProduct.CashBalance,
        ProductAggregate.CashBalance)]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelProduct.CreditBalanceSbl,
        ProductAggregate.Margin)]
    [InlineData(PiWalletServiceIntegrationEventsAggregatesModelProduct.GlobalEquities,
        ProductAggregate.GlobalEquity)]
    public void Should_Return_ExpectedProduct_When_NewProduct(
        PiWalletServiceIntegrationEventsAggregatesModelProduct product, ProductAggregate? expected)
    {
        // Act
        var actual = EntityFactory.NewProduct(product);

        // Assert
        Assert.Equal(expected, actual);
    }


    [Fact]
    public void Should_Return_NewAccountsDTO_When_NewOpenAccounts()
    {
        // Arrange
        var expectedId = "id1";
        var expectedIdentification = new PiOnboardServiceApplicationModelsOpenAccountIdentification("citizenId",
            "title", "firstNameTh", "lastNameTh", "firstNameEn", "lastNameEn",
            "dateOfBirth", "idCardExpiryDate", "laserCode");
        var expectedDocuments = new List<PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto>
        {
            new(PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.CitizenCard, "url1",
                "test1.jpg"),
            new(PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.BookBank, "url2",
                "test2.jpg"),
            new(PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.Statement, "url3",
            "test3.jpg"),
            new(PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.CitizenCardPortrait, "url4",
            "test4.jpg"),
            new(PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.Selfie, "url5",
            "test5.jpg"),
            new(PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.FatcaCrs, "url6",
            "test6.jpg"),
            new(PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.Signature, "url7",
            "test7.jpg"),
            new(PiClientUserServiceModelPiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.NameChange, "url8",
                "test8.jpg"),
        };
        var expectedStatus = "PENDING";
        var expectedDate = "24/09/2023";
        var expectedBpmReceived = true;

        var accounts = new List<PiOnboardServiceAPIModelsOpenAccountOpenAccountInfoDto>
        {
            new()
            {
                Id = expectedId,
                Identification = expectedIdentification,
                Documents = expectedDocuments,
                Status = expectedStatus,
                CreatedDate = expectedDate,
                UpdatedDate = expectedDate,
                BpmReceived = expectedBpmReceived
            }
        };


        // Act
        var actual = EntityFactory.NewOpenAccounts(accounts);

        // Assert
        Assert.Equal(accounts[0].Id, actual[0].Id);
        Assert.Equal(accounts[0].Status, actual[0].Status);
        Assert.Equal(accounts[0].CreatedDate, actual[0].CreatedDate);
        Assert.Equal(accounts[0].UpdatedDate, actual[0].UpdatedDate);
        Assert.Equal(accounts[0].BpmReceived, actual[0].BpmReceived);


        //identification
        var identity = actual[0].Identification;
        if (identity != null)
        {
            Assert.Equal(accounts[0].Identification.CitizenId, identity.CitizenId);
            Assert.Equal(accounts[0].Identification.Title, identity.Title);
            Assert.Equal(accounts[0].Identification.FirstNameTh, identity.FirstNameTh);
            Assert.Equal(accounts[0].Identification.LastNameTh, identity.LastNameTh);
            Assert.Equal(accounts[0].Identification.FirstNameEn, identity.FirstNameEn);

            Assert.Equal(accounts[0].Identification.LastNameEn, identity.LastNameEn);
            Assert.Equal(accounts[0].Identification.DateOfBirth, identity.DateOfBirth);
            Assert.Equal(accounts[0].Identification.IdCardExpiryDate, identity.IdCardExpiryDate);
            Assert.Equal(accounts[0].Identification.LaserCode, identity.LaserCode);
        }
        else
        {
            Assert.Fail("Identity should not be empty");
        }

        //documents
        var documents = actual[0].Documents;
        if (documents != null)
        {
            Assert.Equal(documents[0].Url, documents[0].Url);
            Assert.Equal(documents[0].DocumentType, documents[0].DocumentType);

            Assert.Equal(documents[1].Url, documents[1].Url);
            Assert.Equal(documents[1].DocumentType, documents[1].DocumentType);
        }
        else
        {
            Assert.Fail("Document should not be empty");
        }
    }

    [Fact]
    public void Should_Return_NewUserDTO_When_NewUser_Is_Called()
    {
        // Arrange
        var id = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        var customerCode = "12345";
        var userModel = new PiUserApplicationModelsUser
        {
            Id = id,
            Devices = new List<PiUserApplicationModelsDevice>
            {
                new(deviceId, "deviceToken", "deviceIdntity", "en", "platform",
                    new PiUserApplicationModelsNotificationPreference { Wallet = true }),
                new(deviceId)
            },
            CustomerCodes = new List<PiUserApplicationModelsCustomerCode>
            {
                new(customerCode)
            },
            TradingAccounts = new List<PiUserDomainAggregatesModelUserInfoAggregateTradingAccount>
            {
                new("11111111")
            },
            FirstnameTh = "firstNameTh",
            LastnameTh = "LastNameTh",
            FirstnameEn = "English First",
            LastnameEn = "English Last",
            PhoneNumber = "0413111111",
            GlobalAccount = "Account1",
            Email = "test@test.com",
            CustomerId = "12345"
        };

        // Act
        var actual = EntityFactory.NewUser(userModel);

        // Assert
        Assert.Equal(userModel.Id.ToString(), actual.Id);
        Assert.Equal(userModel.FirstnameTh, actual.FirstnameTh);
        Assert.Equal(userModel.LastnameTh, actual.LastnameTh);
        Assert.Equal(userModel.FirstnameEn, actual.FirstnameEn);
        Assert.Equal(userModel.LastnameEn, actual.LastnameEn);

        Assert.Equal(userModel.PhoneNumber, actual.PhoneNumber);
        Assert.Equal(userModel.GlobalAccount, actual.GlobalAccount);
        Assert.Equal(userModel.Email, actual.Email);

        Assert.Equal(userModel.Devices[0].DeviceId, actual.Devices[0].DeviceId);
        Assert.True(userModel.Devices[0].NotificationPreference.Wallet);
        Assert.Null(userModel.Devices[1].NotificationPreference);
        Assert.Equal(userModel.CustomerCodes[0].Code, actual.CustomerCodes[0].Code);
        Assert.Equal(userModel.TradingAccounts[0].TradingAccountId, actual.TradingAccounts[0].Account);
    }

    [Fact]
    public void Should_Return_TransactionV2_When_NewTransactionV2_Is_Called()
    {
        // Arrange
        var transactionDto = new PiWalletServiceAPIModelsTransactionDetailsDto
        {
            CurrentState = "currentState",
            Status = PiWalletServiceDomainAggregatesModelTransactionAggregateStatus.Pending,
            TransactionNo = "transactionNo",
            TransactionType = PiWalletServiceIntegrationEventsAggregatesModelTransactionType.Deposit,
            Product = PiWalletServiceIntegrationEventsAggregatesModelProduct.Cash,
            Channel = PiWalletServiceIntegrationEventsAggregatesModelChannel.QR,
            AccountCode = "accountCode",
            CustomerCode = "customerCode",
            CustomerName = "customerName",
            BankAccountNo = "bankAccountNo",
            BankAccountName = "bankAccountName",
            BankName = "bankName",
            RequestedAmount = "100",
            RequestedCurrency = PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency.THB,
            ToCurrency = PiWalletServiceDomainAggregatesModelGlobalWalletAggregateCurrency.USD,
            TransferAmount = "200",
            Fee = "10",
            GlobalTransfer = new PiWalletServiceDomainAggregatesModelGlobalTransferGlobalTransferState
            {
                CurrentState = "globalTransferState",
                GlobalAccount = "globalAccount"
            },
            QrDeposit = new PiWalletServiceDomainAggregatesModelQrDepositAggregateQrDepositState
            {
                CurrentState = "currentState",
                DepositQrGenerateDateTime = DateTime.Now,
                QrTransactionNo = "qrTransactionNo",
                QrTransactionRef = "qrTransactionRef",
                QrValue = "qrValue",
                PaymentReceivedAmount = 100,
                PaymentReceivedDateTime = DateTime.Now,
                Fee = 10,
                FailedReason = "failedReason",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            OddDeposit = new PiWalletServiceDomainAggregatesModelOddDepositAggregateOddDepositState
            {
                CurrentState = "currentState",
            },
            OddWithdraw = new PiWalletServiceDomainAggregatesModelOddWithdrawAggregateOddWithdrawState
            {
                CurrentState = "currentState"
            },
            AtsWithdraw = new PiWalletServiceDomainAggregatesModelAtsWithdrawAggregateAtsWithdrawState
            {
                CurrentState = "currentState",
            },
            AtsDeposit = new PiWalletServiceDomainAggregatesModelAtsDepositAggregateAtsDepositState
            {
                CurrentState = "currentState",
            },
            UpBack = new PiWalletServiceDomainAggregatesModelUpBackAggregateUpBackState
            {
                CurrentState = "currentState",
            },
            Recovery = new PiWalletServiceDomainAggregatesModelRecoveryAggregateRecoveryState
            {
                CurrentState = "currentState",
            },
            RefundInfo = new PiWalletServiceDomainAggregatesModelRefundInfoAggregateRefundInfo
            {
                CreatedAt = DateTime.Now,
            },
            PaymentAt = DateTime.Now,
            CreatedAt = DateTime.Now
        };

        // Act
        var actual = EntityFactory.NewTransactionV2(transactionDto);

        // Assert
        Assert.IsType<TransactionV2>(actual);
        Assert.Equal(transactionDto.CurrentState, actual.State);
        Assert.Equal(transactionDto.Status.ToString(), actual.Status);
        Assert.Equal(transactionDto.TransactionNo, actual.TransactionNo);
        Assert.Equal(transactionDto.AccountCode, actual.AccountCode);
        Assert.Equal(transactionDto.CustomerCode, actual.CustomerCode);
        Assert.Equal(transactionDto.CustomerName, actual.CustomerName);
        Assert.Equal(transactionDto.BankAccountNo, actual.BankAccountNo);
        Assert.Equal(transactionDto.BankAccountName, actual.BankAccountName);
        Assert.Equal(transactionDto.BankName, actual.BankName);
        Assert.Equal(transactionDto.RequestedAmount, actual.RequestedAmount);
        Assert.Equal(transactionDto.Fee, actual.Fee);
        Assert.Equal(DateOnly.FromDateTime(transactionDto.PaymentAt), actual.EffectiveDate);
        Assert.Equal(transactionDto.CreatedAt, actual.CreatedAt);
        Assert.Equal(transactionDto.GlobalTransfer.CurrentState, actual.GlobalTransfer?.State);
        Assert.Equal(transactionDto.GlobalTransfer.GlobalAccount, actual.GlobalTransfer?.GlobalAccount);
        var qrDepositStateDto = transactionDto.QrDeposit;
        Assert.Equal(qrDepositStateDto.CurrentState, actual.QrDeposit?.State);
        Assert.Equal(qrDepositStateDto.DepositQrGenerateDateTime, actual.QrDeposit?.DepositQrGenerateDateTime);
        Assert.Equal(qrDepositStateDto.QrTransactionNo, actual.QrDeposit?.QrTransactionNo);
        Assert.Equal(qrDepositStateDto.QrTransactionRef, actual.QrDeposit?.QrTransactionRef);
        Assert.Equal(qrDepositStateDto.QrValue, actual.QrDeposit?.QrValue);
        Assert.Equal(qrDepositStateDto.PaymentReceivedAmount, actual.QrDeposit?.PaymentReceivedAmount);
        Assert.Equal(qrDepositStateDto.PaymentReceivedDateTime, actual.QrDeposit?.PaymentReceivedDateTime);
        Assert.Equal(qrDepositStateDto.Fee, actual.QrDeposit?.Fee);
        Assert.Equal(qrDepositStateDto.FailedReason, actual.QrDeposit?.FailedReason);
        Assert.Equal(qrDepositStateDto.CreatedAt, actual.QrDeposit?.CreatedAt);
        Assert.Equal(qrDepositStateDto.UpdatedAt, actual.QrDeposit?.UpdatedAt);
        Assert.Equal(transactionDto.UpBack.CurrentState, actual.UpBack?.State);
        Assert.Equal(transactionDto.Recovery.CurrentState, actual.Recovery?.State);
        Assert.Equal(transactionDto.RefundInfo.CreatedAt, actual.Refund?.CreatedAt);
        Assert.Equal(transactionDto.OddDeposit.CurrentState, actual.OddDeposit?.State);
        Assert.Equal(transactionDto.AtsDeposit.CurrentState, actual.AtsDeposit?.State);
        Assert.Equal(transactionDto.OddWithdraw.CurrentState, actual.OddWithdraw?.State);
        Assert.Equal(transactionDto.AtsWithdraw.CurrentState, actual.AtsWithdraw?.State);
    }
}
