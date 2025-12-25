using SqlSugar;

namespace backendStd.Core.Entity;

/// <summary>
/// 角色权限关系实体
/// </summary>
[SugarTable("sys_role_permission")]
public class RolePermission : Base.EntityBase
{
    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(ColumnDescription = "角色ID")]
    public long RoleId { get; set; }

    /// <summary>
    /// 权限ID
    /// </summary>
    [SugarColumn(ColumnDescription = "权限ID")]
    public long PermissionId { get; set; }
}
