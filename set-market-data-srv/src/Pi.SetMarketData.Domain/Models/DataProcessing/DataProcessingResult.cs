namespace Pi.SetMarketData.Domain.Models.DataProcessing;

public class DataProcessingResult
{
    public DataProcessingChannel Channel { get; set; }
    public DataProcessingValue[]? Values { get; set; }
}

public class DataProcessingValue
{
    public string? TableName { get; set; }
    public object? Value { get; set; }
}