using SqlSugar;

namespace backendStd.Core.Entity;

/// <summary>
/// 部门实体
/// </summary>
[SugarTable("sys_department")]
public class Department : Base.EntityBase
{
    /// <summary>
    /// 部门名称
    /// </summary>
    [SugarColumn(ColumnDescription = "部门名称", Length = 50)]
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// 部门编码
    /// </summary>
    [SugarColumn(ColumnDescription = "部门编码", Length = 50)]
    public string DepartmentCode { get; set; } = string.Empty;

    /// <summary>
    /// 父级部门ID
    /// </summary>
    [SugarColumn(ColumnDescription = "父级部门ID")]
    public long ParentId { get; set; } = 0;

    /// <summary>
    /// 部门级别（用于树形结构）
    /// </summary>
    [SugarColumn(ColumnDescription = "部门级别")]
    public int Level { get; set; } = 1;

    /// <summary>
    /// 部门路径（用于查找子部门）
    /// </summary>
    [SugarColumn(ColumnDescription = "部门路径", Length = 500, IsNullable = true)]
    public string? DepartmentPath { get; set; }

    /// <summary>
    /// 负责人
    /// </summary>
    [SugarColumn(ColumnDescription = "负责人", Length = 50, IsNullable = true)]
    public string? Leader { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(ColumnDescription = "联系电话", Length = 20, IsNullable = true)]
    public string? Phone { get; set; }

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
