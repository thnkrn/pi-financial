using Moq;
using Pi.Common.Features;
using Pi.TfexService.Application.Queries.Margin;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Domain.Models.InitialMargin;

namespace Pi.TfexService.Application.Tests.Queries.Margin;

public class InitialMarginQueriesBaseTests
{
    protected readonly Mock<IInitialMarginRepository> InitialMarginRepository;
    protected readonly Mock<ISetTradeService> SetTradeService;
    protected readonly IInitialMarginQueries InitialMarginQueries;
    protected readonly Mock<IFeatureService> FeatureService;

    public InitialMarginQueriesBaseTests()
    {
        InitialMarginRepository = new Mock<IInitialMarginRepository>();
        SetTradeService = new Mock<ISetTradeService>();
        FeatureService = new Mock<IFeatureService>();
        InitialMarginQueries = new InitialMarginQueries(InitialMarginRepository.Object, SetTradeService.Object, FeatureService.Object);
    }
}