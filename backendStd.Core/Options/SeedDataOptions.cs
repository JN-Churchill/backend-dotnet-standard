namespace backendStd.Core.Options;

/// <summary>
/// 种子数据配置选项
/// </summary>
public class SeedDataOptions
{
    /// <summary>
    /// 是否启用种子数据初始化
    /// </summary>
    public bool EnableSeedData { get; set; } = true;
    
    /// <summary>
    /// 种子数据初始化标记文件路径
    /// </summary>
    public string SeedDataFlagFile { get; set; } = "seed_data_initialized.flag";
}
