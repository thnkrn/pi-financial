using GrowthBook;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pi.Common.Features.Models;
using Pi.SetService.Application.Services.FeatureService;
using Experiment = Pi.Common.Features.Models.Experiment;
using ExperimentAssignment = Pi.Common.Features.Models.ExperimentAssignment;
using ExperimentResult = Pi.Common.Features.Models.ExperimentResult;
using FeatureResult = Pi.Common.Features.Models.FeatureResult;

namespace Pi.SetService.Infrastructure.Services;

// Note: This class is clone of feature service and serve worker feature flag to support attributes like userId
public class CustomFeatureService : ICustomFeatureService
{
    private readonly ILogger<ICustomFeatureService> _logger;
    private readonly HttpClient _httpClient;
    private readonly GrowthBook.GrowthBook _growthBook;

    public CustomFeatureService(
        HttpClient httpClient,
        ILogger<CustomFeatureService> logger,
        string apiKey,
        string projectId,
        string baseUrl = "",
        bool? isQaMode = false,
        IDictionary<string, string>? attributes = null,
        IReadOnlyCollection<string>? forcedVariations = null)
    {
        _httpClient = httpClient;
        _logger = logger;
        _growthBook = InitGrowthBook(
                baseUrl,
                apiKey,
                projectId,
                isQaMode,
                attributes,
                forcedVariations
            )
            .GetAwaiter()
            .GetResult();
    }

    public bool IsOn(string key)
    {
        return _growthBook.IsOn(key);
    }

    public bool IsOff(string key)
    {
        return _growthBook.IsOff(key);
    }

    public Action Subscribe(Action<Experiment, ExperimentResult> callback)
    {
        var newDelegate =
            (Action<GrowthBook.Experiment, GrowthBook.ExperimentResult>)Delegate.CreateDelegate(
                typeof(Action<GrowthBook.Experiment, GrowthBook.ExperimentResult>),
                callback.Target,
                callback.Method);

        return _growthBook.Subscribe(newDelegate);
    }

    public FeatureResult EvaluateFeature(string key)
    {
        var featureResult = _growthBook.EvalFeature(key);

        return new FeatureResult
        {
            Value = featureResult.Value,
            Source = featureResult.Source,
            Experiment = new Experiment
            {
                Key = featureResult.Experiment.Key,
                Variations = featureResult.Experiment.Variations,
                Weights = featureResult.Experiment.Weights,
                Active = featureResult.Experiment.Active,
                Coverage = featureResult.Experiment.Coverage,
                Condition = featureResult.Experiment.Condition,
                Namespace = featureResult.Experiment.Namespace,
                Force = featureResult.Experiment.Force,
                HashAttribute = featureResult.Experiment.HashAttribute
            },
            ExperimentResult = new ExperimentResult
            {
                InExperiment = featureResult.ExperimentResult.InExperiment,
                VariationId = featureResult.ExperimentResult.VariationId,
                Value = featureResult.ExperimentResult.Value,
                HashUsed = featureResult.ExperimentResult.HashUsed,
                HashAttribute = featureResult.ExperimentResult.HashAttribute,
                HashValue = featureResult.ExperimentResult.HashValue
            }
        };
    }

    public ExperimentResult Run(Experiment experiment)
    {
        var experimentResult = _growthBook.Run(
            new GrowthBook.Experiment
            {
                Key = experiment.Key,
                Variations = experiment.Variations,
                Weights = experiment.Weights,
                Active = experiment.Active,
                Coverage = experiment.Coverage,
                Condition = experiment.Condition,
                Namespace = experiment.Namespace,
                Force = experiment.Force,
                HashAttribute = experiment.HashAttribute
            });

        return new ExperimentResult
        {
            InExperiment = experimentResult.InExperiment,
            VariationId = experimentResult.VariationId,
            Value = experimentResult.Value,
            HashUsed = experimentResult.HashUsed,
            HashAttribute = experimentResult.HashAttribute,
            HashValue = experimentResult.HashValue
        };
    }

    public IDictionary<string, ExperimentAssignment> GetAllExperimentAssignments()
    {
        var result = _growthBook.GetAllResults();

        return result.ToDictionary(
            r => r.Key,
            r => new ExperimentAssignment
            {
                Experiment = new Experiment
                {
                    Key = r.Value.Experiment.Key,
                    Variations = r.Value.Experiment.Variations,
                    Weights = r.Value.Experiment.Weights,
                    Active = r.Value.Experiment.Active,
                    Coverage = r.Value.Experiment.Coverage,
                    Condition = r.Value.Experiment.Condition,
                    Namespace = r.Value.Experiment.Namespace,
                    Force = r.Value.Experiment.Force,
                    HashAttribute = r.Value.Experiment.HashAttribute,
                },
                Result = new ExperimentResult
                {
                    InExperiment = r.Value.Result.InExperiment,
                    VariationId = r.Value.Result.VariationId,
                    Value = r.Value.Result.Value,
                    HashUsed = r.Value.Result.HashUsed,
                    HashAttribute = r.Value.Result.HashAttribute,
                    HashValue = r.Value.Result.HashValue,
                }
            });
    }

    public T GetFeatureValue<T>(string key, T fallback)
    {
        return _growthBook.GetFeatureValue(key, fallback);
    }

    private async Task<GrowthBook.GrowthBook> InitGrowthBook(
        string baseUrl,
        string apiKey,
        string projectId,
        bool? isQaMode,
        IDictionary<string, string>? attributes,
        IReadOnlyCollection<string>? forcedVariations)
    {
        var url = $"{baseUrl}/api/features/{apiKey}?project={projectId}";

        using var httpResponseMessage = await _httpClient.GetAsync(url);

        var content = await httpResponseMessage.Content.ReadAsStringAsync();

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            _logger.LogError("Receive non-success status code from GrowthBook API: {Content}", content);
        }

        httpResponseMessage.EnsureSuccessStatusCode();

        var result = JsonConvert.DeserializeObject<FeaturesResult?>(content);

        if (result == null)
        {
            throw new UnableToInitializedGrowthBookException();
        }

        return new GrowthBook.GrowthBook(
            new Context
            {
                Enabled = true,
                Url = url,
                Features = result.Features,
                QaMode = isQaMode ?? false,
                Attributes = attributes?.Any() ?? false
                    ? new JObject(attributes.Select(a => new JProperty(a.Key, a.Value)))
                    : new JObject(),
                ForcedVariations = forcedVariations?.Any() ?? false
                    ? new JObject(forcedVariations.Select((value, index) => new JProperty(value, index)))
                    : new JObject(),
            });
    }

    public void UpsertUserIdAttribute(Guid userId)
    {
        UpsertAttribute("userId", userId.ToString());
    }

    public void UpsertAttributes(IDictionary<string, string> attributes)
    {
        foreach (var keyValuePair in attributes)
        {
            UpsertAttribute(keyValuePair.Key, keyValuePair.Value);
        }
    }

    private void UpsertAttribute(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (_growthBook.Attributes.TryGetValue(key, out _))
        {
            _growthBook.Attributes[key] = value;
        }
        else
        {
            _growthBook.Attributes.Add(new JProperty(key, value));
        }
    }
}
