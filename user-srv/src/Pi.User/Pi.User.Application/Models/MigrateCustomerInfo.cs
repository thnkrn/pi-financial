using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.Models;

public record MigrateCustomerInfo(List<UserInfo> UserInfos, int Total);