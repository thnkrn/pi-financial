using MassTransit;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;

namespace Pi.User.Application.Commands;

public record UploadDocumentRequest(string Image, DocumentType DocType);
public record SubmitDocument(DocumentType Type, string FileUrl, string FileName);
public record SubmitDocumentRequest(Guid UserId, List<SubmitDocument> Documents);

public class SubmitDocumentConsumer : IConsumer<SubmitDocumentRequest>
{
    private readonly IDocumentRepository _documentRepository;

    public SubmitDocumentConsumer(
        IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task Consume(ConsumeContext<SubmitDocumentRequest> context)
    {
        try
        {
            foreach (var doc in context.Message.Documents)
            {
                var document = new Document(Guid.NewGuid(), context.Message.UserId, doc.Type, doc.FileUrl, doc.FileName);
                _documentRepository.CreateDocument(document);
            }

            await _documentRepository.UnitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Unable to save document", e);
        }
    }
}