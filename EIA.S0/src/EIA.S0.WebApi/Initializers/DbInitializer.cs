
using Microsoft.EntityFrameworkCore;
using EIA.S0.Infrastructure.EntityFrameworkCore;

namespace EIA.S0.WebApi.Initializers;

/// <summary>
/// 数据库初始化.
/// </summary>
public class DbInitializer : IInitializer
{
    private readonly EiaS0dbContext _context;

    /// <summary>
    /// 构造.
    /// </summary>
    /// <param name="context"></param>
    public DbInitializer(EiaS0dbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 初始化.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        await _context.Database.MigrateAsync();
    }
}
