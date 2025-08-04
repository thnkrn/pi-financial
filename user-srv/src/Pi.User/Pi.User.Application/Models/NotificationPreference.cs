namespace Pi.User.Application.Models;

public record NotificationPreference(bool Important, bool Order, bool Portfolio, bool Wallet, bool Market);