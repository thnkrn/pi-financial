using Pi.FundMarketData.Worker.Services.FundMarket.FundConnext.Models;

namespace Pi.FundMarketData.Worker.Services.FundMarket.FundConnext
{
    public interface IFundConnextService
    {
        Task Authenticate(CancellationToken ct);
        Task<IEnumerable<FundProfile>> GetFundProfiles(DateTime businessDate, CancellationToken ct);
        Task<IEnumerable<Nav>> GetFundNavInfos(DateTime businessDate, CancellationToken ct);
        Task<IEnumerable<SwitchingMatrix>> GetSwitchingInfos(DateTime businessDate, CancellationToken ct);
        Task<IEnumerable<TradeCalendar>> GetTradeCalendars(DateTime businessDate, CancellationToken ct);
        Task<IEnumerable<FundHoliday>> GetFundHolidays(DateTime businessDate, CancellationToken ct);
        Task<IEnumerable<Fee>> GetFeeInfos(DateTime businessDate, CancellationToken ct);
        Task<IEnumerable<FundMapping>> GetFundMappings(DateTime businessDate, CancellationToken ct);
    }
}
