using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Pi.SetMarketData.Domain.ConstantConfigurations;
using Pi.SetMarketData.Infrastructure.Interfaces.Utils;

namespace Pi.SetMarketData.Infrastructure.Utils;

public class FileUtils : IFileUtils
{
    private readonly bool _enabled;
    private readonly string _directory;
    public FileUtils(IConfiguration configuration)
    {
        _enabled = configuration.GetValue<bool>(ConfigurationKeys.CsvEnabled);
        _directory = configuration.GetValue<string>(ConfigurationKeys.CsvDirectory) ?? "./CsvFiles";

        if (!_enabled) return;
        EnsureDirectoryExists();
    }

    public void EnsureDirectoryExists()
    {
        if (!Directory.Exists(_directory))
        {
            Directory.CreateDirectory(_directory);
        }
    }

    public void SaveFile(IFormFile file)
    {
        if (!_enabled) return;

        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is empty or invalid");
        }

        // Check file size (for example, limit to 10MB)
        if (file.Length > 10 * 1024 * 1024)
        {
            throw new ArgumentException("File size exceeds the limit");
        }

        // Create a safe filename to prevent path traversal attacks
        var fileName = Path.GetFileName(file.FileName);
        var filePath = Path.Combine(_directory, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        file.CopyTo(stream);
    }
}