using Pi.BackofficeService.Application.Models.Ats;
using System.Text.Json.Serialization;

namespace Pi.BackofficeService.API.Models;

public record UploadAtsPayload(string UserName, [property: JsonRequired] AtsUploadType UploadType, IFormFile UploadFile);
