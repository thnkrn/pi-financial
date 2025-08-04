using Microsoft.EntityFrameworkCore.DataEncryption;

namespace Pi.User.Application.Services.Cryptography;

public interface ICryptographyService : IEncryptionProvider
{
    public string Hash(string input);
}