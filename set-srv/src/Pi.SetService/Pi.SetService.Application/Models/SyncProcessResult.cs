namespace Pi.SetService.Application.Models;

public class SyncProcessResult()
{
    public int Create { get; set; }
    public int Update { get; set; }
    public int Skip { get; set; }
    public int Execution { get; set; }
}
