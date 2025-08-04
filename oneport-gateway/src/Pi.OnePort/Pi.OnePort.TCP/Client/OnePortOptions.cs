namespace Pi.OnePort.TCP.Client;

public class OnePortOptions
{
    public const string Options = "OnePort";

    public required string Ip { get; set; }
    public required int Port { get; set; }
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public int SendTimeout { get; set; } = 1000;
    public int TimeoutPeriod { get; set; } = 5 * 1000;
}
