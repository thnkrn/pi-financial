using Pi.User.Application.Commands;
using Microsoft.Extensions.DependencyInjection;
using MassTransit;
using Moq;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;
using Pi.Common.SeedWork;
using FluentAssertions;

namespace Pi.User.Application.Tests.Commands;

public class SubmitDocumentTests : ConsumerTest
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IDocumentRepository> _mockDocumentRepository = new();

    private readonly SubmitDocument _bookbankSubmitDocument =
        new(DocumentType.BookBank, "bookbankFileUrl", "bookbankFileName");

    private readonly SubmitDocument _statementSubmitDocument =
        new(DocumentType.Statement, "statementFileUrl", "statementFileName");

    private readonly SubmitDocument _citizenCardSubmitDocument =
        new(DocumentType.CitizenCard, "citizenCardFileUrl", "citizenCardFileName");

    private readonly SubmitDocument _citizenCardPortraitSubmitDocument =
        new(DocumentType.CitizenCardPortrait, "citizenCardPortraitFileUrl", "citizenCardPortraitFileName");

    private readonly SubmitDocument _selfieSubmitDocument =
        new(DocumentType.Selfie, "selfieFileUrl", "selfieFileName");

    private readonly SubmitDocument _factaCrsSubmitDocument =
        new(DocumentType.FatcaCrs, "factaCrsFileUrl", "factaCrsFileName");

    private readonly SubmitDocument _signatureSubmitDocument =
        new(DocumentType.Signature, "signatureFileUrl", "signatureFileName");

    private readonly SubmitDocument _nameChangeSubmitDocument =
        new(DocumentType.NameChange, "nameChangeFileUrl", "nameChangeFileName");

    public SubmitDocumentTests()
    {
        _mockDocumentRepository.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<SubmitDocumentConsumer>();
            })
            .AddScoped(_ => _mockDocumentRepository.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async Task SubmitDocumentRequestConsumer_WhenRequestToCreateMultipleDocuments_ShouldCreateTheCorrectNumberOfDocuments()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var documents = new List<SubmitDocument>()
        {
            _bookbankSubmitDocument, _statementSubmitDocument, _citizenCardSubmitDocument,
            _citizenCardPortraitSubmitDocument, _selfieSubmitDocument, _factaCrsSubmitDocument,
            _signatureSubmitDocument, _nameChangeSubmitDocument
        };
        var submitDocumentRequest = new SubmitDocumentRequest(userId, documents);

        var expectedTimesDocumentIsCreated = documents.Count();

        // Original event is published.
        var expectedAllPublishedEvents = new List<Type>() { typeof(SubmitDocumentRequest) };

        // Consumer is expected to succeed, therefore it must successfully consume the event.
        var expectedAllConsumedEvents = new List<Type>() { typeof(SubmitDocumentRequest) };
        var expectedAllConsumedMessages = new List<SubmitDocumentRequest>() { submitDocumentRequest };
        var expectedSubmitDocumentConsumedMessages = new List<SubmitDocumentRequest>() { submitDocumentRequest };

        // Consumer is expected to succeed, therefore it must not throw any error.
        var expectedAllConsumerExceptions = new List<ValueTuple<Type, string>>() { };
        var expectedSubmitDocumentExceptions = new List<ValueTuple<Type, string>>() { };

        // Act
        await Harness.Bus.Publish(submitDocumentRequest);

        // Assert
        var submitDocumentConsumerHarness = Harness.GetConsumerHarness<SubmitDocumentConsumer>();

        var isPublished = await Harness.Published.Any<SubmitDocumentRequest>();
        var isConsumed = await Harness.Consumed.Any<SubmitDocumentRequest>();

        var actualAllPublishedEvents = Harness.Published
            .Select(q => q.Exception == null)
            .Select(q => q.MessageType).ToList();

        var actualAllConsumedEvents = Harness.Consumed
            .Select(q => q.Exception == null)
            .Select(q => q.MessageType).ToList();
        var actualAllConsumedMessages = Harness.Consumed
            .Select(q => q.Exception == null)
            .Select(q => q.MessageObject).ToList();
        var actualAllConsumerExceptions = Harness.Consumed
            .Select(q => q.Exception != null)
            .Select(q => new ValueTuple<Type, string>(q.Exception.GetType(), q.Exception.Message)).ToList();

        var actualSubmitDocumentConsumedMessages = submitDocumentConsumerHarness.Consumed
            .Select(q => q.Exception == null)
            .Select(q => q.MessageObject).ToList();
        var actualSubmitDocumentExceptions = submitDocumentConsumerHarness.Consumed
            .Select(q => q.Exception != null)
            .Select(q => new ValueTuple<Type, string>(q.Exception.GetType(), q.Exception.Message)).ToList();

        // WARNING: In debug mode, putting a breakpoint before this line will result in the consumer not being called. 

        Assert.True(isPublished);
        Assert.True(isConsumed);

        actualAllPublishedEvents.Should().BeEquivalentTo(expectedAllPublishedEvents);

        actualAllConsumedEvents.Should().BeEquivalentTo(expectedAllConsumedEvents);
        actualAllConsumedMessages.Should().BeEquivalentTo(expectedAllConsumedMessages);
        actualAllConsumerExceptions.Should().BeEquivalentTo(expectedAllConsumerExceptions);

        actualSubmitDocumentConsumedMessages.Should().BeEquivalentTo(expectedSubmitDocumentConsumedMessages);
        actualSubmitDocumentExceptions.Should().BeEquivalentTo(expectedSubmitDocumentExceptions);

        _mockDocumentRepository
            .Verify(d =>
                d.CreateDocument(It.IsAny<Document>()), Times.Exactly(expectedTimesDocumentIsCreated));
    }

    [Fact]
    public async Task SubmitDocumentRequestConsumer_WhenFailedToCreateMultipleDocuments_ShouldThrowError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var unableToSaveDocumentException = new ValueTuple<Type, string>(typeof(Exception), "Unable to save document");
        var documents = new List<SubmitDocument>()
        {
            _bookbankSubmitDocument, _statementSubmitDocument, _citizenCardSubmitDocument,
            _citizenCardPortraitSubmitDocument, _selfieSubmitDocument, _factaCrsSubmitDocument,
            _signatureSubmitDocument, _nameChangeSubmitDocument
        };
        var submitDocumentRequest = new SubmitDocumentRequest(userId, documents);

        // Original event is published. When the consumer fails, a FaulEvent will also be published.
        var expectedAllPublishedEvents = new List<Type>()
            { typeof(SubmitDocumentRequest), typeof(Fault<SubmitDocumentRequest>) };

        // Consumer is expected to fail, therefore it doesn't successfully consume any events.
        var expectedAllConsumedEvents = new List<Type>() { };
        var expectedAllConsumedMessages = new List<SubmitDocumentRequest>() { };
        var expectedSubmitDocumentConsumedMessages = new List<SubmitDocumentRequest>() { };

        // Consumer is expected to fail because an exception is thrown.
        var expectedAllConsumerExceptions = new List<ValueTuple<Type, string>>() { unableToSaveDocumentException };
        var expectedSubmitDocumentExceptions = new List<ValueTuple<Type, string>>() { unableToSaveDocumentException };

        _mockDocumentRepository
            .Setup(d => d.CreateDocument(It.IsAny<Document>()))
            .Throws(new Exception("some error"));

        // Act
        await Harness.Bus.Publish(submitDocumentRequest);

        // Assert
        var submitDocumentConsumerHarness = Harness.GetConsumerHarness<SubmitDocumentConsumer>();

        var isPublished = await Harness.Published.Any<SubmitDocumentRequest>();
        var isConsumed = await Harness.Consumed.Any<SubmitDocumentRequest>();

        var actualAllPublishedEvents = Harness.Published
            .Select(q => q.Exception == null)
            .Select(q => q.MessageType).ToList();

        var actualAllConsumedEvents = Harness.Consumed
            .Select(q => q.Exception == null)
            .Select(q => q.MessageType).ToList();
        var actualAllConsumedMessages = Harness.Consumed
            .Select(q => q.Exception == null)
            .Select(q => q.MessageObject).ToList();
        var actualAllConsumerExceptions = Harness.Consumed
            .Select(q => q.Exception != null)
            .Select(q => new ValueTuple<Type, string>(q.Exception.GetType(), q.Exception.Message)).ToList();

        var actualSubmitDocumentConsumedMessages = submitDocumentConsumerHarness.Consumed
            .Select(q => q.Exception == null)
            .Select(q => q.MessageObject).ToList();
        var actualSubmitDocumentExceptions = submitDocumentConsumerHarness.Consumed
            .Select(q => q.Exception != null)
            .Select(q => new ValueTuple<Type, string>(q.Exception.GetType(), q.Exception.Message)).ToList();

        // WARNING: In debug mode, putting a breakpoint before this line will result in the consumer not being called.

        Assert.True(isPublished);
        Assert.True(isConsumed);

        actualAllPublishedEvents.Should().BeEquivalentTo(expectedAllPublishedEvents);

        actualAllConsumedEvents.Should().BeEquivalentTo(expectedAllConsumedEvents);
        actualAllConsumedMessages.Should().BeEquivalentTo(expectedAllConsumedMessages);
        actualAllConsumerExceptions.Should().BeEquivalentTo(expectedAllConsumerExceptions);

        actualSubmitDocumentConsumedMessages.Should().BeEquivalentTo(expectedSubmitDocumentConsumedMessages);
        actualSubmitDocumentExceptions.Should().BeEquivalentTo(expectedSubmitDocumentExceptions);

        // Error happens when this method is called, therefore it must be called only once.
        _mockDocumentRepository
            .Verify(d => d.CreateDocument(It.IsAny<Document>()), Times.Exactly(1));
    }
}