using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Pi.BackofficeService.Domain.SeedWork;

namespace Pi.BackofficeService.Infrastructure.Extensions;

public static class DbQueryExtension
{
    public static IQueryable<TSource> OrderByProperty<TSource>(this IQueryable<TSource> source, string orderBy, string orderDir)
    {
        var type = typeof(TSource).GetProperty(orderBy)?.PropertyType;

        return type switch
        {
            not null when type == typeof(string) => OrderByDir<TSource, string>(source, orderBy, orderDir),
            not null when type == typeof(int) => OrderByDir<TSource, int>(source, orderBy, orderDir),
            not null when type == typeof(float) => OrderByDir<TSource, float>(source, orderBy, orderDir),
            not null when type == typeof(decimal) => OrderByDir<TSource, decimal>(source, orderBy, orderDir),
            not null when type == typeof(DateTime) => OrderByDir<TSource, DateTime>(source, orderBy, orderDir),
            _ => source
        };
    }

    public static IOrderedQueryable<TSource> OrderByDir<TSource, TKey>(this IQueryable<TSource> source, string orderBy, string orderDir)
    {
        Expression<Func<TSource, TKey>> expression = q => EF.Property<TKey>(q!, orderBy);

        return orderDir.ToLower() == "desc" ? source.OrderByDescending(expression) : source.OrderBy(expression);
    }

    public static IQueryable<TSource> WhereByFilters<TSource>(this IQueryable<TSource> source, IQueryFilter<TSource>? filters) where TSource : class
    {
        return filters == null ? source : filters.GetExpressions().Aggregate(source, (current, expression) => current.Where(expression));
    }
}
