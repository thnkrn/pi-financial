namespace Pi.BackofficeService.Application.Models.Customer
{
    public record CustomerDto(
        string Id,
        List<DeviceDto> Devices,
        List<CustomerCodeDto> CustomerCodes,
        List<TradingAccountDto> TradingAccounts,
        string? FirstnameTh,
        string? LastnameTh,
        string? FirstnameEn,
        string? LastnameEn,
        string? PhoneNumber,
        string? GlobalAccount,
        string? Email
    )
    {
        public string GetFullName()
        {
            return $"{FirstnameTh} {LastnameTh} {FirstnameEn} {LastnameEn}".Trim();
        }
    };
}
