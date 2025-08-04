using MassTransit;
using Pi.GlobalMarketData.Application.Commands;
using Pi.GlobalMarketData.Application.Mediator;
using Pi.GlobalMarketData.Application.Queries;
using Pi.GlobalMarketData.Application.Queries.MarketFilterInstruments;
using Pi.GlobalMarketData.Infrastructure;

namespace Pi.GlobalMarketData.API.Startup;

public static class MassTransitExtensions
{
    public static IServiceCollection SetupMassTransit(
        this IServiceCollection services,
        ConfigurationManager configuration,
        IWebHostEnvironment environment
    )
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.UsingInMemory();
            busConfigurator.AddMediator(x =>
            {
                x.ConfigureMediator(
                    (ctx, mcfg) => { mcfg.UseSendFilter(typeof(AuthorizationFilter<>), ctx); }
                );
                x.AddConsumers(typeof(InfrastructureAssemblyMarker).Assembly);

                x.AddRequestClient(typeof(GetGeInstrumentRequest));
                x.AddRequestClient(typeof(GetByIdGeInstrumentRequest));
                x.AddRequestClient(typeof(GetBySymbolGeInstrumentRequest));

                x.AddRequestClient(typeof(GetCuratedFilterRequest));
                x.AddRequestClient(typeof(GetByIdCuratedFilterRequest));
                x.AddRequestClient(typeof(CreateCuratedFilterRequest));
                x.AddRequestClient(typeof(UpdateCuratedFilterRequest));
                x.AddRequestClient(typeof(DeleteCuratedFilterRequest));

                x.AddRequestClient(typeof(GetCuratedListRequest));
                x.AddRequestClient(typeof(GetByIdCuratedListRequest));
                x.AddRequestClient(typeof(CreateCuratedListRequest));
                x.AddRequestClient(typeof(UpdateCuratedListRequest));
                x.AddRequestClient(typeof(DeleteCuratedListRequest));
                
                x.AddRequestClient(typeof(GetCuratedMemberRequest));
                x.AddRequestClient(typeof(GetByIdCuratedMemberRequest));
                x.AddRequestClient(typeof(CreateCuratedMemberRequest));
                x.AddRequestClient(typeof(UpdateCuratedMemberRequest));
                x.AddRequestClient(typeof(DeleteCuratedMemberRequest));

                x.AddRequestClient(typeof(PostMarketTickerRequest));
                x.AddRequestClient(typeof(PostMarketProfileDescriptionRequest));
                x.AddRequestClient(typeof(PostMarketProfileFinancialsRequest));
                x.AddRequestClient(typeof(PostMarketProfileFundamentalsRequest));
                x.AddRequestClient(typeof(PostMarketTimelineRenderedRequest));
                x.AddRequestClient(typeof(PostMarketDerivativeInformationRequest));
                x.AddRequestClient(typeof(PostMarketScheduleRequest));
                x.AddRequestClient(typeof(PostMarketFiltersRequest));
                x.AddRequestClient(typeof(PostMarketIndicatorRequest));
                x.AddRequestClient(typeof(PostMarketFilterInstrumentsRequest));
                x.AddRequestClient(typeof(PostMarketProfileOverViewRequest));
                x.AddRequestClient(typeof(PostHomeInstrumentRequest));
                x.AddRequestClient(typeof(PostMarketStatusRequest));
                x.AddRequestClient(typeof(PostMarketInstrumentInfoRequest));
                x.AddRequestClient(typeof(PostOrderBookRequest));
            });
        });

        return services;
    }
}