// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Pi.FundMarketData.API.Configs;

public class PiConfig
{
    public const string Name = "PiConfig";

    public TimeSpan CutOffTimeDeduction { get; init; }
}
