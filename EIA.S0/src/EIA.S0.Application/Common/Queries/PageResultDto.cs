namespace EIA.S0.Application.Common.Queries;

/// <summary>
/// 分页查询结果.
/// </summary>
/// <typeparam name="T"></typeparam>
public class PageResultDto<T>
    where T : class
{
    /// <summary>
    /// 总条数.
    /// </summary>
    public int Total { get; set; }
    
    /// <summary>
    /// 返回的行数.
    /// </summary>
    public List<T>? Rows { get; set; }
}