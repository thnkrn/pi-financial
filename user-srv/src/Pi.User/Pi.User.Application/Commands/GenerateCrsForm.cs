using MassTransit;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;
using Pi.OnboardService.IntegrationEvents;
using Pi.User.Application.Utils;
using Pi.Financial.Client.PdfService.Model;
using System.Globalization;
using Pi.User.Application.Services.Pdf;
using Microsoft.Extensions.Logging;

namespace Pi.User.Application.Commands;

public class GenerateCrsFormConsumer : IConsumer<GenerateCrsFormEvent>
{
    private readonly IPdfService _pdfService;
    private readonly ILogger<GenerateCrsFormConsumer> _logger;

    public GenerateCrsFormConsumer(
        IPdfService pdfService,
        ILogger<GenerateCrsFormConsumer> logger)
    {
        _pdfService = pdfService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GenerateCrsFormEvent> context)
    {
        try
        {
            var fileName = $"{context.Message.UserId}_{DocumentType.FatcaCrs.ToString()}_{CommonUtil.RandomNumberCode()}.pdf";
            var crsForm = new FatcaCRSFormDto
            (
                applicantName: context.Message.ApplicationNameEn,
                applicantNameTH: context.Message.ApplicationNameTh,
                applicantSignature: context.Message.ApplicationNameTh,
                nationality: "Thai",
                date: DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                idNumber: context.Message.CitizenId,
                isUSCitizen: false,
                isGreenCardHolder: false,
                isUSForTax: false,
                isBornInUS: false,
                haveUSResidence: false,
                haveUSPhoneNumber: false,
                haveUSAccountReceiver: false,
                haveUSSignatoryAuthority: false,
                haveSoleAddress: false,
                officerSignature: string.Empty,
                crs: context.Message.Details.Select(
                    x => new CRSDto(
                        x.CountryOfTaxResidence,
                        x.Tin ?? "",
                        x.Reason?.ToString() ?? "",
                        x.ReasonDesc ?? ""
                    )
                ).ToList() ?? new List<CRSDto>(),
                placeOfBirthCity: context.Message.PlaceOfBirthCityFullName,
                placeOfBirthCountry: context.Message.PlaceOfBirthCountryFullName
            );

            var fileUrl = await _pdfService.GenerateCrsForm(crsForm, fileName);

            await context.Publish(
                new SubmitDocumentRequest(
                    context.Message.UserId,
                    new List<SubmitDocument>() {
                        new SubmitDocument(DocumentType.FatcaCrs, fileUrl, fileName)
                    }
                )
            );
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while consume GenerateCrsFormConsumer with {Message}", e.Message);
            throw;
        }
    }
}