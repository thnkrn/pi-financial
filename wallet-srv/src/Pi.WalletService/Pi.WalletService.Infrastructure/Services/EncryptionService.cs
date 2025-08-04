using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.DataEncryption;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Common.Cryptography;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.Infrastructure.Options;

namespace Pi.WalletService.Infrastructure.Services;

public class EncryptionService : IEncryptionProvider
{
    private const int AesBlockSize = 128;

    private readonly byte[] _aesKey;
    private readonly CipherMode _mode;
    private readonly PaddingMode _padding;

    private readonly ILogger<EncryptionService> _logger;


    public EncryptionService(
        IOptions<DatabaseOptions> options,
        ILogger<EncryptionService> logger)
    {
        _aesKey = Convert.FromBase64String(options.Value.AesKey);
        _mode = CipherMode.CBC;
        _padding = PaddingMode.PKCS7;

        _logger = logger;
    }

    public byte[] Encrypt(byte[] input)
    {
        try
        {
            if (!input.Any())
            {
                _logger.LogInformation("Input is empty. Unable to Encrypt.");
                return Array.Empty<byte>();
            }

            var encryptedString = AesEncrypt(input);

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
                _logger.LogInformation("Input is empty. Unable to Decrypt.");
                return Array.Empty<byte>();
            }

            var decryptedString = AesDecrypt(input);

            return decryptedString;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to Decrypt. {Message}", e.Message);

            return Array.Empty<byte>();
        }
    }

    #region AES
    private byte[] AesEncrypt(byte[] input)
    {
        try
        {
            using Aes aes = CreateCryptographyProvider(_aesKey, _mode, _padding);
            // Generate new IV for each encryption
            aes.GenerateIV();
            using ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);

            // Write IV to the beginning of the stream
            memoryStream.Write(aes.IV);
            cryptoStream.Write(input);
            cryptoStream.FlushFinalBlock();

            return StreamToBytes(memoryStream);
        }
        catch (Exception e)
        {
            _logger.LogError("Unable to Encrypt with AES. {Message}", e.Message);
            throw new CryptographyException.AesEncryptionException("Unable to Encrypt with AES.", e);
        }
    }

    private byte[] AesDecrypt(byte[] input)
    {
        try
        {
            using Aes aes = CreateCryptographyProvider(_aesKey, _mode, _padding);

            // Get IV from input
            aes.IV = input.Take(aes.BlockSize / 8).ToArray();

            using ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using MemoryStream memoryStream = new(input.Skip(aes.IV.Length).ToArray());
            using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);

            return StreamToBytes(cryptoStream);
        }
        catch (Exception e)
        {
            _logger.LogError("Unable to Decrypt with AES. {Message}", e.Message);
            throw new CryptographyException.AesDecryptionException("Unable to Decrypt with AES.", e);
        }
    }

    private byte[] StreamToBytes(Stream stream)
    {
        if (stream is MemoryStream ms)
        {
            return ms.ToArray();
        }

        using var output = new MemoryStream();
        stream.CopyTo(output);
        return output.ToArray();
    }

    private Aes CreateCryptographyProvider(byte[] key, CipherMode mode, PaddingMode padding)
    {
        var aes = Aes.Create();
        aes.Mode = mode;
        aes.KeySize = key.Length * 8;
        aes.BlockSize = AesBlockSize;
        aes.FeedbackSize = AesBlockSize;
        aes.Padding = padding;
        aes.Key = key;
        return aes;
    }

    #endregion
}