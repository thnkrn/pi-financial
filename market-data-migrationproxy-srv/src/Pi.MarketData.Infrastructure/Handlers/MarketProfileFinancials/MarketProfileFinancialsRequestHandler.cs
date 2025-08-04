using Pi.MarketData.Application.Interfaces.MarketProfileFinancials;
using Pi.MarketData.Application.Queries.MarketProfileFinancials;
using Pi.MarketData.Application.Services.MarketData.MarketProfileFinancials;
using Pi.MarketData.Domain.Models.Response.MorningStar;
using Pi.MarketData.Infrastructure.Interfaces.Mongo;

namespace Pi.MarketData.Infrastructure.Handlers.MarketProfileFinancials;

public class MarketProfileFinancialsRequestHandler
    : GetMarketProfileFinancialsRequestAbstractHandler
{
    private readonly IMongoService<BalanceSheet> _balanceSheetService;
    private readonly IMongoService<IncomeStatement> _incomeStatementService;
    private readonly int _limited;
    private readonly IMongoService<ProfitabilityRatios> _profitabilityRatiosService;
    private readonly IMongoService<SegmentSheet> _segmentSheetService;
    private readonly IMongoService<ValuationRations> _valuationRationsService;

    public MarketProfileFinancialsRequestHandler(
        IMongoService<IncomeStatement> incomeStatementService,
        IMongoService<ValuationRations> valuationRationsService,
        IMongoService<BalanceSheet> balanceSheetService,
        IMongoService<ProfitabilityRatios> profitabilityRatiosService,
        IMongoService<SegmentSheet> segmentSheetService
    )
    {
        _incomeStatementService = incomeStatementService;
        _valuationRationsService = valuationRationsService;
        _balanceSheetService = balanceSheetService;
        _profitabilityRatiosService = profitabilityRatiosService;
        _segmentSheetService = segmentSheetService;
        _limited = 5;
    }

    protected override async Task<PostMarketProfileFinancialsResponse> Handle(
        PostMarketProfileFinancialsRequest request,
        CancellationToken cancellationToken
    )
    {
        IncomeStatement? _incomeStatementList;
        ValuationRations? _valuationRations;
        BalanceSheet? _balanceSheet;
        ProfitabilityRatios? _profitabilityRatios;
        SegmentSheet? _segmentSheet;

        try
        {
            _incomeStatementList = await _incomeStatementService.GetMongoBySymbolAsync(
                request.data.Symbol ?? ""
            );
            _valuationRations = await _valuationRationsService.GetMongoBySymbolAsync(
                request.data.Symbol ?? ""
            );
            _balanceSheet = await _balanceSheetService.GetMongoBySymbolAsync(
                request.data.Symbol ?? ""
            );
            _profitabilityRatios = await _profitabilityRatiosService.GetMongoBySymbolAsync(
                request.data.Symbol ?? ""
            );
            _segmentSheet = await _segmentSheetService.GetMongoBySymbolAsync(
                request.data.Symbol ?? ""
            );

            var result = MarketProfileFinancialsService.GetResult(
                _incomeStatementList,
                _valuationRations,
                _balanceSheet,
                _profitabilityRatios,
                _segmentSheet,
                _limited
            );

            return new PostMarketProfileFinancialsResponse(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}