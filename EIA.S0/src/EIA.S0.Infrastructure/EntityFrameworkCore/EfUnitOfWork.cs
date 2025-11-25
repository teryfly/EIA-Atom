using EIA.S0.Domain.Core.Repositories;

namespace EIA.S0.Infrastructure.EntityFrameworkCore.Repositories;

/// <summary>
/// unit of work.
/// </summary>
public class EfUnitOfWork : IUnitOfWork
{
    private readonly EiaS0dbContext _context;

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="context"></param>
    public EfUnitOfWork(EiaS0dbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> TransactionAsync(Func<CancellationToken, Task> func,
        CancellationToken cancellationToken = default)
    {
        var strategy = _context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(
            state: func,
            operation: async (dbContext, operationState, ct) =>
                await ((EiaS0dbContext)dbContext).TransactionAsync(operationState, ct),
            verifySucceeded: null,
            cancellationToken: cancellationToken
        );
    }
}