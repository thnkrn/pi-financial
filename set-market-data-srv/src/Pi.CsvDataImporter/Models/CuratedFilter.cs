using MongoDB.Bson;

namespace Pi.CsvDataImporter.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class CuratedFilter
{
    public ObjectId Id { get; set; }
    public int FilterId { get; set; }
    public string FilterName { get; set; } = string.Empty;
    public string FilterCategory { get; set; } = string.Empty;
    public string FilterType { get; set; } = string.Empty;
    public int CategoryPriority { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string SubGroupName { get; set; } = string.Empty;
    public int CuratedListId { get; set; }
    public bool IsDefault { get; set; }
    public bool Highlight { get; set; }
    public int Ordering { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}