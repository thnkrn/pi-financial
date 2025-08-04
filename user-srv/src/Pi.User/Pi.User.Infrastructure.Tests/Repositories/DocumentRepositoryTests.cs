using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;
using Pi.User.Infrastructure.Repositories;

namespace Pi.User.Infrastructure.Tests.Repositories;

public class DocumentRepositoryTests
{
    private readonly Mock<ILogger<DocumentRepository>> _logger = new();

    [Fact]
    public async Task GetDocumentsByUserId_ShouldReturnAndOrderByLatest()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var now = DateTime.Now;
        var past = DateTime.Now.AddHours(-1);

        var userDbContextMock = new Mock<UserDbContext>(new DbContextOptions<UserDbContext>());
        userDbContextMock
            .Setup(x => x.Set<Document>())
            .ReturnsDbSet(new List<Document>()
            {
                new(Guid.NewGuid(), userId, DocumentType.Selfie,"filUrl1","fileName1")
                {
                    CreatedAt = past
                },
                new(Guid.NewGuid(), userId, DocumentType.Selfie,"filUrl2","fileName2")
                {
                    CreatedAt = now
                }
            });

        // Setup SUT.
        var repository = new DocumentRepository(userDbContextMock.Object, _logger.Object);

        // Act
        var actual = await repository.GetDocumentsByUserId(userId);

        // Assert
        Assert.Equal("fileName2", actual[0].FileName);
        Assert.Equal("fileName1", actual[1].FileName);
    }
}