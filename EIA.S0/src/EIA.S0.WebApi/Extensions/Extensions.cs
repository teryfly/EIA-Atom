using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;
using EIA.S0.Application.Common.Behaviors;
using EIA.S0.Domain.Core.DomainEvents;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Core.Services;
using EIA.S0.Infrastructure.DomainEventHandlers;
using EIA.S0.Infrastructure.EntityFrameworkCore;
using EIA.S0.Infrastructure.EntityFrameworkCore.PostgreSQL;
using EIA.S0.Infrastructure.EntityFrameworkCore.Repositories;
using EIA.S0.WebApi.Initializers;
using EIA.S0.WebApi.Settings;

namespace EIA.S0.WebApi.Extensions;

internal static class Extensions
{
    /// <summary>
    /// 添加应用服务.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="settings"></param>
    /// <exception cref="Exception"></exception>
    public static void AddApplicationServices(this IServiceCollection services, AppSettings settings)
    {
        // 注入 TimeProvider
        services.AddSingleton(TimeProvider.System);

        // 注入 UnitOfWork
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // 注入 DomainEventDispatcher
        if (settings.DomainEventDispatch.HandleType == DomainEventDispatchSettings.DomainEventDispatchHandleType.Async)
        {
            services.AddSingleton<IDomainEventDispatcher, AsyncDomainEventDispatcher>();
        }
        else
        {
            services.AddSingleton<IDomainEventDispatcher, SyncDomainEventDispatcher>();
        }

        // 注入数据库
        services.AddDbContext<EiaS0dbContext>(options =>
        {
            // 启用调试模式.
            if (settings.Database.Debugger)
            {
                options
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging();
            }

            if (settings.Database.Engine == DatabaseSettings.DatabaseEngine.PostgreSQL)
            {
                options.UseNpgsql(settings.Database.ConnectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsAssembly(typeof(PostgreSqlMigrator).Assembly.FullName);
                    if (settings.Database.PostgreSQL != null)
                    {
                        var postgreSqlSettings = settings.Database.PostgreSQL;
                        if (!string.IsNullOrWhiteSpace(postgreSqlSettings.Version))
                        {
                            npgsqlOptions.SetPostgresVersion(Version.Parse(postgreSqlSettings.Version));
                        }

                        if (postgreSqlSettings.SplitQuery)
                        {
                            npgsqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                        }
                    }
                });
            }
            else if (settings.Database.Engine == DatabaseSettings.DatabaseEngine.InMemory)
            {
                options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                options.UseInMemoryDatabase(settings.Database.ConnectionString);
            }
            else
            {
                throw new Exception("数据库配置有误.");
            }
        });

        // 配置 mediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining(typeof(LoggingBehavior<,>));

            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        });

        // 配置验证.
        services.AddServiceFromAssembly(typeof(LoggingBehavior<,>).Assembly, typeof(IValidator<>),
            ServiceLifetime.Singleton, true);

        // 注入领域服务
        services.AddServiceFromAssembly(typeof(IDomainService).Assembly, typeof(IDomainService));

        // 注入领域事件处理器
        services.AddServiceFromAssembly(typeof(LoggingBehavior<,>).Assembly, typeof(IDomainEventHandler));

        // 注入仓储服务
        services.AddServiceFromAssembly(typeof(EiaS0dbContext).Assembly, typeof(IRepository<>));

        // 注入worker
        services.AddServiceFromAssembly(typeof(Program).Assembly, typeof(IHostedService), ServiceLifetime.Singleton);

        // 注入应用初始化
        services.AddServiceFromAssembly(typeof(Program).Assembly, typeof(IInitializer), ServiceLifetime.Transient);

        // 添加基础服务.
        services.AddInfrastructureService(settings);
    }

    /// <summary>
    /// 自动注入服务.
    /// </summary>
    /// <param name="services">builder.</param>
    /// <param name="assembly">程序集.</param>
    /// <param name="baseInterface">公共接口.</param>
    /// <param name="lifetime">生命周期.</param>
    /// <param name="implementationIsGenericType">实现类是否是泛型.</param>
    private static void AddServiceFromAssembly(this IServiceCollection services, Assembly assembly,
        Type baseInterface,
        ServiceLifetime lifetime = ServiceLifetime.Scoped, bool implementationIsGenericType = false)
    {
        // 找出所有的实现类.
        var implementations = assembly
            .GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false });

        // 如果接口是泛型，根据interfaces的泛型查询，否则接口就是本身.
        if (baseInterface.IsGenericType)
        {
            implementations = implementations.Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseInterface));
        }
        else
        {
            implementations = implementations.Where(t => t.GetInterfaces()
                .Any(i => i == baseInterface));
        }

        foreach (var impl in implementations)
        {
            // 找到实现类中要注入的接口.
            var @interface = baseInterface;

            try
            {
                // 如果base接口是泛型
                if (baseInterface.IsGenericType)
                {
                    // 实现类也是泛型，就找泛型的接口，否则就找不是泛型，但是接口的接口是泛型的
                    if (implementationIsGenericType)
                    {
                        @interface = impl.GetInterfaces()
                            .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == baseInterface);
                    }
                    else
                    {
                        @interface = impl.GetInterfaces().First(i =>
                            !i.IsGenericType &&
                            i.GetInterfaces().Any(g =>
                                g.IsGenericType && g.GetGenericTypeDefinition() == baseInterface));
                    }
                }
                // 如果不是泛型，就从接口中找不是base的接口，如果不存在就是base
                else
                {
                    var candidate = impl.GetInterfaces().FirstOrDefault(i => i != baseInterface
                                                                            && i != typeof(IDisposable)
                                                                            && i != typeof(IAsyncDisposable));
                    if (candidate != null)
                    {
                        @interface = candidate;
                    }
                }
            }
            catch
            {
                // 如果未找到匹配接口则跳过该实现，避免 First 抛异常
                continue;
            }

            services.Add(new ServiceDescriptor(@interface, impl, lifetime));
        }
    }
}