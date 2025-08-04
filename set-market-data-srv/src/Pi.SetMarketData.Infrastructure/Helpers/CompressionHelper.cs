using System.IO.Compression;
using System.Text;
using DotNetty.Codecs.Compression;
using Newtonsoft.Json;

namespace Pi.SetMarketData.Infrastructure.Helpers;

public static class CompressionHelper
{
    /// <summary>
    ///     Compresses a string using GZip compression
    /// </summary>
    /// <param name="data">The string to compress</param>
    /// <returns>Compressed byte array</returns>
    public static byte[] CompressString(string data)
    {
        if (string.IsNullOrEmpty(data))
            return [];

        try
        {
            using var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal))
            using (var writer = new StreamWriter(gzipStream, Encoding.UTF8))
            {
                writer.Write(data);
            }

            return outputStream.ToArray();
        }
        catch (Exception ex)
        {
            throw new CompressionException(ex.Message);
        }
    }

    /// <summary>
    ///     Decompresses a GZip compressed byte array back to a string
    /// </summary>
    /// <param name="compressedData">The compressed byte array</param>
    /// <returns>Decompressed string</returns>
    public static string DecompressData(byte[]? compressedData)
    {
        if (compressedData == null || compressedData.Length == 0)
            return string.Empty;

        try
        {
            using var inputStream = new MemoryStream(compressedData);
            using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzipStream, Encoding.UTF8);
            return reader.ReadToEnd();
        }
        catch (Exception ex)
        {
            throw new CompressionException(ex.Message);
        }
    }

    /// <summary>
    ///     Compresses an object to a byte array using GZip compression
    /// </summary>
    /// <typeparam name="T">Type of the object to compress</typeparam>
    /// <param name="obj">The object to compress</param>
    /// <param name="settings">Optional JSON serializer settings</param>
    /// <returns>Compressed byte array</returns>
    public static byte[] CompressObject<T>(T obj, JsonSerializerSettings? settings = null)
    {
        if (Equals(obj, default(T)))
            return [];

        try
        {
            var json = settings != null
                ? JsonConvert.SerializeObject(obj, settings)
                : JsonConvert.SerializeObject(obj);

            return CompressString(json);
        }
        catch (Exception ex)
        {
            throw new CompressionException(ex.Message);
        }
    }

    /// <summary>
    ///     Decompresses a GZip compressed byte array back to an object
    /// </summary>
    /// <typeparam name="T">Type of the object to decompress to</typeparam>
    /// <param name="compressedData">The compressed byte array</param>
    /// <param name="settings">Optional JSON serializer settings</param>
    /// <returns>Decompressed object</returns>
    public static T? DecompressToObject<T>(byte[]? compressedData, JsonSerializerSettings? settings = null)
    {
        if (compressedData == null || compressedData.Length == 0)
            return default;

        try
        {
            var json = DecompressData(compressedData);

            return settings != null
                ? JsonConvert.DeserializeObject<T>(json, settings)
                : JsonConvert.DeserializeObject<T>(json);
        }
        catch (Exception ex)
        {
            throw new CompressionException(ex.Message);
        }
    }

    /// <summary>
    ///     Converts a compressed byte array to a Base64 string for easier transport
    /// </summary>
    /// <param name="compressedData">The compressed byte array</param>
    /// <returns>Base64 encoded string</returns>
    public static string ToBase64String(byte[]? compressedData)
    {
        if (compressedData == null || compressedData.Length == 0)
            return string.Empty;

        try
        {
            return Convert.ToBase64String(compressedData);
        }
        catch (Exception ex)
        {
            throw new CompressionException(ex.Message);
        }
    }

    /// <summary>
    ///     Converts a Base64 string back to a compressed byte array
    /// </summary>
    /// <param name="base64String">The Base64 encoded string</param>
    /// <returns>Compressed byte array</returns>
    public static byte[] FromBase64String(string base64String)
    {
        try
        {
            return string.IsNullOrEmpty(base64String)
                ? []
                : Convert.FromBase64String(base64String);
        }
        catch (Exception ex)
        {
            throw new CompressionException(ex.Message);
        }
    }
}