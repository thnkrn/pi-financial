
using Microsoft.AspNetCore.Http;

namespace Pi.SetMarketData.Infrastructure.Interfaces.Utils;

public interface IFileUtils
{
    void EnsureDirectoryExists();
    void SaveFile(IFormFile file);
}