using System.Text.Json;
using EIA.S0.Domain.Core.Aggregates;
using EIA.S0.Domain.Core.DomainEvents;
using EIA.S0.Domain.Core.Exceptions;
using EIA.S0.Domain.IntegrationEventLogs.Enums;

namespace EIA.S0.Domain.IntegrationEventLogs.Aggregates;

/// <summary>
/// 事件日志.
/// </summary>
public class IntegrationEventLog : AggregateRoot
{
    /// <summary>
    /// 空构造.
    /// </summary>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
    private IntegrationEventLog() { }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="event">领域事件.</param>
    public IntegrationEventLog(DomainEvent @event)
    {
        Id = @event.Id;
        CreateTime = @event.CreateTime;
        Sequence = @event.ConcurrentSort;
        var eventType = @event.GetType();
        EventName = eventType.Name;
        EventTypeName = eventType.FullName!;
        Identifier = @event.Identifier!;
        Content = JsonSerializer.Serialize(@event, eventType, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        State = IntegrationEventLogState.Pending;
        TimesSent = 0;
        DomainEvent = @event;
    }

    /// <summary>
    /// 事件名称.
    /// </summary>
    public string EventName { get; private set; }

    /// <summary>
    /// 全称.
    /// </summary>
    public string EventTypeName { get; private set; }

    /// <summary>
    /// 标识号.
    /// </summary>
    public string Identifier { get; private set; }

    /// <summary>
    /// 内容.
    /// </summary>
    public string Content { get; private set; }

    /// <summary>
    /// 创建时间.
    /// </summary>
    public DateTime CreateTime { get; private set; }

    /// <summary>
    /// 顺序.
    /// </summary>
    public int Sequence { get; private set; }

    /// <summary>
    /// 最后更新时间.
    /// </summary>
    public DateTime? LastUpdateTime { get; set; }

    /// <summary>
    /// 发送次数.
    /// </summary>
    public int TimesSent { get; set; }

    /// <summary>
    /// 状态.
    /// </summary>
    public IntegrationEventLogState State { get; set; }

    /// <summary>
    /// 出错信息.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// 事件.
    /// </summary>
    public DomainEvent DomainEvent { get; set; }

    /// <summary>
    /// 反序列化内容.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public IntegrationEventLog DeserializeJsonContent(Type type)
    {
        DomainEvent = (JsonSerializer.Deserialize(Content, type, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) as DomainEvent)!;
        return this;
    }

    /// <summary>
    /// 是否已发布.
    /// </summary>
    /// <returns></returns>
    public bool IsPublished()
    {
        return this.State == IntegrationEventLogState.Published;
    }

    public void InProgress(DateTime occurredOn)
    {
        if (IsPublished())
        {
            throw new DomainException($"已推送的事件不可修改状态到{IntegrationEventLogState.InProgress}.");
        }

        this.State = IntegrationEventLogState.InProgress;
        this.TimesSent += 1;
        this.LastUpdateTime = occurredOn;
    }

    /// <summary>
    /// 发送成功.
    /// </summary>
    /// <exception cref="Exceptions.ResourcePoolDomainException"></exception>
    public void Published(DateTime occurredOn)
    {
        if (IsPublished())
        {
            throw new DomainException($"已推送的事件不可修改状态到{IntegrationEventLogState.Published}.");
        }

        this.State = IntegrationEventLogState.Published;
        this.TimesSent += 1;
        this.LastUpdateTime = occurredOn;
    }

    /// <summary>
    /// 发送失败.
    /// </summary>
    public void InError(string error, DateTime occurredOn)
    {
        if (IsPublished())
        {
            throw new DomainException($"已推送的事件不可修改状态到{IntegrationEventLogState.InError}.");
        }

        this.State = IntegrationEventLogState.InError;
        this.TimesSent += 1;
        this.LastUpdateTime = occurredOn;
        this.Error = error;
    }
}
