namespace Pi.OnePort.TCP.API.Options;

public class OperationHoursOptions
{
    public const string Options = "OperationHours";

    public required string Start { get; set; }
    public required string End { get; set; }
}
