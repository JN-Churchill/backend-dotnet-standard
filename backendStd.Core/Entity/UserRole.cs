using SqlSugar;

namespace backendStd.Core.Entity;

/// <summary>
/// 用户角色关系实体
/// </summary>
[SugarTable("sys_user_role")]
public class UserRole : Base.EntityBase
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [SugarColumn(ColumnDescription = "用户ID")]
    public long UserId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    [SugarColumn(ColumnDescription = "角色ID")]
    public long RoleId { get; set; }
}
