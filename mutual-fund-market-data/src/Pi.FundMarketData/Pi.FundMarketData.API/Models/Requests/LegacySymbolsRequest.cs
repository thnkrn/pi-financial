// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.DataAnnotations;

namespace Pi.FundMarketData.API.Models.Requests;

public class LegacySymbolsRequest
{
    [Required]
    public IList<string> Symbols { get; init; }
}
