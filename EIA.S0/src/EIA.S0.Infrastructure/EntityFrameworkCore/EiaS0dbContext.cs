using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using EIA.S0.Domain.Core.Aggregates;
using EIA.S0.Domain.Core.DomainEvents;
using EIA.S0.Domain.IntegrationEventLogs.Aggregates;

namespace EIA.S0.Infrastructure.EntityFrameworkCore;

/// <summary>
/// whyfate context.
/// </summary>
public class EiaS0dbContext : DbContext
{
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly ConcurrentDictionary<Guid, List<DomainEvent>> _transEvents = new();
    private readonly ILogger<EiaS0dbContext> _logger;

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="dispatcher"></param>
    /// <param name="logger"></param>
    public EiaS0dbContext(
        DbContextOptions<EiaS0dbContext> options,
        IDomainEventDispatcher dispatcher,
        ILogger<EiaS0dbContext> logger)
        : base(options)
    {
        _dispatcher = dispatcher;
        _logger = logger;
    }

    public DbSet<IntegrationEventLog> IntegrationEventLogs => Set<IntegrationEventLog>();

    /// <summary>
    /// 模型创建.
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 处理聚合.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EiaS0dbContext).Assembly);
        
        // 以下代码要写在处理聚合之前.
        // 找到所有实体类型
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            // 找到 DomainEvents 属性
            var domainEventsProperty = entityType.ClrType.GetProperty("DomainEvents");
            if (domainEventsProperty != null)
            {
                modelBuilder
                    .Entity(entityType.ClrType)
                    .Ignore("DomainEvents");
            }
        }
        
        // 处理时间
        var datetimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc).ToLocalTime()
        );
        foreach (var property in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?)))
        {
            property.SetValueConverter(datetimeConverter);
        }
    }
    
    /// <summary>
    /// SaveChangesAsync.
    /// </summary>
    /// <returns></returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 将 DomainEvent 添加到 IntegrationEventLogEntry
        var entities = this.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();
        var domainEvents = entities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        entities.ToList()
            .ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _dispatcher.PreDispatchAsync(domainEvent, this);
        }

        var i = await base.SaveChangesAsync(cancellationToken);

        if (domainEvents.Any())
        {
            // 不是事务提交.
            if (this.Database.CurrentTransaction == null)
            {
                try
                {
                    foreach (var domainEvent in domainEvents)
                    {
                        await _dispatcher.PostDispatchAsync(domainEvent);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "领域事件发布失败.");
                }
            }
            else
            {
                if (!_transEvents.ContainsKey(this.Database.CurrentTransaction.TransactionId))
                {
                    _transEvents.TryAdd(this.Database.CurrentTransaction.TransactionId, domainEvents);
                }
                else
                {
                    _transEvents[this.Database.CurrentTransaction.TransactionId].AddRange(domainEvents);
                }
            }
        }

        return i;
    }

    /// <summary>
    /// 事务.
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> TransactionAsync(Func<CancellationToken, Task> func,
        CancellationToken cancellationToken = default)
    {
        var success = false;
        await using var trans = await this.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await func(cancellationToken);
            await trans.CommitAsync(cancellationToken);
            success = true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "事务报错.");
            try
            {
                await trans.RollbackAsync(cancellationToken);
            }
            catch (Exception rollbackEx)
            {
                _logger.LogError(rollbackEx, "事务回滚报错.");
            }
        }
        finally
        {
            // 处理领域事件
            if (_transEvents.ContainsKey(trans.TransactionId))
            {
                try
                {
                    if (_transEvents.Remove(trans.TransactionId, out var transEvents) && success)
                    {
                        foreach (var domainEvent in transEvents)
                        {
                            await _dispatcher.PostDispatchAsync(domainEvent);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "领域事件发布失败.");
                }
            }
        }

        return success;
    }

}