using Pi.Common.SeedWork;

namespace Pi.User.Domain.AggregatesModel.DocumentAggregate;

public interface IDocumentRepository : IRepository<Document>
{
    Document CreateDocument(Document document);
    Task<List<Document>> GetDocumentsByUserId(Guid userId);
}