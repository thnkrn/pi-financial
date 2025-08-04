using System.ComponentModel;

namespace Pi.Financial.FundService.Application.Models.Enums;

public enum MailingMethod
{
    [Description("Email")] Email,
    [Description("Fax")] Fax,
    [Description("Post")] Post,
}
