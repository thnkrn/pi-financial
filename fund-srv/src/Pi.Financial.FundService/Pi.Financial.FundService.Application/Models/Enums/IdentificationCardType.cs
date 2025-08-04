using System.ComponentModel;

namespace Pi.Financial.FundService.Application.Models.Enums;

public enum IdentificationCardType
{
    [Description("CITIZEN_CARD")] Citizen,
    [Description("PASSPORT")] Passport,
}
