namespace Pi.Financial.FundService.Domain.Events
{
    public record FundCustomerCreated(string CustomerCode, bool Ndid, Guid? UserId);
}
