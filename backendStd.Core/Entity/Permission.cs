using SqlSugar;

namespace backendStd.Core.Entity;

/// <summary>
/// 权限实体
/// </summary>
[SugarTable("sys_permission")]
public class Permission : Base.EntityBase
{
    /// <summary>
    /// 权限名称
    /// </summary>
    [SugarColumn(ColumnDescription = "权限名称", Length = 50)]
    public string PermissionName { get; set; } = string.Empty;

    /// <summary>
    /// 权限编码
    /// </summary>
    [SugarColumn(ColumnDescription = "权限编码", Length = 100)]
    public string PermissionCode { get; set; } = string.Empty;

    /// <summary>
    /// 权限类型 1=菜单 2=按钮
    /// </summary>
    [SugarColumn(ColumnDescription = "权限类型")]
    public int PermissionType { get; set; } = 1;

    /// <summary>
    /// 父级权限ID
    /// </summary>
    [SugarColumn(ColumnDescription = "父级权限ID")]
    public long ParentId { get; set; } = 0;

    /// <summary>
    /// 路由路径
    /// </summary>
    [SugarColumn(ColumnDescription = "路由路径", Length = 200, IsNullable = true)]
    public string? Path { get; set; }

    /// <summary>
    /// 组件路径
    /// </summary>
    [SugarColumn(ColumnDescription = "组件路径", Length = 200, IsNullable = true)]
    public string? Component { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    [SugarColumn(ColumnDescription = "图标", Length = 100, IsNullable = true)]
    public string? Icon { get; set; }

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
