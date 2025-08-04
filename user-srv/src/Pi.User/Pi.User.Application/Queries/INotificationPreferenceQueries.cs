using Pi.User.Application.Models;
using Pi.User.Domain.AggregatesModel;

namespace Pi.User.Application.Queries;

public interface INotificationPreferenceQueries
{
    Task<Device> GetNotificationPreference(Guid userId, Guid deviceId);
}