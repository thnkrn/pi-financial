using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Common.Cryptography;
using Pi.User.Application.Options;
using Pi.User.Application.Services.Cryptography;

namespace Pi.User.Infrastructure.Services;

public class EncryptionService : ICryptographyService
{
    private readonly IEncryption _encryption;
    private readonly IDecryption _decryption;
    private readonly ILogger<EncryptionService> _logger;
    private readonly DbConfig _dbConfig;

    public EncryptionService(IEncryption encryption, IDecryption decryption, IOptions<DbConfig> dbConfig, ILogger<EncryptionService> logger)
    {
        _encryption = encryption;
        _decryption = decryption;
        _logger = logger;
        _dbConfig = dbConfig.Value;
    }

    public byte[] Encrypt(byte[] input)
    {
        try
        {
            if (!input.Any())
            {
                return Array.Empty<byte>();
            }

            var encryptedString = _encryption.Encrypt(input, _dbConfig.PublicKey);

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

            var decryptedString = _decryption.Decrypt(input, _dbConfig.PrivateKey);

            return decryptedString;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to Decrypt. {Message}", e.Message);

            return Array.Empty<byte>();
        }
    }

    public string Hash(string input)
    {
        return _encryption.Hashed(input, _dbConfig.Salt);
    }
}