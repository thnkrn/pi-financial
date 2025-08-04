// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Pi.Client.OnboardService.Model;
using Pi.Client.UserService.Model;
using Pi.Common.Features;
using Pi.Financial.FundService.Application.Commands;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AccountOpening.Events.Models;

namespace Pi.Financial.FundService.Application.Tests.Commands;

public class UploadFundAccountOpeningDocumentsTests
{
    private readonly Mock<ILogger<UploadFundAccountOpeningDocumentsConsumer>> _mockLogger = new();
    private readonly Mock<IFundConnextService> _mockFundConnextService = new();
    private readonly Mock<ICustomerService> _mockCustomerService = new();
    private readonly Mock<IFeatureService> _mockFeatureService = new();
    private readonly Mock<IUserService> _mockUserService = new();
    private readonly Mock<IOnboardService> _mockOnboardService = new();
    private readonly UploadFundAccountOpeningDocumentsConsumer _consumer;
    private readonly Mock<ConsumeContext<UploadFundAccountOpeningDocuments>> _consumeContextMock = new();

    public UploadFundAccountOpeningDocumentsTests()
    {
        _mockFeatureService.Setup(fs => fs.IsOn(Features.UseOnboardUserDocument)).Returns(false);

        _consumer = new UploadFundAccountOpeningDocumentsConsumer
        (
            _mockLogger.Object,
            _mockFundConnextService.Object,
            _mockCustomerService.Object,
            _mockFeatureService.Object,
            _mockUserService.Object,
            _mockOnboardService.Object
        );
    }

