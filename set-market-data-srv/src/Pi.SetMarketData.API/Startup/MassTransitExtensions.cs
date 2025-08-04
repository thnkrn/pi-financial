using MassTransit;
using Pi.SetMarketData.Application.Commands;
using Pi.SetMarketData.Application.Commands.BrokerInfo;
using Pi.SetMarketData.Application.Commands.CorporateAction;
using Pi.SetMarketData.Application.Commands.Filter;
using Pi.SetMarketData.Application.Commands.Financial;
using Pi.SetMarketData.Application.Commands.FundDetail;
using Pi.SetMarketData.Application.Commands.FundPerformance;
using Pi.SetMarketData.Application.Commands.FundTradeDate;
using Pi.SetMarketData.Application.Commands.Indicator;
using Pi.SetMarketData.Application.Commands.Instrument;
using Pi.SetMarketData.Application.Commands.InstrumentDetail;
using Pi.SetMarketData.Application.Commands.Intermission;
using Pi.SetMarketData.Application.Commands.MarketStatus;
using Pi.SetMarketData.Application.Commands.NavList;
using Pi.SetMarketData.Application.Commands.OrderBook;
using Pi.SetMarketData.Application.Commands.PriceInfo;
using Pi.SetMarketData.Application.Commands.PublicTrade;
using Pi.SetMarketData.Application.Commands.TradingSign;
using Pi.SetMarketData.Application.Mediator;
using Pi.SetMarketData.Application.Queries;
using Pi.SetMarketData.Application.Queries.CorporateAction;
using Pi.SetMarketData.Application.Queries.Filter;
using Pi.SetMarketData.Application.Queries.Financial;
using Pi.SetMarketData.Application.Queries.FundDetail;
using Pi.SetMarketData.Application.Queries.FundPerformance;
using Pi.SetMarketData.Application.Queries.FundTradeDate;
using Pi.SetMarketData.Application.Queries.Indicator;
using Pi.SetMarketData.Application.Queries.Instrument;
using Pi.SetMarketData.Application.Queries.InstrumentDetail;
using Pi.SetMarketData.Application.Queries.Intermission;
using Pi.SetMarketData.Application.Queries.MarketDerivativeInformation;
using Pi.SetMarketData.Application.Queries.MarketFilterInstruments;
using Pi.SetMarketData.Application.Queries.MarketFilters;
using Pi.SetMarketData.Application.Queries.MarketIndicator;
using Pi.SetMarketData.Application.Queries.MarketInstrumentSearch;
using Pi.SetMarketData.Application.Queries.MarketProfileDescription;
using Pi.SetMarketData.Application.Queries.MarketProfileFinancials;
using Pi.SetMarketData.Application.Queries.MarketProfileFundamentals;
using Pi.SetMarketData.Application.Queries.MarketStatus;
using Pi.SetMarketData.Application.Queries.MarketTicker;
using Pi.SetMarketData.Application.Queries.MarketTimelineRendered;
using Pi.SetMarketData.Application.Queries.NavList;
using Pi.SetMarketData.Application.Queries.OrderBook;
using Pi.SetMarketData.Application.Queries.PriceInfo;
using Pi.SetMarketData.Application.Queries.PublicTrade;
using Pi.SetMarketData.Application.Queries.TradingSign;
using Pi.SetMarketData.Infrastructure;

