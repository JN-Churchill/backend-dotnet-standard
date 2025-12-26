namespace backendStd.Application.Dtos.Department;

/// <summary>
/// 部门DTO
/// </summary>
public class DepartmentDto
{
    /// <summary>
    /// 部门ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DepartmentName { get; set; } = string.Empty;

    /// <summary>
    /// 部门编码
    /// </summary>
    public string DepartmentCode { get; set; } = string.Empty;

    /// <summary>
    /// 父级部门ID
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 部门级别
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// 部门路径
    /// </summary>
    public string? DepartmentPath { get; set; }

    /// <summary>
    /// 负责人
    /// </summary>
    public string? Leader { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 状态 0=禁用 1=启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 子部门列表（用于树形结构）
    /// </summary>
    public List<DepartmentDto>? Children { get; set; }
}
