using SqlSugar;

namespace backendStd.Core.Options;

/// <summary>
/// 数据库连接配置选项
/// </summary>
public class DbConnectionOptions
{
    /// <summary>
    /// 数据库连接配置列表
    /// </summary>
    public List<DbConnectionConfig> ConnectionConfigs { get; set; }
}

/// <summary>
/// 单个数据库连接配置
/// </summary>
public class DbConnectionConfig
{
    /// <summary>
    /// 配置ID（用于区分多数据库）
    /// </summary>
    public string ConfigId { get; set; }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public string DbType { get; set; }

    /// <summary>
    /// 连接字符串
    /// </summary>
    public string ConnectionString { get; set; }

    /// <summary>
    /// 是否自动关闭连接
    /// </summary>
    public bool IsAutoCloseConnection { get; set; } = true;

    /// <summary>
    /// 是否启用初始化数据库
    /// </summary>
    public bool EnableInitDb { get; set; }

    /// <summary>
    /// 是否启用差异日志
    /// </summary>
    public bool EnableDiffLog { get; set; }

    /// <summary>
    /// 是否启用SQL日志
    /// </summary>
    public bool EnableSqlLog { get; set; }
}
