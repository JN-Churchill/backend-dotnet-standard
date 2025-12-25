namespace backendStd.Application.Dtos.User;

/// <summary>
/// 用户DTO
/// </summary>
public class UserDto
{
    public long Id { get; set; }
    public string UserName { get; set; }
    public string RealName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Avatar { get; set; }
    public int Status { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime? LastLoginTime { get; set; }
}

/// <summary>
/// 用户新增输入
/// </summary>
public class UserInput
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string RealName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
}

/// <summary>
/// 用户更新输入
/// </summary>
public class UserUpdateInput
{
    public string RealName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Avatar { get; set; }
    public int Status { get; set; }
}

/// <summary>
/// 登录输入
/// </summary>
public class LoginInput
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

/// <summary>
/// 登录结果
/// </summary>
public class LoginOutput
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public long ExpiredTime { get; set; }
    public UserDto UserInfo { get; set; }
}
