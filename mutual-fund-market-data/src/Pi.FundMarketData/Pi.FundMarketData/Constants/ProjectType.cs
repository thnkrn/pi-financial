// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Pi.FundMarketData.Constants;

public enum ProjectType
{
    Unknown = 0,
    GeneralInvestorsFund,
    InstitutionalInvestorsFund,
    NonRetailInvestorsFund,
    NonRetailAndHighNetWorthInvestorsFund,
    InstitutionalAndSpecialLargeInvestorsFund,
    HighNetWorthInvestorsFund,
    CorporateInvestorsFund,
    ThaiGovernmentFund,
    PensionReserveFund,
}

public static class ProjectTypeExtension
{
    public static ProjectType ParseProjectType(string value)
    {
        return value.Trim() switch
        {
            "R" => ProjectType.GeneralInvestorsFund,
            "N" => ProjectType.InstitutionalInvestorsFund,
            "A" => ProjectType.NonRetailInvestorsFund,
            "X" => ProjectType.NonRetailAndHighNetWorthInvestorsFund,
            "H" => ProjectType.InstitutionalAndSpecialLargeInvestorsFund,
            "B" => ProjectType.HighNetWorthInvestorsFund,
            "J" => ProjectType.CorporateInvestorsFund,
            "G" => ProjectType.ThaiGovernmentFund,
            "V" => ProjectType.PensionReserveFund,
            _ => ProjectType.Unknown
        };
    }
}
