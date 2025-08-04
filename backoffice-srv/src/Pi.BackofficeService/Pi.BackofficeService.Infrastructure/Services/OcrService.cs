using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Services.OcrService;
using Pi.BackofficeService.Application.Utils;
using Pi.BackofficeService.Domain.Exceptions;
using System.Net;
using System.Net.Http.Headers;

namespace Pi.BackofficeService.Infrastructure.Services
{
    public class OcrService : IOcrService
    {
        private readonly ILogger<OcrService> _logger;
        private readonly HttpClient _httpClient;

        public OcrService(HttpClient httpClient, ILogger<OcrService> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<OcrThirdPartyApiResponse> ScanDocumentsAsync(OcrDocumentType documentType, IList<OcrFileUploadModel> model, OcrOutputType output, string? password)
        {
            try
            {
                var apiPath = documentType.GetApiPath();

                if (output == OcrOutputType.Csv)
                    apiPath += "/csv";

                var client = _httpClient;
                var request = new HttpRequestMessage(HttpMethod.Post, apiPath);
                var content = new MultipartFormDataContent();
                foreach (var file in model)
                {
                    if (file.Data != null && !String.IsNullOrEmpty(file.FileName))
                    {
                        var fileContent = new ByteArrayContent(file.Data);
                        var extension = Path.GetExtension(file.FileName);
                        if (string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
                        {
                            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                        }

                        content.Add(fileContent, "file", file.FileName);
                    }
                }

                if (!String.IsNullOrEmpty(password))
                {
                    content.Add(new StringContent(password), "password");
                }

                request.Content = content;
                var result = await client.SendAsync(request);
                var message = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<OcrThirdPartyApiResponse>(message)!;
                var status = (int)result.StatusCode;

                if (status != 200)
                {
                    var errMessage = response.Error == null ?
                        "OCR - Internal Error" :
                        String.Format("OCR Error - {0}", response.Error);

                    throw new OcrException(errMessage);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error - calling ScanDocuments in OcrService and mapping to DTO");
                throw;
            }
        }
    }
}
