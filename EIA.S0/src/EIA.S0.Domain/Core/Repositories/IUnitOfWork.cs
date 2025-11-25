namespace EIA.S0.Domain.Core.Repositories;

/// <summary>
/// unit of work.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Save Changes.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 事务.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<bool> TransactionAsync(Func<CancellationToken, Task> func, CancellationToken cancellationToken = default);
}