    [Fact]
    public async Task AddDocumentFromUserService_WhenAddingDocumentsThatInclude_MultipleIdCards_And1Selfie_ShouldUploadOnlyForUniqueAndAllowedDocumentType_AndReturn1CitizenCardDocumentUpload()
    {
        //Arrange
        var mockData1 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl1",
            "fileName1",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.CitizenCard);
        var mockData2 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl2",
            "fileName2",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.Selfie);
        var mockData3 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl3",
            "fileName3",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.CitizenCard);

        List<PiUserApplicationModelsDocumentDocumentDto> mockList = [mockData1, mockData2, mockData3];
        _mockUserService
            .Setup(s => s.GetDocumentByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(mockList);

        //Act
        List<Document> documentList = new();
        await _consumer.AddDocumentFromUserService(Guid.NewGuid(), documentList);

        //Assert
        Assert.Equivalent("fileUrl1", documentList[0].PreSignedUrl);
        Assert.Equivalent(1, documentList.Count);
        Assert.Equivalent("IdCard", documentList[0].DocumentType.ToDescriptionString());
    }

    [Fact]
    public async Task AddDocumentFromUserService_WhenAddingDocumentsThatInclude_MultipleSignatures_ShouldUploadOnlyForUniqueDocumentTypes_AndReturn1DocumentsThatWereUploadForEachUniqueDocumentType()
    {
        //Arrange
        var mockData1 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl1",
            "fileName1",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.Signature);
        var mockData2 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl2",
            "fileName2",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.Signature);
        var mockData3 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl3",
            "fileName3",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.Statement);
        var mockData4 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl4",
            "fileName4",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.Signature);

        List<PiUserApplicationModelsDocumentDocumentDto> mockList = [mockData1, mockData2, mockData3, mockData4];
        _mockUserService
            .Setup(s => s.GetDocumentByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(mockList);

        //Act
        List<Document> documentList = new();
        await _consumer.AddDocumentFromUserService(Guid.NewGuid(), documentList);

        //Assert
        Assert.Equivalent("fileUrl1", documentList[0].PreSignedUrl);
        Assert.Equivalent(1, documentList.Count);
        Assert.Equivalent("Others", documentList[0].DocumentType.ToDescriptionString());
    }

    [Fact]
    public async Task AddDocumentFromUserService_WhenAddingDocumentsThatInclude_2IdCard_And2Signature_ShouldUploadOnlyForUniqueDocumentTypes_AndReturn2DocumentsThatWereUploadForEachUniqueDocumentType()
    {
        //Arrange
        var mockData1 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl1",
            "fileName1",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.Signature);
        var mockData2 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl2",
            "fileName2",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.CitizenCard);
        var mockData3 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl3",
            "fileName3",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.Signature);
        var mockData4 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl4",
            "fileName4",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.CitizenCard);

        List<PiUserApplicationModelsDocumentDocumentDto> mockList = [mockData1, mockData2, mockData3, mockData4];
        _mockUserService
            .Setup(s => s.GetDocumentByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(mockList);

        //Act
        List<Document> documentList = new();
        await _consumer.AddDocumentFromUserService(Guid.NewGuid(), documentList);

        //Assert
        Assert.Equivalent("fileUrl1", documentList[0].PreSignedUrl);
        Assert.Equivalent("fileUrl2", documentList[1].PreSignedUrl);
        Assert.Equivalent(2, documentList.Count);
        Assert.Equivalent("Others", documentList[0].DocumentType.ToDescriptionString());
        Assert.Equivalent("IdCard", documentList[1].DocumentType.ToDescriptionString());
    }

    [Fact]
    public async Task AddDocumentFromUserService_WhenUseOnboardUserDocument_AndAddingDocumentsThatInclude_MultipleIdCards_And1Selfie_ShouldUploadOnlyForUniqueAndAllowedDocumentType_AndReturn1CitizenCardDocumentUpload()
    {
        //Arrange
        _mockFeatureService.Setup(fs => fs.IsOn(Features.UseOnboardUserDocument)).Returns(true);

        var mockData1 = new PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto(
            "fileUrl1",
            "fileName1",
            PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.CitizenCard);
        var mockData2 = new PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto(
            "fileUrl2",
            "fileName2",
            PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.Selfie);
        var mockData3 = new PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto(
            "fileUrl3",
            "fileName3",
            PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.CitizenCard);

        List<PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto> mockList = new() { mockData1, mockData2, mockData3 };
        _mockOnboardService
            .Setup(s => s.GetDocumentByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockList);

        //Act
        List<Document> documentList = new();
        await _consumer.AddDocumentFromUserService(Guid.NewGuid(), documentList);

        //Assert
        Assert.Equivalent("fileUrl1", documentList[0].PreSignedUrl);
        Assert.Equivalent(1, documentList.Count);
        Assert.Equivalent("IdCard", documentList[0].DocumentType.ToDescriptionString());
    }

    [Fact]
    public async Task AddDocumentFromUserService_WhenUseOnboardUserDocument_AndAddingDocumentsThatInclude_MultipleSignatures_ShouldUploadOnlyForUniqueDocumentTypes_AndReturn1DocumentsThatWereUploadForEachUniqueDocumentType()
    {
        //Arrange
        _mockFeatureService.Setup(fs => fs.IsOn(Features.UseOnboardUserDocument)).Returns(true);

        var mockData1 = new PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto(
            "fileUrl1",
            "fileName1",
            PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.Signature);
        var mockData2 = new PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto(
            "fileUrl2",
            "fileName2",
            PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.Signature);
        var mockData3 = new PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto(
            "fileUrl3",
            "fileName3",
            PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.Statement);
        var mockData4 = new PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto(
            "fileUrl4",
            "fileName4",
            PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.Signature);

        List<PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto> mockList = new() { mockData1, mockData2, mockData3, mockData4 };
        _mockOnboardService
            .Setup(s => s.GetDocumentByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockList);

        //Act
        List<Document> documentList = new();
        await _consumer.AddDocumentFromUserService(Guid.NewGuid(), documentList);

        //Assert
        Assert.Equivalent("fileUrl1", documentList[0].PreSignedUrl);
        Assert.Equivalent(1, documentList.Count);
        Assert.Equivalent("Others", documentList[0].DocumentType.ToDescriptionString());
    }

    [Fact]
    public async Task AddDocumentFromUserService_WhenUseOnboardUserDocument_AndAddingDocumentsThatInclude_2IdCard_And2Signature_ShouldUploadOnlyForUniqueDocumentTypes_AndReturn2DocumentsThatWereUploadForEachUniqueDocumentType()
    {
        //Arrange
        _mockFeatureService.Setup(fs => fs.IsOn(Features.UseOnboardUserDocument)).Returns(true);

        var mockData1 = new PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto(
            "fileUrl1",
            "fileName1",
            PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.Signature);
        var mockData2 = new PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto(
            "fileUrl2",
            "fileName2",
            PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.CitizenCard);
        var mockData3 = new PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto(
            "fileUrl3",
            "fileName3",
            PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.Signature);
        var mockData4 = new PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto(
            "fileUrl4",
            "fileName4",
            PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto.DocumentTypeEnum.CitizenCard);

        List<PiOnboardServiceApplicationModelsUserDocumentUserDocumentDto> mockList = new() { mockData1, mockData2, mockData3, mockData4 };
        _mockOnboardService
            .Setup(s => s.GetDocumentByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockList);

        //Act
        List<Document> documentList = new();
        await _consumer.AddDocumentFromUserService(Guid.NewGuid(), documentList);

        //Assert
        Assert.Equivalent("fileUrl1", documentList[0].PreSignedUrl);
        Assert.Equivalent("fileUrl2", documentList[1].PreSignedUrl);
        Assert.Equivalent(2, documentList.Count);
        Assert.Equivalent("Others", documentList[0].DocumentType.ToDescriptionString());
        Assert.Equivalent("IdCard", documentList[1].DocumentType.ToDescriptionString());
    }

    [Fact]
    public async Task Consume_WhenAdding_EmptyDocument_ShouldReturn0DocumentThatWereAdded()
    {
        //Arrange
        _mockUserService
            .Setup(s => s.GetDocumentByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([]);

        //Act
        List<Document> documentList = [];
        await _consumer.AddDocumentFromUserService(Guid.NewGuid(), documentList);

        //Assert
        Assert.Empty(documentList);
    }

    [Fact]
    public async Task HandleSegregateAccount_WhenAddingDocumentsThatInclude_1Facta_1SuittabilityForm_1FundOpenAccountForm_2IdCardForm_AndFeatureSwitchIsOn_ShouldTriggerUploadIndividualCustomerDocuments6Times()
    {
        //Arrange
        var documents = new List<Document>()
        {
            new(DocumentType.FundOpenAccountForm, "url"),
            new(DocumentType.SuitabilityForm, "url"),
            new(DocumentType.Fatca, "url"),
            new(DocumentType.IdCardForm, "url"),
            new(DocumentType.IdCardForm, "url"),
            new(DocumentType.Signature, "url"),
            new(DocumentType.BankAccount, "url"),
            new(DocumentType.AccountInformation, "url"),
            new(DocumentType.Amendment, "url"),
            new(DocumentType.AttorneyForm, "url")
        };
        var uploadFundAccountOpeningDocuments = new UploadFundAccountOpeningDocuments
        (
            Guid.NewGuid(),
            Guid.NewGuid(),
            "custCode",
            documents,
            true,
            "ndidRequestId",
            true
        );
        _consumeContextMock.Setup(m => m.Message).Returns(uploadFundAccountOpeningDocuments);
        _mockUserService
            .Setup(s => s.GetDocumentByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([]);
        _mockFeatureService.Setup(f => f.IsOn(Features.UploadFileSegregate)).Returns(true);
        _mockFeatureService.Setup(f => f.IsOn(Features.UploadSignatureIdCardToFundConnext)).Returns(true);

        //Act
        await _consumer.HandleSegregateAccount
        (
            _consumeContextMock.Object.Message.Documents,
            _consumeContextMock.Object.Message.UserId,
            _consumeContextMock.Object.Message.IsOpenSegregateAccount,
            _consumeContextMock.Object.Message.TicketId.ToString(),
            IdentificationCardType.Citizen,
            "cardNumber",
            new List<string>()
        );

        //Assert
        _mockFundConnextService
            .Verify(x => x.UploadIndividualCustomerDocuments
                (
                    It.IsAny<string>(),
                    It.IsAny<IdentificationCardType>(),
                    It.IsAny<string>(),
                    It.IsAny<Document>()
                )
                , Times.Exactly(6));
    }

    [Fact]
    public async Task HandleSegregateAccount_WhenAddingDocumentsThatInclude_1Facta_1SuittabilityForm_1FundOpenAccountForm_2IdCardForm_AndFeatureSwitchIsOff_ShouldTriggerUploadIndividualCustomerDocuments5Times()
    {
        //Arrange
        var documents = new List<Document>()
        {
            new(DocumentType.FundOpenAccountForm, "url"),
            new(DocumentType.SuitabilityForm, "url"),
            new(DocumentType.Fatca, "url"),
            new(DocumentType.IdCardForm, "url"),
            new(DocumentType.IdCardForm, "url"),
            new(DocumentType.Signature, "url"),
            new(DocumentType.BankAccount, "url"),
            new(DocumentType.AccountInformation, "url"),
            new(DocumentType.Amendment, "url"),
            new(DocumentType.AttorneyForm, "url")
        };
        var uploadFundAccountOpeningDocuments = new UploadFundAccountOpeningDocuments
        (
            Guid.NewGuid(),
            Guid.NewGuid(),
            "custCode",
            documents,
            true,
            "ndidRequestId",
            true
        );
        _consumeContextMock.Setup(m => m.Message).Returns(uploadFundAccountOpeningDocuments);
        _mockFeatureService.Setup(f => f.IsOn(Features.UploadFileSegregate)).Returns(true);
        _mockFeatureService.Setup(f => f.IsOn(Features.UploadSignatureIdCardToFundConnext)).Returns(false);

        //Act
        await _consumer.HandleSegregateAccount
        (
            _consumeContextMock.Object.Message.Documents,
            _consumeContextMock.Object.Message.UserId,
            _consumeContextMock.Object.Message.IsOpenSegregateAccount,
            _consumeContextMock.Object.Message.TicketId.ToString(),
            IdentificationCardType.Citizen,
            "cardNumber",
            new List<string>()
        );

        //Assert
        _mockFundConnextService
            .Verify(x => x.UploadIndividualCustomerDocuments
                (
                    It.IsAny<string>(),
                    It.IsAny<IdentificationCardType>(),
                    It.IsAny<string>(),
                    It.IsAny<Document>()
                )
                , Times.Exactly(5));
    }

    [Fact]
    public async Task HandleUploadIndividualAccountDocuments_WhenAddingDocumentsThatInclude_1FundOpenAccountForm_2IdCardForm_1BankAccount_1SuitabilityForm_1Fatca_1Signature_AndFeatureSwitchIsOn_ShouldTriggerUploadIndividualCustomerDocuments6Times()
    {
        //Arrange
        var documents = new List<Document>() { new(DocumentType.FundOpenAccountForm, "url"), new(DocumentType.IdCardForm, "url"), new(DocumentType.SuitabilityForm, "url"), new(DocumentType.Fatca, "url") };

        var userServiceDoc = new List<PiUserApplicationModelsDocumentDocumentDto>() { new("url", "name", PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.Signature), new("url", "name", PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.CitizenCard) };
        var uploadFundAccountOpeningDocuments = new UploadFundAccountOpeningDocuments
        (
            Guid.NewGuid(),
            Guid.NewGuid(),
            "custCode",
            documents,
            true,
            "ndidRequestId",
            true
        );
        _mockUserService.Setup(m => m.GetDocumentByUserId(It.IsAny<Guid>())).ReturnsAsync(userServiceDoc);
        _consumeContextMock.Setup(m => m.Message).Returns(uploadFundAccountOpeningDocuments);
        _mockFeatureService.Setup(f => f.IsOn(Features.UploadSignatureIdCardToFundConnext)).Returns(true);

        //Act
        await _consumer.HandleUploadIndividualAccountDocuments
        (
            _consumeContextMock.Object.Message.Documents.ToList(),
            _consumeContextMock.Object.Message.UserId,
            _consumeContextMock.Object.Message.TicketId.ToString(),
            IdentificationCardType.Citizen,
            "cardNumber",
            "accountId",
            new List<string>()
        );

        //Assert
        _mockFundConnextService
            .Verify(x => x.UploadIndividualAccountDocuments
                (
                    It.IsAny<string>(),
                    It.IsAny<IdentificationCardType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Document>()
                )
                , Times.Exactly(6));
    }

    [Fact]
    public async Task HandleUploadIndividualAccountDocuments_WhenAddingDocumentsThatInclude_1FundOpenAccountForm_2IdCardForm_1BankAccount_1SuitabilityForm_1Fatca_1Signature_AndFeatureSwitchIsOff_ShouldTriggerUploadIndividualCustomerDocuments4Times()
    {
        //Arrange
        var documents = new List<Document>() { new(DocumentType.FundOpenAccountForm, "url"), new(DocumentType.IdCardForm, "url"), new(DocumentType.SuitabilityForm, "url"), new(DocumentType.Fatca, "url") };

        var userServiceDoc = new List<PiUserApplicationModelsDocumentDocumentDto>() { new("url", "name", PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.Signature), new("url", "name", PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.CitizenCard) };
        var uploadFundAccountOpeningDocuments = new UploadFundAccountOpeningDocuments
        (
            Guid.NewGuid(),
            Guid.NewGuid(),
            "custCode",
            documents,
            true,
            "ndidRequestId",
            true
        );
        _mockUserService.Setup(m => m.GetDocumentByUserId(It.IsAny<Guid>())).ReturnsAsync(userServiceDoc);
        _consumeContextMock.Setup(m => m.Message).Returns(uploadFundAccountOpeningDocuments);
        _mockFeatureService.Setup(f => f.IsOn(Features.UploadSignatureIdCardToFundConnext)).Returns(false);

        //Act
        await _consumer.HandleUploadIndividualAccountDocuments
        (
            _consumeContextMock.Object.Message.Documents.ToList(),
            _consumeContextMock.Object.Message.UserId,
            _consumeContextMock.Object.Message.TicketId.ToString(),
            IdentificationCardType.Citizen,
            "cardNumber",
            "accountId",
            new List<string>()
        );

        //Assert
        _mockFundConnextService
            .Verify(x => x.UploadIndividualAccountDocuments
                (
                    It.IsAny<string>(),
                    It.IsAny<IdentificationCardType>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<Document>()
                )
                , Times.Exactly(4));
    }

    [Fact]
    public async Task Consume_WhenAdding_1BookBank_And_1Portrait_ShouldReturn2DocumentsThatWereAdded()
    {
        //Arrange
        var mockData1 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl1",
            "fileName1",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.BookBank);
        var mockData2 = new PiUserApplicationModelsDocumentDocumentDto(
            "fileUrl2",
            "fileName2",
            PiUserApplicationModelsDocumentDocumentDto.DocumentTypeEnum.CitizenCardPortrait);

        List<PiUserApplicationModelsDocumentDocumentDto> mockList = [mockData1, mockData2];
        _mockUserService
            .Setup(s => s.GetDocumentByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(mockList);

        //Act
        List<Document> documentList = [];
        await _consumer.AddDocumentFromUserService(Guid.NewGuid(), documentList, addBookBank: true, addPortrait: true);

        //Assert
        Assert.Equivalent("fileUrl1", documentList[0].PreSignedUrl);
        Assert.Equivalent("fileUrl2", documentList[1].PreSignedUrl);
        Assert.Equivalent(2, documentList.Count);
        Assert.Equivalent("BankAccount", documentList[0].DocumentType.ToDescriptionString());
        Assert.Equivalent("IdCard", documentList[1].DocumentType.ToDescriptionString());
    }
}