namespace Pi.SetMarketData.API.Startup;

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
                    (ctx, cfg) =>
                    {
                        cfg.UseSendFilter(typeof(AuthorizationFilter<>), ctx);
                    }
                );
                x.AddConsumers(typeof(InfrastructureAssemblyMarker).Assembly);

                x.AddRequestClient(typeof(GetCorporateActionRequest));
                x.AddRequestClient(typeof(GetCuratedFilterRequest));
                x.AddRequestClient(typeof(GetCuratedListRequest));
                x.AddRequestClient(typeof(GetCuratedMemberRequest));
                x.AddRequestClient(typeof(GetFilterRequest));
                x.AddRequestClient(typeof(GetFinancialRequest));
                x.AddRequestClient(typeof(GetFundDetailRequest));
                x.AddRequestClient(typeof(GetFundPerformanceRequest));
                x.AddRequestClient(typeof(GetFundTradeDateRequest));
                x.AddRequestClient(typeof(GetIndicatorRequest));
                x.AddRequestClient(typeof(GetInstrumentRequest));
                x.AddRequestClient(typeof(GetInstrumentDetailRequest));
                x.AddRequestClient(typeof(GetIntermissionRequest));
                x.AddRequestClient(typeof(GetMarketStatusRequest));
                x.AddRequestClient(typeof(GetNavListRequest));
                x.AddRequestClient(typeof(GetOrderBookRequest));
                x.AddRequestClient(typeof(GetPriceInfoRequest));
                x.AddRequestClient(typeof(GetPublicTradeRequest));
                x.AddRequestClient(typeof(GetTradingSignRequest));

                x.AddRequestClient(typeof(GetByIdCorporateActionRequest));
                x.AddRequestClient(typeof(GetByIdCuratedFilterRequest));
                x.AddRequestClient(typeof(GetByIdCuratedListRequest));
                x.AddRequestClient(typeof(GetByIdCuratedMemberRequest));
                x.AddRequestClient(typeof(GetByIdFilterRequest));
                x.AddRequestClient(typeof(GetByIdFinancialRequest));
                x.AddRequestClient(typeof(GetByIdFundDetailRequest));
                x.AddRequestClient(typeof(GetByIdFundPerformanceRequest));
                x.AddRequestClient(typeof(GetByIdFundTradeDateRequest));
                x.AddRequestClient(typeof(GetByIdIndicatorRequest));
                x.AddRequestClient(typeof(GetByIdInstrumentRequest));
                x.AddRequestClient(typeof(GetByIdInstrumentDetailRequest));
                x.AddRequestClient(typeof(GetByIdIntermissionRequest));
                x.AddRequestClient(typeof(GetByIdMarketStatusRequest));
                x.AddRequestClient(typeof(GetByIdNavListRequest));
                x.AddRequestClient(typeof(GetByIdOrderBookRequest));
                x.AddRequestClient(typeof(GetByIdPriceInfoRequest));
                x.AddRequestClient(typeof(GetByIdPublicTradeRequest));
                x.AddRequestClient(typeof(GetByIdTradingSignRequest));

                x.AddRequestClient(typeof(CreateCorporateActionRequest));
                x.AddRequestClient(typeof(CreateCuratedFilterRequest));
                x.AddRequestClient(typeof(CreateCuratedListRequest));
                x.AddRequestClient(typeof(CreateCuratedMemberRequest));
                x.AddRequestClient(typeof(CreateFilterRequest));
                x.AddRequestClient(typeof(CreateFinancialRequest));
                x.AddRequestClient(typeof(CreateFundDetailRequest));
                x.AddRequestClient(typeof(CreateFundPerformanceRequest));
                x.AddRequestClient(typeof(CreateFundTradeDateRequest));
                x.AddRequestClient(typeof(CreateIndicatorRequest));
                x.AddRequestClient(typeof(CreateInstrumentRequest));
                x.AddRequestClient(typeof(CreateInstrumentDetailRequest));
                x.AddRequestClient(typeof(CreateIntermissionRequest));
                x.AddRequestClient(typeof(CreateMarketStatusRequest));
                x.AddRequestClient(typeof(CreateNavListRequest));
                x.AddRequestClient(typeof(CreateOrderBookRequest));
                x.AddRequestClient(typeof(CreatePriceInfoRequest));
                x.AddRequestClient(typeof(CreatePublicTradeRequest));
                x.AddRequestClient(typeof(CreateTradingSignRequest));

                x.AddRequestClient(typeof(UpdateCorporateActionRequest));
                x.AddRequestClient(typeof(UpdateCuratedFilterRequest));
                x.AddRequestClient(typeof(UpdateCuratedListRequest));
                x.AddRequestClient(typeof(UpdateCuratedMemberRequest));
                x.AddRequestClient(typeof(UpdateFilterRequest));
                x.AddRequestClient(typeof(UpdateFinancialRequest));
                x.AddRequestClient(typeof(UpdateFundDetailRequest));
                x.AddRequestClient(typeof(UpdateFundPerformanceRequest));
                x.AddRequestClient(typeof(UpdateFundTradeDateRequest));
                x.AddRequestClient(typeof(UpdateIndicatorRequest));
                x.AddRequestClient(typeof(UpdateInstrumentRequest));
                x.AddRequestClient(typeof(UpdateInstrumentDetailRequest));
                x.AddRequestClient(typeof(UpdateIntermissionRequest));
                x.AddRequestClient(typeof(UpdateMarketStatusRequest));
                x.AddRequestClient(typeof(UpdateNavListRequest));
                x.AddRequestClient(typeof(UpdateOrderBookRequest));
                x.AddRequestClient(typeof(UpdatePriceInfoRequest));
                x.AddRequestClient(typeof(UpdatePublicTradeRequest));
                x.AddRequestClient(typeof(UpdateTradingSignRequest));

                x.AddRequestClient(typeof(DeleteCorporateActionRequest));
                x.AddRequestClient(typeof(DeleteCuratedFilterRequest));
                x.AddRequestClient(typeof(DeleteCuratedListRequest));
                x.AddRequestClient(typeof(DeleteCuratedMemberRequest));
                x.AddRequestClient(typeof(DeleteFilterRequest));
                x.AddRequestClient(typeof(DeleteFinancialRequest));
                x.AddRequestClient(typeof(DeleteFundDetailRequest));
                x.AddRequestClient(typeof(DeleteFundPerformanceRequest));
                x.AddRequestClient(typeof(DeleteFundTradeDateRequest));
                x.AddRequestClient(typeof(DeleteIndicatorRequest));
                x.AddRequestClient(typeof(DeleteInstrumentRequest));
                x.AddRequestClient(typeof(DeleteInstrumentDetailRequest));
                x.AddRequestClient(typeof(DeleteIntermissionRequest));
                x.AddRequestClient(typeof(DeleteMarketStatusRequest));
                x.AddRequestClient(typeof(DeleteNavListRequest));
                x.AddRequestClient(typeof(DeleteOrderBookRequest));
                x.AddRequestClient(typeof(DeletePriceInfoRequest));
                x.AddRequestClient(typeof(DeletePublicTradeRequest));
                x.AddRequestClient(typeof(DeleteTradingSignRequest));

                x.AddRequestClient(typeof(PostBrokerInfoRequest));
                x.AddRequestClient(typeof(PostMarketProfileFinancialsRequest));
                x.AddRequestClient(typeof(PostMarketProfileFundamentalsRequest));
                x.AddRequestClient(typeof(PostMarketProfileOverViewRequest));
                x.AddRequestClient(typeof(PostMarketTickerRequest));
                x.AddRequestClient(typeof(PostMarketProfileDescriptionRequest));
                x.AddRequestClient(typeof(PostMarketTimelineRenderedRequest));
                x.AddRequestClient(typeof(PostMarketDerivativeInformationRequest));
                x.AddRequestClient(typeof(PostMarketProfileFundamentalsRequest));
                x.AddRequestClient(typeof(PostMarketFiltersRequest));
                x.AddRequestClient(typeof(PostMarketStatusRequest));
                x.AddRequestClient(typeof(PostMarketInstrumentSearchRequest));
                x.AddRequestClient(typeof(PostMarketInstrumentInfoRequest));
                x.AddRequestClient(typeof(PostMarketIndicatorRequest));
                x.AddRequestClient(typeof(PostHomeInstrumentsRequest));
                x.AddRequestClient(typeof(PostMarketFilterInstrumentsRequest));
                x.AddRequestClient(typeof(PostMarketGlobalEquityInstrumentInfoRequest));
                x.AddRequestClient(typeof(PostMarketInitialMarginRequest));
                x.AddRequestClient(typeof(PostOrderBookRequest));
            });
        });

        return services;
    }
}
