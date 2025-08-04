using System.ComponentModel;

namespace Pi.Financial.FundService.Application.Models.Enums;

public enum MailingAddressOption
{
    [Description("IdDocument")]
    IdAddress,
    [Description("Current")]
    CurrentAddress,
    [Description("Work")]
    WorkAddress,
    [Description("")]
    Other,
}
