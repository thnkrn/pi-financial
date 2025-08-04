using System.ComponentModel.DataAnnotations;

namespace Pi.GlobalEquities.DomainModels;

public interface IOrderRequest : IOrderType, IOrderValues, IValidatableObject
{
    string AccountId { get; }
    string Venue { get; }
    string Symbol { get; }
}
