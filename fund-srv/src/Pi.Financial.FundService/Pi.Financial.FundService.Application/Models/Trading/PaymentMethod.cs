namespace Pi.Financial.FundService.Application.Models.Trading;

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum PaymentMethod
{
    Ats,
    BankTransfer,
}
