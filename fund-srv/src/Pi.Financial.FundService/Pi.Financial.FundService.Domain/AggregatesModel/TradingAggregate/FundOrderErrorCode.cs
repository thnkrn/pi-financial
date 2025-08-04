using System.ComponentModel;

namespace Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

public enum FundOrderErrorCode
{
    [Description("Unsupported payment method")]
    FOE001,
    [Description("Invalid quantity")]
    FOE002,
    [Description("Funds that have minimum either from first buy or next buy must be below limit")]
    FOE003,
    [Description("Fund sell amount/unit exceed outstand amount/unit")]
    FOE004,
    [Description("Fund min switch amount exceed limit")]
    FOE005,
    [Description("Fund information cannot be found")]
    FOE101,
    [Description("Trading Account cannot be found")]
    FOE102,
    [Description("Sale License cannot be found")]
    FOE103,
    [Description("Bank Account cannot be found")]
    FOE104,
    [Description("Selling or Switching tax fund is not allowed")]
    FOE105,
    [Description("Unit Holder Id cannot be found")]
    FOE106,
    [Description("Cant find the balance of the customer for this fund")]
    FOE107,
    [Description("Source Asset Nav is Zero")]
    FOE109,
    [Description("Buying fund is not allowed")]
    FOE110,
    [Description("Non retail Funds")]
    FOE111,
    [Description("Too close to cut off time")]
    FOE112,
    [Description("Something went wrong")]
    FOE201,
    [Description("Application appear to be incomplete")]
    FOE202,
    [Description("Citizen ID has expired")]
    FOE203,
    [Description("Buy Amount is less than the Minimum purchase")]
    FOE204,
    [Description("Switching Amount is less than the minimum purchase")]
    FOE205,
    [Description("Sell amount/unit is less than minimum sell amount/unit")]
    FOE206,
    [Description("Order has been rejected")]
    FOE207,
    [Description("Order effective date cannot be a date in the past")]
    FOE208,
    [Description("Order was submitted for a non-business day of this fund")]
    FOE209,
    [Description("Order was submitted after Fund Cut off time")]
    FOE210,
    [Description("Outstanding Unit/Balance is lower than the order submitted")]
    FOE211,
    [Description("Remaining Unit/balance is lower than required minimum balance")]
    FOE212,
    [Description("Suitability Test has expired")]
    FOE213,
    [Description("BankAccount is invalid")]
    FOE214,
    [Description("FinNet Customer No cannot be found")]
    FOE215,
    [Description("Account cannot be found")]
    FOE216,
    [Description("Maximum TESGX subscription is 300,000 baht")]
    FOE217,
    [Description("Service is under maintenance. please try again later")]
    FOE218,
}
