using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Domain.AggregatesModel.AddressAggregate;

public class Address(Guid id) : Entity<Guid>(id), IAggregateRoot
{
    public string? Place { get; set; }
    public string? HomeNo { get; set; }
    public string? Town { get; set; }
    public string? Building { get; set; }
    public string? Village { get; set; }
    public string? Floor { get; set; }
    public string? Soi { get; set; }
    public string? Road { get; set; }
    public string? SubDistrict { get; set; }
    public string? District { get; set; }
    public string? Province { get; set; }
    public string? Country { get; set; }
    public string? ZipCode { get; set; }
    public string? CountryCode { get; set; }
    public string? ProvinceCode { get; set; }
    public Guid UserId { get; init; }
}