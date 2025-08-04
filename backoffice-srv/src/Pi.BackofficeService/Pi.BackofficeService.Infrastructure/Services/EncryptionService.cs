using System.Text;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.Common.Cryptography;

namespace Pi.BackofficeService.Infrastructure.Services;

public class EncryptionService : IEncryptionProvider
{
    private readonly IEncryption _encryption;
    private readonly IDecryption _decryption;
    private readonly ILogger<EncryptionService> _logger;
    private readonly string _publicKey;
    private readonly string _privateKey;

    public EncryptionService(IEncryption encryption, IDecryption decryption, IConfiguration configuration, ILogger<EncryptionService> logger)
    {
        _encryption = encryption;
        _decryption = decryption;
        _logger = logger;
        _publicKey = Encoding.UTF8.GetString(Convert.FromBase64String(configuration["Database:PublicKey"] ?? string.Empty));
        _privateKey = Encoding.UTF8.GetString(Convert.FromBase64String(configuration["Database:PrivateKey"] ?? string.Empty));
    }

    public byte[] Encrypt(byte[] input)
    {
        try
        {
            if (!input.Any())
            {
                return Array.Empty<byte>();
            }

            var encryptedString = _encryption.Encrypt(input, _publicKey);

            return encryptedString;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to Encrypt. {Message}", e.Message);

            return Array.Empty<byte>();
        }
    }

    public byte[] Decrypt(byte[] input)
    {
        try
        {
            if (!input.Any())
            {
                return Array.Empty<byte>();
            }

            var decryptedString = _decryption.Decrypt(input, _privateKey);

            return decryptedString;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to Decrypt. {Message}", e.Message);

            return Array.Empty<byte>();
        }
    }
}
