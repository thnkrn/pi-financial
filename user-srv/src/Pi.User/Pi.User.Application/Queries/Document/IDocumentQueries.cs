using Microsoft.AspNetCore.Http;
using Pi.User.Application.Models.Document;

namespace Pi.User.Application.Queries.Document;

public interface IDocumentQueries
{
    Task<List<DocumentDto>> GetDocumentsByUserId(Guid userId);
    Task<List<DocumentDto>> GetDocumentsWithPreSignedUrlByUserId(Guid userId);
}