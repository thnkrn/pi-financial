using MongoDB.Bson;

namespace Pi.CsvDataImporter.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class CuratedMember
{
    public ObjectId Id { get; set; }
    public int CuratedListId { get; set; }
    public int InstrumentId { get; set; }
    public int Ordering { get; set; }
    public bool IsDefault { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string SubGroupName { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}