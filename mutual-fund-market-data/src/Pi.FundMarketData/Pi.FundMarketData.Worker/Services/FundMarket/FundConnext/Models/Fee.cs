// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Pi.FundMarketData.Utils;

namespace Pi.FundMarketData.Worker.Services.FundMarket.FundConnext.Models;

public class Fee
{
    public const string SwitchingInFeeType = "7";
    private readonly string _fundCode;
    public string FundCode { get => _fundCode; init => _fundCode = value.ToUpper(); }
    public string FeeType { get; init; }
    public string FeeUnit { get; init; }
    public decimal? ActualFee { get; init; }
    public DateTime? EffectiveDate { get; init; }

    public static readonly Func<string[], string, Fee> Mapper = (dataValues, asOfDate) =>
    {
        return new Fee
        {
            FundCode = dataValues[1],
            FeeType = dataValues[2],
            FeeUnit = dataValues[3],
            ActualFee = UtilsMethod.StringToDecimal(dataValues[5]),
            EffectiveDate = UtilsMethod.StringToDateTime(dataValues[0])
        };
    };
}
