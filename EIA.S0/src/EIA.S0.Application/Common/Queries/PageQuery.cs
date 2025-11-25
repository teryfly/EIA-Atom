using MediatR;

namespace EIA.S0.Application.Common.Queries;

/// <summary>
/// 分页查询.
/// </summary>
public class PageQuery<TResponse> : IRequest<TResponse>
{
    /// <summary>
    /// page index.
    /// </summary>
    public int PageIndex { get; init; } = 1;

    /// <summary>
    /// page size.
    /// </summary>
    public int PageSize { get; init; } = 20;
}