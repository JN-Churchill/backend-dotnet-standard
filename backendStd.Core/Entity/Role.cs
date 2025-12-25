using SqlSugar;

namespace backendStd.Core.Entity;

/// <summary>
/// 角色实体
/// </summary>
[SugarTable("sys_role")]
public class Role : Base.EntityBase
{
    /// <summary>
    /// 角色名称
    /// </summary>
    [SugarColumn(ColumnDescription = "角色名称", Length = 50)]
    public string RoleName { get; set; } = string.Empty;

    /// <summary>
    /// 角色编码
    /// </summary>
    [SugarColumn(ColumnDescription = "角色编码", Length = 50)]
    public string RoleCode { get; set; } = string.Empty;

    /// <summary>
    /// 角色描述
    /// </summary>
    [SugarColumn(ColumnDescription = "角色描述", Length = 500, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    [SugarColumn(ColumnDescription = "排序")]
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态 0=禁用 1=启用
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;
}
