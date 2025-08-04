using Pi.MarketData.Application.Interfaces.MarketProfileFundamentals;
using Pi.MarketData.Application.Queries.MarketProfileFundamentals;
using Pi.MarketData.Application.Services.MarketData.MarketProfileFundamentals;
using Pi.MarketData.Domain.Models.Response.MorningStar;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.MarketProfileFundamentals;

public class MarketProfileFundamentalsRequestHandler
    : GetMarketProfileFundamentalsRequestAbstractHandler
{
    private readonly IMongoService<CashDividend> _cashDividendService;
    private readonly IMongoService<CompanyGeneralInformation> _companyGeneralInformationService;
    private readonly IMongoService<CurrentMarketCapitalization> _currentMarketCapitalizationService;
    private readonly IMongoService<SegmentSheet> _segmentSheetService;
    private readonly IMongoService<ValuationRations> _valuationRationsService;

    public MarketProfileFundamentalsRequestHandler(
        IMongoService<CurrentMarketCapitalization> currentMarketCapitalizationService,
        IMongoService<ValuationRations> valuationRationsService,
        IMongoService<CashDividend> cashDividendService,
        IMongoService<SegmentSheet> segmentSheetService,
        IMongoService<CompanyGeneralInformation> companyGeneralInformationService
    )
    {
        _currentMarketCapitalizationService = currentMarketCapitalizationService;
        _valuationRationsService = valuationRationsService;
        _cashDividendService = cashDividendService;
        _segmentSheetService = segmentSheetService;
        _companyGeneralInformationService = companyGeneralInformationService;
    }

    protected override async Task<PostMarketProfileFundamentalsResponse> Handle(
        PostMarketProfileFundamentalsRequest request,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var _currentMarketCapitalization =
                await _currentMarketCapitalizationService.GetMongoBySymbolAsync(
                    request.data.Symbol ?? ""
                );

            var _valuationRations = await _valuationRationsService.GetMongoBySymbolAsync(
                request.data.Symbol ?? ""
            );

            var _cashDividends = await _cashDividendService.GetMongoBySymbolAsync(
                request.data.Symbol ?? ""
            );

            var _segmentSheet = await _segmentSheetService.GetMongoBySymbolAsync(
                request.data.Symbol ?? ""
            );

            var _companyGeneralInformation =
                await _companyGeneralInformationService.GetMongoBySymbolAsync(
                    request.data.Symbol ?? ""
                );

            var result = MarketProfileFundamentalsService.GetResult(
                _currentMarketCapitalization,
                _valuationRations,
                _cashDividends,
                _segmentSheet,
                _companyGeneralInformation
            );

            return new PostMarketProfileFundamentalsResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}