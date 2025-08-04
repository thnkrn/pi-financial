using System.ComponentModel;

namespace Pi.Financial.FundService.Application.Models.Enums;

public enum AccompanyingDocument
{
    [Description("CITIZEN_CARD")] Citizen,
    [Description("ALIEN_CARD")] AlienCard,
    [Description("")] DrivingLicense,
    [Description("")] GovernmentCard,
}
