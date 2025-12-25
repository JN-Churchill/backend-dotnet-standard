using SqlSugar;

namespace backendStd.Core.Entity.Base;

/// <summary>
/// 多租户实体基类
/// 在EntityBase基础上增加租户ID字段
/// </summary>
public abstract class EntityTenantBase : EntityBase
{
    /// <summary>
    /// 租户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "租户ID", IsOnlyIgnoreUpdate = true)]
    public long? TenantId { get; set; }
}
