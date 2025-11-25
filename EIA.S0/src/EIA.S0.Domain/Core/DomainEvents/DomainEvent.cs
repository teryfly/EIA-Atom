using System.Text.Json.Serialization;

namespace EIA.S0.Domain.Core.DomainEvents;

public abstract class DomainEvent
{
    /// <summary>
    /// 空构造.
    /// </summary>
    public DomainEvent()
    {
        this.Id = Guid.CreateVersion7().ToString("N");
        this.CreateTime = DateTime.Now;
    }

    /// <summary>
    /// .
    /// </summary>
    /// <param name="identifier">identifier.</param>
    /// <param name="createTime">create time.</param>
    public DomainEvent(string identifier, DateTime createTime)
        : this(Guid.CreateVersion7().ToString("N"), identifier, createTime)
    {
    }

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="id">id.</param>
    /// <param name="identifier">identifier.</param>
    /// <param name="createTime">create time.</param>
    public DomainEvent(string id, string identifier, DateTime createTime)
    {
        Id = id;
        Identifier = identifier;
        CreateTime = createTime;
    }

    /// <summary>
    /// id.
    /// </summary>
    [JsonInclude]
    public string Id { get; init; }

    /// <summary>
    /// identifier.
    /// </summary>
    [JsonInclude]
    public string? Identifier { get; init; }

    /// <summary>
    /// create time.
    /// </summary>
    [JsonInclude]
    public DateTime CreateTime { get; init; }

    /// <summary>
    /// concurrent sort.
    /// </summary>
    [JsonInclude]
    public int ConcurrentSort { get; private set; }

    /// <summary>
    /// set sort.
    /// </summary>
    /// <param name="sort"></param>
    public void SetSort(int sort)
    {
        ConcurrentSort = sort;
    }
}