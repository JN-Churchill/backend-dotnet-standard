namespace backendStd.Application.Dtos;

/// <summary>
/// 分页输入参数
/// </summary>
public class PageInput
{
    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序字段
    /// </summary>
    public string OrderBy { get; set; }

    /// <summary>
    /// 是否降序
    /// </summary>
    public bool IsDescending { get; set; } = true;
}

/// <summary>
/// 分页结果
/// </summary>
/// <typeparam name="T">数据类型</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> Items { get; set; }

    /// <summary>
    /// 总记录数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 当前页码
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// 每页大小
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(Total / (double)PageSize);

    /// <summary>
    /// 是否有上一页
    /// </summary>
    public bool HasPrevious => Page > 1;

    /// <summary>
    /// 是否有下一页
    /// </summary>
    public bool HasNext => Page < TotalPages;
}
