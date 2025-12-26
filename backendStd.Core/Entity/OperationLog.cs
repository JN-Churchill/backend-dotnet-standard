using SqlSugar;

namespace backendStd.Core.Entity;

/// <summary>
/// 操作日志实体
/// </summary>
[SugarTable("sys_operation_log")]
public class OperationLog : Base.EntityBase
{
    /// <summary>
    /// 操作用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "操作用户ID")]
    public long UserId { get; set; }
    
    /// <summary>
    /// 操作用户名
    /// </summary>
    [SugarColumn(ColumnDescription = "操作用户名", Length = 50)]
    public string UserName { get; set; } = string.Empty;
    
    /// <summary>
    /// 操作模块
    /// </summary>
    [SugarColumn(ColumnDescription = "操作模块", Length = 100)]
    public string Module { get; set; } = string.Empty;
    
    /// <summary>
    /// 操作类型
    /// </summary>
    [SugarColumn(ColumnDescription = "操作类型", Length = 50)]
    public string OperationType { get; set; } = string.Empty;
    
    /// <summary>
    /// 操作描述
    /// </summary>
    [SugarColumn(ColumnDescription = "操作描述", Length = 500)]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// 请求路径
    /// </summary>
    [SugarColumn(ColumnDescription = "请求路径", Length = 500)]
    public string RequestPath { get; set; } = string.Empty;
    
    /// <summary>
    /// 请求方法
    /// </summary>
    [SugarColumn(ColumnDescription = "请求方法", Length = 10)]
    public string RequestMethod { get; set; } = string.Empty;
    
    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(ColumnDescription = "请求参数", ColumnDataType = "text", IsNullable = true)]
    public string? RequestParams { get; set; }
    
    /// <summary>
    /// IP地址
    /// </summary>
    [SugarColumn(ColumnDescription = "IP地址", Length = 50)]
    public string IpAddress { get; set; } = string.Empty;
    
    /// <summary>
    /// 执行时长(ms)
    /// </summary>
    [SugarColumn(ColumnDescription = "执行时长(ms)")]
    public long Duration { get; set; }
    
    /// <summary>
    /// 执行结果 0=失败 1=成功
    /// </summary>
    [SugarColumn(ColumnDescription = "执行结果 0=失败 1=成功")]
    public int Result { get; set; }
    
    /// <summary>
    /// 错误信息
    /// </summary>
    [SugarColumn(ColumnDescription = "错误信息", Length = 2000, IsNullable = true)]
    public string? ErrorMessage { get; set; }
}
