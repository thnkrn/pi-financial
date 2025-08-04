using MongoDB.Bson;

namespace Pi.CsvDataImporter.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

public class ImportHistory
{
    public ObjectId Id { get; set; }
    public string CollectionName { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string FileHash { get; set; } = string.Empty;
    public long RecordCount { get; set; }
    public DateTime ImportedAt { get; set; }
    public string Version { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }
}