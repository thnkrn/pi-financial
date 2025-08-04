using Pi.Financial.Client.PdfService.Model;

namespace Pi.User.Application.Services.Pdf;

public interface IPdfService
{
    public Task<string> GenerateCrsForm(
        FatcaCRSFormDto crsForm,
        string fileName
    );
}