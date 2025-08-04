namespace Pi.BackofficeService.Application.Models.Customer
{
    public record NotificationPreferenceDto(bool Important, bool Order, bool Portfolio, bool Wallet, bool Market);
}
