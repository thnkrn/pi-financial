// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Pi.Financial.FundService.Application.Models.Enums;

namespace Pi.Financial.FundService.Application.Models.Customer;

public record CustomerInfoForSyncCustomerFundAccount(
    IdentificationCardType IdentificationCardType,
    string CardNumber,
    Country? PassportCountry,
    MailingAddressOption MailingAddressOption,
    MailingMethod MailingMethod,
    ILookup<InvestmentObjective, string> InvestmentObjective,
    bool Approved);
