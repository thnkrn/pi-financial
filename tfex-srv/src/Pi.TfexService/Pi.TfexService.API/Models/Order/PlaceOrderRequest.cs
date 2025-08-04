using System.Globalization;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.API.Models.Order;

public record PlaceOrderRequest(
    string Series,
    Side Side,
    Position Position,
    PriceType PriceType,
    decimal? Price,
    int Volume,
    int IcebergVol = 0,
    Validity? ValidityType = null,
    string? ValidityDateCondition = null,
    TriggerCondition? StopCondition = null,
    string? StopSymbol = null,
    decimal? StopPrice = null,
    TriggerSession? TriggerSession = null,
    bool? BypassWarning = true
)
{
    // validate the request
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Series))
        {
            throw new ArgumentException("Series is required", nameof(Series));
        }

        if (!Enum.IsDefined(typeof(Side), Side))
        {
            throw new ArgumentException($"Invalid Side: {Side}", nameof(Side));
        }

        if (!Enum.IsDefined(typeof(Position), Position))
        {
            throw new ArgumentException($"Invalid Position: {Position}", nameof(Position));
        }

        if (!Enum.IsDefined(typeof(PriceType), PriceType))
        {
            throw new ArgumentException($"Invalid PriceType: {PriceType}", nameof(PriceType));
        }

        switch (PriceType)
        {
            // validate price type, price must be 0 if price type is ATO or MP-MTL or MP-MKT
            // price must be greater than 0 if price type is Limit
            case PriceType.Limit when Price is null or <= 0:
                throw new SetTradePriceOutOfRangeException("Price must greater than 0 when PriceType is Limit");
        }

        if (Volume <= 0)
        {
            throw new ArgumentException($"Invalid Volume: {Volume}", nameof(Volume));
        }

        if (IcebergVol < 0)
        {
            throw new ArgumentException($"Invalid IcebergVol: {IcebergVol}", nameof(IcebergVol));
        }

        if (ValidityType != null && !Enum.IsDefined(typeof(Validity), ValidityType))
        {
            throw new ArgumentException($"Invalid ValidityType: {ValidityType}", nameof(ValidityType));
        }

        // validate the validity date condition if the validity type is Date and must be format of yyyy-MM-dd
        if (ValidityType == Validity.Date &&
            (string.IsNullOrEmpty(ValidityDateCondition) ||
             !DateTime.TryParseExact(ValidityDateCondition, "yyyy-MM-dd",
                 CultureInfo.InvariantCulture, DateTimeStyles.None, out _)))
        {
            throw new ArgumentException($"Invalid ValidityDateCondition: {ValidityDateCondition}",
                nameof(ValidityDateCondition));
        }

        // if stop condition is provided; stop symbol, stop price and trigger session must be provided based on the stop condition
        if (!StopCondition.HasValue) return;

        if (!Enum.IsDefined(typeof(TriggerCondition), StopCondition))
        {
            throw new ArgumentException($"Invalid StopCondition: {StopCondition}", nameof(StopCondition));
        }

        // if stop condition is type of 'Price Movement', stop symbol and stop price must be provided
        // if stop condition is type of 'Session', trigger session must be provided
        var groupAttribute = GetAttribute<GroupAttribute>(StopCondition.Value);
        switch (groupAttribute?.GroupName)
        {
            case "Price Movement" when string.IsNullOrWhiteSpace(StopSymbol):
                throw new ArgumentException("StopSymbol is required", nameof(StopSymbol));
            case "Price Movement" when StopPrice is null or < 0:
                throw new ArgumentException($"Invalid StopPrice: {StopPrice}", nameof(StopPrice));
            case "Session":
                if (TriggerSession == null || !Enum.IsDefined(typeof(TriggerSession), TriggerSession))
                {
                    throw new ArgumentException($"Invalid TriggerSession: {TriggerSession}", nameof(TriggerSession));
                }

                break;
        }
    }

    private static TAttribute? GetAttribute<TAttribute>(Enum value)
        where TAttribute : Attribute
    {
        var enumType = value.GetType();
        var name = Enum.GetName(enumType, value);
        if (name == null) throw new ArgumentException($"Invalid Enum Value: {value}", nameof(value));
        return enumType.GetField(name)?.GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
    }
}