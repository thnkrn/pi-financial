using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;

namespace Pi.User.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly UserDbContext _userDbContext;
    private readonly ILogger<DocumentRepository> _logger;
    public IUnitOfWork UnitOfWork => _userDbContext;

    public DocumentRepository(
        UserDbContext userDbContext,
        ILogger<DocumentRepository> logger)
    {
        _userDbContext = userDbContext;
        _logger = logger;
    }

    public Document CreateDocument(Document document)
    {
        return _userDbContext.Documents.Add(document).Entity;
    }

    public async Task<List<Document>> GetDocumentsByUserId(Guid userId)
    {
        return await _userDbContext.Documents
            .Where(x => x.UserId.Equals(userId))
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
}