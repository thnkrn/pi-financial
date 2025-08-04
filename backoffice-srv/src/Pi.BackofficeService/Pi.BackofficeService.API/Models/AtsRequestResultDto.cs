namespace Pi.BackofficeService.API.Models;


public class AtsRequestResultDto
{
    public AtsRequestResultDto(
        int recordCount
    )
    {
        RecordCount = recordCount;
    }

    public int RecordCount { get; init; }
}
