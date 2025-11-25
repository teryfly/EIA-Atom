using EIA.S0.WebApi.Settings;
using EIA.S0.Application.Governance.DocTypes;
using EIA.S0.Application.Governance.Cache;
using EIA.S0.Application.Governance.Sync;
using EIA.S0.Domain.Core.Repositories;
using EIA.S0.Domain.Governance.Entities;
using EIA.S0.Infrastructure.Governance.Repositories;
using EIA.S0.Infrastructure.Messaging;
using Microsoft.Extensions.Logging;
using VZhen.Components.MessageQueue;
using VZhen.Components.MessageQueue.Kafka;

namespace EIA.S0.WebApi.Extensions;

/// <summary>
/// 基础设施服务.
/// </summary>
internal static class InfrastructureExtensions
{
    /// <summary>
    /// 基础设施服务.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="settings"></param>
    public static void AddInfrastructureService(this IServiceCollection services, AppSettings settings)
    {
        // Kafka IMessageQueue
        services.AddSingleton<IMessageQueue>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();
            // 绑定到 WebApi 的 KafkaSettings，然后转换为 VZhen 的 KafkaSettings
            var appKafkaSettings = configuration.GetSection("Kafka").Get<EIA.S0.WebApi.Settings.KafkaSettings>();
            if (appKafkaSettings == null)
            {
                throw new InvalidOperationException("Kafka settings is missing.");
            }

            var logger = sp.GetRequiredService<ILogger<KafkaProvider>>();
            var providerSettings = new VZhen.Components.MessageQueue.Kafka.KafkaSettings
            {
                BootstrapServers = appKafkaSettings.BootstrapServers,
                UserID = appKafkaSettings.UserID
            };

            var provider = new KafkaProvider(providerSettings, logger);
            provider.WriteEventLog();
            return provider;
        });

        // EventPublisher
        services.AddSingleton<EventPublisher>();

        // DocType 缓存
        services.AddSingleton<DocTypeCache>();

        // DocType 仓储
        services.AddScoped<IRepository<DocType>, DocTypeRepository>();

        // DocTypeService 注入事件发布委托
        services.AddScoped<DocTypeService>(sp =>
        {
            var repo = sp.GetRequiredService<IRepository<DocType>>();
            var uow = sp.GetRequiredService<IUnitOfWork>();
            var timeProvider = sp.GetRequiredService<TimeProvider>();
            var cache = sp.GetRequiredService<DocTypeCache>();
            var publisher = sp.GetRequiredService<EventPublisher>();

            Func<string, object, CancellationToken, Task> publishFunc =
                (topic, payload, token) => publisher.PublishAsync(topic, payload);

            return new DocTypeService(repo, uow, timeProvider, cache, publishFunc);
        });

        // SyncService
        services.AddScoped<SyncService>();
    }
}