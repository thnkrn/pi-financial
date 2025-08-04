namespace Pi.GlobalEquities.Worker.ExternalServices.FeatureFlags;

public interface IFeatureService
{
    bool IsOn(string key);
    bool IsOn(string userId, string key);
}
