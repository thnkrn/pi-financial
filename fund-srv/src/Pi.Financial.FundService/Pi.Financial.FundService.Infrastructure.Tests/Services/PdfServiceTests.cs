// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Moq;
using Pi.Financial.Client.PdfService.Api;
using Pi.Financial.Client.PdfService.Model;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Infrastructure.Services;

namespace Pi.Financial.FundService.Infrastructure.Tests.Services;

public class PdfServiceTests
{
    private readonly Mock<IPdfServiceApi> _pdfServiceApi = new();
    private readonly Mock<ILogger<PdfService>> _logger = new();
    private readonly PdfService _pdfService;

    public PdfServiceTests()
    {
        _pdfService = new PdfService(
            _pdfServiceApi.Object,
            _logger.Object
        );
    }

    [Fact]
    public void GetBankAccountDtos_WhenNoSubscriptionButHasRedemption_ReturnSubscriptionOnly()
    {
        //Arrange
        List<BankAccount> redemptionMockList =
        [
            new BankAccount("bankCode1",
                "BankAccount1",
                "BankBranch1",
                true),

            new BankAccount("bankCode2",
                "BankAccount2",
                "BankBranch2")
        ];
        List<BankAccount> subscriptionMockList = [];

        //Act
        (List<BankAccountDto> actualSubscription, List<BankAccountDto> actualRedemption) = _pdfService.GetBankAccountDtos(
            subscriptionMockList,
            redemptionMockList,
            "firstNameTh",
            "lastNameTh"
            );

        //Assert
        Assert.Equivalent(2, actualSubscription.Count);
        Assert.Equivalent(0, actualRedemption.Count);
    }

    [Fact]
    public void GetBankAccountDtos_WhenHasSubscriptionAndRedemptionDifferently_ReturnBoth()
    {
        //Arrange
        List<BankAccount> subscriptionMockList = [
            new BankAccount("bankCode1",
                "BankAccount1",
                "BankBranch1",
                true),

            new BankAccount("bankCode2",
                "BankAccount2",
                "BankBranch2")
        ];

        List<BankAccount> redemptionMockList =
        [
            new BankAccount("bankCode3",
                "BankAccount3",
                "BankBranch3",
                true),

            new BankAccount("bankCode4",
                "BankAccount4",
                "BankBranch4")
        ];

        //Act
        (List<BankAccountDto> actualSubscription, List<BankAccountDto> actualRedemption) = _pdfService.GetBankAccountDtos(
            subscriptionMockList,
            redemptionMockList,
            "firstNameTh",
            "lastNameTh"
        );

        //Assert
        Assert.Equivalent("BankAccount1", actualSubscription[0].AccountNumber);
        Assert.Equivalent("BankAccount2", actualSubscription[1].AccountNumber);
        Assert.Equivalent("BankAccount3", actualRedemption[0].AccountNumber);
        Assert.Equivalent("BankAccount4", actualRedemption[1].AccountNumber);
    }

    [Fact]
    public void GetBankAccountDtos_WhenHas2SubscriptionAnd1RedemptionSimilarly_ReturnOnlySubscription()
    {
        //Arrange
        List<BankAccount> subscriptionMockList = [
            new BankAccount("bankCode1",
                "BankAccount1",
                "BankBranch1",
                true)
        ];

        List<BankAccount> redemptionMockList =
        [
            new BankAccount("bankCode1",
                "BankAccount1",
                "BankBranch1",
                true)
        ];

        //Act
        (List<BankAccountDto> actualSubscription, List<BankAccountDto> actualRedemption) = _pdfService.GetBankAccountDtos(
            subscriptionMockList,
            redemptionMockList,
            "firstNameTh",
            "lastNameTh"
        );

        //Assert
        Assert.Equivalent(1, actualSubscription.Count);
        Assert.Equivalent(0, actualRedemption.Count);
    }
}
