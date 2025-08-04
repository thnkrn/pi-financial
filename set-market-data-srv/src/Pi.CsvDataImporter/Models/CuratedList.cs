using MongoDB.Bson;

namespace Pi.CsvDataImporter.Models;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class CuratedList
{
    public ObjectId Id { get; set; }
    public int CuratedListId { get; set; }
    public string CuratedListCode { get; set; } = string.Empty;
    public string CuratedType { get; set; } = string.Empty;
    public string RelevantTo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Hashtag { get; set; } = string.Empty;
    public float Ordering { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public string UpdateBy { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}