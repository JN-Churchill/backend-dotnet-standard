using SqlSugar;

namespace backendStd.Core.Entity;

/// <summary>
/// 用户实体
/// </summary>
[SugarTable("sys_user")]
public class User : Base.EntityBase
{
    /// <summary>
    /// 用户名
    /// </summary>
    [SugarColumn(ColumnDescription = "用户名", Length = 50)]
    public string UserName { get; set; }

    /// <summary>
    /// 密码（MD5加密）
    /// </summary>
    [SugarColumn(ColumnDescription = "密码", Length = 100)]
    public string Password { get; set; }

    /// <summary>
    /// 真实姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "真实姓名", Length = 50, IsNullable = true)]
    public string RealName { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [SugarColumn(ColumnDescription = "手机号", Length = 20, IsNullable = true)]
    public string Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    [SugarColumn(ColumnDescription = "邮箱", Length = 100, IsNullable = true)]
    public string Email { get; set; }

    /// <summary>
    /// 头像URL
    /// </summary>
    [SugarColumn(ColumnDescription = "头像", Length = 500, IsNullable = true)]
    public string Avatar { get; set; }

    /// <summary>
    /// 状态 0=禁用 1=启用
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 最后登录时间
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录时间", IsNullable = true)]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 最后登录IP
    /// </summary>
    [SugarColumn(ColumnDescription = "最后登录IP", Length = 50, IsNullable = true)]
    public string LastLoginIp { get; set; }
}
