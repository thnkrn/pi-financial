using System.Linq.Expressions;

namespace Pi.WalletService.Domain;

public interface IQueryFilter<T> where T : class
{
    List<Expression<Func<T, bool>>> GetExpressions();
}
