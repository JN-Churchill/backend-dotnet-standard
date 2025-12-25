namespace backendStd.Core.Enum;

/// <summary>
/// 通用状态枚举
/// </summary>
public enum StatusEnum
{
    /// <summary>
    /// 禁用
    /// </summary>
    Disabled = 0,

    /// <summary>
    /// 启用
    /// </summary>
    Enabled = 1
}

/// <summary>
/// 错误码枚举
/// </summary>
public enum ErrorCodeEnum
{
    /// <summary>
    /// 成功
    /// </summary>
    Success = 200,

    /// <summary>
    /// 请求参数错误
    /// </summary>
    BadRequest = 400,

    /// <summary>
    /// 未授权
    /// </summary>
    Unauthorized = 401,

    /// <summary>
    /// 禁止访问
    /// </summary>
    Forbidden = 403,

    /// <summary>
    /// 资源不存在
    /// </summary>
    NotFound = 404,

    /// <summary>
    /// 服务器内部错误
    /// </summary>
    InternalServerError = 500
}

/// <summary>
/// 数据权限范围枚举
/// </summary>
public enum DataScopeEnum
{
    /// <summary>
    /// 全部数据
    /// </summary>
    All = 1,

    /// <summary>
    /// 自定义数据（指定部门）
    /// </summary>
    Custom = 2,

    /// <summary>
    /// 本部门数据
    /// </summary>
    Dept = 3,

    /// <summary>
    /// 本部门及子部门数据
    /// </summary>
    DeptAndChild = 4,

    /// <summary>
    /// 仅本人数据
    /// </summary>
    Self = 5
}
