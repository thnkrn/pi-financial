#nullable enable
using System.Linq.Expressions;
using Pi.Common.Domain;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Filters;

public class FundFilter : IQueryFilter<Fund>
{
    public string[]? Categories { get; set; }
    public string[]? TaxTypes { get; set; }
    public string[]? AmcCodes { get; set; }
    public int[]? RiskLevels { get; set; }
    public bool? Dividend { get; set; }

    public List<Expression<Func<Fund, bool>>> GetExpressions()
    {
        var expressions = new List<Expression<Func<Fund, bool>>>();

        if (Categories is { Length: > 0 })
        {
            expressions.Add(q => Categories.Select(q => q.ToUpper()).Contains(q.Category.ToUpper()));
        }

        if (TaxTypes is { Length: > 0 })
        {
            expressions.Add(q => TaxTypes.Select(q => q.ToUpper()).Contains(q.Fundamental.TaxType.ToUpper()));
        }

        if (AmcCodes is { Length: > 0 })
        {
            expressions.Add(q => AmcCodes.Select(q => q.ToUpper()).Contains(q.AmcCode.ToUpper()));
        }

        if (RiskLevels is { Length: > 0 })
        {
            if (RiskLevels.Any(q => q > 8))
            {
                expressions.Add(q => RiskLevels.Contains(q.Fundamental.RiskLevel) || q.Fundamental.RiskLevel > 8);
            }
            else
            {
                expressions.Add(q => RiskLevels.Contains(q.Fundamental.RiskLevel));
            }
        }

        if (Dividend != null)
        {
            expressions.Add(q => q.Fundamental.IsDividend == Dividend);
        }

        return expressions;
    }
}
