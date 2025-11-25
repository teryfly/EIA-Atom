namespace EIA.S0.WebApi.Settings;

/// <summary>
/// 数据库配置.
/// </summary>
public class DatabaseSettings
{
    /// <summary>
    /// 数据库连接字符串.
    /// </summary>
    public required string ConnectionString { get; init; }

    /// <summary>
    /// 开启调试模式.
    /// </summary>
    public bool Debugger { get; set; }

    /// <summary>
    /// 引擎.
    /// </summary>
    public DatabaseEngine Engine { get; init; } = DatabaseEngine.PostgreSQL;
    
    /// <summary>
    /// sqlserver 配置.
    /// </summary>
    public SqlServerSettings? SqlServer { get; init; }

    /// <summary>
    /// PostgreSQL 配置.
    /// </summary>
    public PostgreSQLSettings? PostgreSQL { get; init; }
    
    /// <summary>
    /// engine.
    /// </summary>
    public enum DatabaseEngine
    {
        /// <summary>
        /// sqlserver.
        /// </summary>
        SqlServer,
        /// <summary>
        /// pg.
        /// </summary>
        PostgreSQL,
        /// <summary>
        /// 内存数据库(仅用作单元测试).
        /// </summary>
        InMemory
    }
    
    /// <summary>
    /// sqlserver 配置.
    /// </summary>
    public class SqlServerSettings
    {
        /// <summary>
        /// CompatibilityLevel.
        /// </summary>
        public int? CompatibilityLevel { get; init; }
    }
    
    /// <summary>
    /// pg配置.
    /// </summary>
    public class PostgreSQLSettings
    {
        /// <summary>
        /// 指定版本.
        /// </summary>
        public string? Version { get; init; }

        /// <summary>
        /// 用split query.
        /// </summary>
        public bool SplitQuery { get; set; } = true;
    }
}