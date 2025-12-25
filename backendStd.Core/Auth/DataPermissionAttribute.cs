namespace backendStd.Core.Auth;

/// <summary>
/// 数据权限特性
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class DataPermissionAttribute : Attribute
{
    /// <summary>
    /// 数据权限范围
    /// 1=全部数据 2=自定义数据 3=本部门 4=本部门及子部门 5=仅本人
    /// </summary>
    public int DataScope { get; set; } = 1;

    /// <summary>
    /// 实体字段名（用于数据过滤）
    /// </summary>
    public string EntityField { get; set; } = "CreateUserId";

    public DataPermissionAttribute()
    {
    }

    public DataPermissionAttribute(int dataScope, string entityField = "CreateUserId")
    {
        DataScope = dataScope;
        EntityField = entityField;
    }
}
