using SqlSugar;

namespace backendStd.Core.Entity.Base;

/// <summary>
/// 实体基类
/// 包含Id、创建时间、更新时间、创建人、更新人、软删除等审计字段
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// 主键ID（雪花ID）
    /// </summary>
    [SugarColumn(ColumnDescription = "主键ID", IsPrimaryKey = true, IsIdentity = false)]
    public long Id { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnDescription = "创建时间", IsOnlyIgnoreUpdate = true)]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnDescription = "更新时间")]
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 创建人ID
    /// </summary>
    [SugarColumn(ColumnDescription = "创建人ID", IsOnlyIgnoreUpdate = true)]
    public long? CreateUserId { get; set; }

    /// <summary>
    /// 更新人ID
    /// </summary>
    [SugarColumn(ColumnDescription = "更新人ID")]
    public long? UpdateUserId { get; set; }

    /// <summary>
    /// 是否删除（软删除标记）
    /// </summary>
    [SugarColumn(ColumnDescription = "是否删除")]
    public bool IsDeleted { get; set; }

    /// <summary>
    /// 删除时间
    /// </summary>
    [SugarColumn(ColumnDescription = "删除时间", IsNullable = true)]
    public DateTime? DeleteTime { get; set; }
}
