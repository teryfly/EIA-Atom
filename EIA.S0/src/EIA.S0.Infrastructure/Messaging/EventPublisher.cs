using System.Text.Json;
using Microsoft.Extensions.Logging;
using VZhen.Components.MessageQueue;

namespace EIA.S0.Infrastructure.Messaging;

/// <summary>
/// 事件发布器，封装 IMessageQueue（Kafka 实现）.
/// </summary>
public class EventPublisher
{
    private readonly IMessageQueue _messageQueue;
    private readonly ILogger<EventPublisher> _logger;

    /// <summary>
    /// 构造.
    /// </summary>
    public EventPublisher(IMessageQueue messageQueue, ILogger<EventPublisher> logger)
    {
        _messageQueue = messageQueue;
        _logger = logger;
    }

    /// <summary>
    /// 将消息发布到指定 topic.
    /// </summary>
    /// <typeparam name="TValue">消息类型（需要可序列化、带无参构造）。</typeparam>
    /// <param name="eventCode">Kafka topic.</param>
    /// <param name="value">消息内容.</param>
    public async Task PublishAsync<TValue>(string eventCode, TValue value)
        where TValue : class, new()
    {
        try
        {
            await _messageQueue.PublishAsync(eventCode, value);
            _logger.LogInformation("Published event {EventCode}: {Payload}",
                eventCode, JsonSerializer.Serialize(value));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {EventCode}", eventCode);
            throw;
        }
    }
}