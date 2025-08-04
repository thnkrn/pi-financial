using AutoMapper;
using Pi.User.Application.Models.Document;
using Pi.User.Application.Queries.Storage;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;
using DbDocument = Pi.User.Domain.AggregatesModel.DocumentAggregate.Document;

namespace Pi.User.Application.Queries.Document;

public class DocumentQueries : IDocumentQueries
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IStorageQueries _storageQueries;


    public DocumentQueries(
        IDocumentRepository documentRepository,
        IStorageQueries storageQueries)
    {
        _documentRepository = documentRepository;
        _storageQueries = storageQueries;
    }

    public async Task<List<DocumentDto>> GetDocumentsByUserId(Guid userId)
    {
        var documents = await _documentRepository.GetDocumentsByUserId(userId);
        var config = new MapperConfiguration(cfg =>
                    cfg
                        .CreateMap<DbDocument, DocumentDto>()
                        .ForAllMembers(
                            opts => opts.Condition((src, dest, member) => member != null)
                        )
                );
        var mapper = config.CreateMapper();
        var mapResp = mapper.Map<List<DbDocument>, List<DocumentDto>>(documents);

        return mapResp;
    }

    public async Task<List<DocumentDto>> GetDocumentsWithPreSignedUrlByUserId(Guid userId)
    {
        var documents = await GetDocumentsByUserId(userId);
        foreach (var item in documents)
        {
            item.FileUrl = _storageQueries.GetPreSignedURL(item.FileName);
        }

        return documents;
    }
}