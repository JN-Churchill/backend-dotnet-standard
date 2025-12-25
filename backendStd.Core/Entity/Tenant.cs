using SqlSugar;

namespace backendStd.Core.Entity;

/// <summary>
/// 租户实体
/// </summary>
[SugarTable("sys_tenant")]
public class Tenant : Base.EntityBase
{
    /// <summary>
    /// 租户名称
    /// </summary>
    [SugarColumn(ColumnDescription = "租户名称", Length = 100)]
    public string TenantName { get; set; } = string.Empty;

    /// <summary>
    /// 租户编码
    /// </summary>
    [SugarColumn(ColumnDescription = "租户编码", Length = 50)]
    public string TenantCode { get; set; } = string.Empty;

    /// <summary>
    /// 联系人
    /// </summary>
    [SugarColumn(ColumnDescription = "联系人", Length = 50, IsNullable = true)]
    public string? ContactName { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(ColumnDescription = "联系电话", Length = 20, IsNullable = true)]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 联系邮箱
    /// </summary>
    [SugarColumn(ColumnDescription = "联系邮箱", Length = 100, IsNullable = true)]
    public string? ContactEmail { get; set; }

    /// <summary>
    /// 过期时间
    /// </summary>
    [SugarColumn(ColumnDescription = "过期时间", IsNullable = true)]
    public DateTime? ExpireTime { get; set; }

    /// <summary>
    /// 状态 0=禁用 1=启用
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 备注
    /// </summary>
    [SugarColumn(ColumnDescription = "备注", Length = 500, IsNullable = true)]
    public string? Remark { get; set; }
}
