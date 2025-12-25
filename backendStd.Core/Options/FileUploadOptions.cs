namespace backendStd.Core.Options;

/// <summary>
/// 文件上传配置选项
/// </summary>
public class FileUploadOptions
{
    /// <summary>
    /// 最大文件大小（字节）
    /// </summary>
    public long MaxSize { get; set; } = 10485760; // 10MB

    /// <summary>
    /// 允许的文件扩展名
    /// </summary>
    public List<string> AllowedExtensions { get; set; } = new()
    {
        ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx", ".xls", ".xlsx"
    };

    /// <summary>
    /// 上传路径
    /// </summary>
    public string UploadPath { get; set; } = "uploads";
}

/// <summary>
/// 雪花ID配置选项
/// </summary>
public class SnowIdOptions
{
    /// <summary>
    /// 工作机器ID（0-63）
    /// </summary>
    public ushort WorkerId { get; set; } = 1;
}

/// <summary>
/// CORS配置选项
/// </summary>
public class CorsOptions
{
    /// <summary>
    /// 允许的来源
    /// </summary>
    public List<string> AllowOrigins { get; set; } = new();

    /// <summary>
    /// 允许的方法
    /// </summary>
    public List<string> AllowMethods { get; set; } = new();

    /// <summary>
    /// 允许的头
    /// </summary>
    public List<string> AllowHeaders { get; set; } = new();

    /// <summary>
    /// 是否允许凭据
    /// </summary>
    public bool AllowCredentials { get; set; }
}
