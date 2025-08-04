namespace Pi.Financial.FundService.Domain.Events
{
    public record FundAccountCreated(string CustomerCode, string AccountCode, bool Ndid);
}
