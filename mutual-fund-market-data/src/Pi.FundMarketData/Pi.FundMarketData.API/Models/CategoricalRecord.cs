namespace Pi.FundMarketData.API.Models;

public class CategoricalRecord<TDimension, TMeasurement>
{
    public TDimension Dimension { get; init; }
    public TMeasurement Value { get; init; }

    public CategoricalRecord(TDimension dimension, TMeasurement measurement)
    {
        Dimension = dimension;
        Value = measurement;
    }
}
