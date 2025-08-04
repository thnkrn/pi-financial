using Pi.Common.Features;

namespace Pi.SetService.Application.Services.FeatureService;

public interface ICustomFeatureService : IFeatureService
{
    void UpsertUserIdAttribute(Guid userId);
    void UpsertAttributes(IDictionary<string, string> attributes);
}
