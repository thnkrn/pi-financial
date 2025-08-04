// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Pi.FundMarketData.Models;

public class FundSearchData
{
    public string Symbol { get; set; }
    public string Name { get; set; }
    public decimal? Nav { get; set; }
    public decimal? ValueChange { get; set; }
    public double? NavChangePercentage { get; set; }
    public string Currency { get; set; }
    public string AmcCode { get; set; }
}
