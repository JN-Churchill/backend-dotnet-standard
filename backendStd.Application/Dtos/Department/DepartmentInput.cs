namespace backendStd.Application.Dtos.Department;

/// <summary>
/// 部门新增输入
/// </summary>
public class DepartmentInput
{
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
    public long ParentId { get; set; } = 0;

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
    public int Sort { get; set; } = 0;

    /// <summary>
    /// 状态 0=禁用 1=启用
    /// </summary>
    public int Status { get; set; } = 1;
}
