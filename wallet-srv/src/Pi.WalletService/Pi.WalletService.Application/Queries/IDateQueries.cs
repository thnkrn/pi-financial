namespace Pi.WalletService.Application.Queries;

public interface IDateQueries
{
    Task<DateTime> GetNextBusinessDay(DateTime currentDate);
}