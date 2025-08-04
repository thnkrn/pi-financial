using System.ComponentModel;

namespace Pi.SetService.Domain.AggregatesModel.ErrorAggregate;

public enum SetErrorCode
{
    [Description("Invalid trading account no")]
    SE001,
    [Description("Invalid action")] SE002,
    [Description("Action unsupported")] SE003,

    [Description("Limit order 'Price' can't be 0 or null")]
    SE004,

    [Description("Market order 'Price' must be null")]
    SE005,

    [Description("Quantity must have more than 0")]
    SE006,

    [Description("Quantity must have multiple by 100")]
    SE007,

    [Description("Query date range exceed maximum range")]
    SE008,

    [Description("Invalid query date range")]
    SE009,

    [Description("Offline order condition price is not supported")]
    SE010,

    [Description("Offline order condition is not supported")]
    SE011,

    [Description("Customer can't be found")]
    SE101,

    [Description("Trading account can't be found")]
    SE102,

    [Description("Account balance can't be found")]
    SE103,

    [Description("Instrument can't be found")]
    SE104,

    [Description("Order price out of range of ceiling and floor")]
    SE105,

    [Description("Insufficient unit balance")]
    SE106,

    [Description("System maintenance time")]
    SE107,

    [Description("Open order can't be found")]
    SE108,

    [Description("Requested order volume more than macthed volume")]
    SE109,

    [Description("Margin rate can't be found")]
    SE110,

    [Description("SBL instrument not found")]
    SE111,

    [Description("Insufficient SBL available lending")]
    SE112,

    [Description("Can't create SBL order")]
    SE113,

    [Description("Account position can't be found")]
    SE114,

    [Description("Account SBL disabled")]
    SE115,

    [Description("SBL order can't be found")]
    SE116,

    [Description("Equity order state can't be found")]
    SE117,

    [Description("Something went wrong")] SE201,

    [Description("Order had been rejected (\'{0}\')")]
    SE202,

    [Description("Cannot trade ATO price in MKT open period")]
    SE203,

    [Description("MP can trade in MKT open period only")]
    SE204,

    [Description("ATC can trade in MKT call market only")]
    SE205,

    [Description("Wash Sell order not allow")]
    SE206,

    [Description("ATO/ATC/MKT must trade with condition IOC only")]
    SE207,

    [Description("This Customer Type Is Not Allow For NVDR")]
    SE208,

    [Description("SET products under maintenance time")]
    SE209,
}
