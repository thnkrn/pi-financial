using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pi.Client.CgsBank;

public record CgsBankTransportObject([property: JsonPropertyName("data")]
    string Data);

public class CgsBankSecurityHandler : DelegatingHandler
{
    private readonly string _secretKey;

    public CgsBankSecurityHandler(string secretKey)
    {
        _secretKey = secretKey;
    }

    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await EncryptRequestBody(request);

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        return (request.RequestUri?.AbsolutePath.Contains("GetToken") ?? false) || !response.IsSuccessStatusCode
            ? response
            : await DecryptResponseBody(response);
    }

    private async Task EncryptRequestBody(HttpRequestMessage request)
    {
        var reqBody = await GetRequestBody(request);

        if (string.IsNullOrWhiteSpace(reqBody))
        {
            return;
        }

        // Start Encryption
        var encryptedData = EncryptString(reqBody, GenerateIv());

        // Assign EncryptedBody to RequestBody
        var body = new CgsBankTransportObject(encryptedData);

        request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
    }

    private static string GenerateIv()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 16)
            .Select(s => s[RandomNumberGenerator.GetInt32(s.Length)]).ToArray());
    }

    private async static Task<string> GetRequestBody(HttpRequestMessage request)
    {
        var responseBody = string.Empty;

        if (request.Content == null)
        {
            return responseBody;
        }

        await using var stream = await request.Content.ReadAsStreamAsync();
        stream.Seek(0, SeekOrigin.Begin);
        using var sr = new StreamReader(stream);
        responseBody = await sr.ReadToEndAsync();

        return responseBody;
    }

    private async Task<HttpResponseMessage> DecryptResponseBody(HttpResponseMessage response)
    {
        // Read Response
        var strContent = await response.Content.ReadAsStringAsync();

        // Deserialize to CgsBankTransport Object
        var strDeserialize = JsonSerializer.Deserialize<CgsBankTransportObject>(strContent);

        if (strDeserialize == null) return response;
        var strDecrypt = strDeserialize.Data;
        var responseStr = DecryptString(strDecrypt);
        response.Content = new StringContent(responseStr, Encoding.UTF8, "application/json");

        return response;
    }

    private string EncryptString(string plainText, string iv)
    {
        var keyBytes = Encoding.ASCII.GetBytes(_secretKey);
        var ivBytes = Encoding.ASCII.GetBytes(iv);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Key = keyBytes;
        aes.IV = ivBytes;

        byte[] encrypted;
        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

        using (var ms = new MemoryStream())
        {
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                encrypted = ms.ToArray();
            }
        }

        return iv + ":" + Convert.ToBase64String(encrypted);
    }

    private string DecryptString(string cipherText)
    {
        var iv = cipherText.Split(":")[0];
        var cipherTextBytes = Convert.FromBase64String(cipherText.Split(":")[1]);

        if (string.IsNullOrEmpty(cipherText))
            throw new InvalidDataException(nameof(cipherText));
        if (string.IsNullOrEmpty(_secretKey))
            throw new InvalidDataException(nameof(_secretKey));
        if (string.IsNullOrEmpty(iv))
            throw new InvalidDataException(nameof(iv));

        var keyBytes = Encoding.ASCII.GetBytes(_secretKey);
        var ivBytes = Encoding.ASCII.GetBytes(iv);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Key = keyBytes;
        aes.IV = ivBytes;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using var ms = new MemoryStream(cipherTextBytes);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var sr = new StreamReader(cs);
        var decryptData = sr.ReadToEnd();

        return decryptData;
    }
}