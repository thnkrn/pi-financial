using Microsoft.AspNetCore.Http;

namespace Pi.GlobalMarketData.Infrastructure.Interfaces.Utils;

public interface IFileUtils
{
    void EnsureDirectoryExists();
    void SaveFile(IFormFile file);
}