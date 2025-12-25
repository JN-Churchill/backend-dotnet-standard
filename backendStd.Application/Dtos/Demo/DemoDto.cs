namespace backendStd.Application.Dtos.Demo;

/// <summary>
/// DemoDTO
/// </summary>
public class DemoDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Status { get; set; }
    public int Sort { get; set; }
    public DateTime CreateTime { get; set; }
}

/// <summary>
/// Demo新增输入
/// </summary>
public class DemoInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Sort { get; set; }
}

/// <summary>
/// Demo更新输入
/// </summary>
public class DemoUpdateInput
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Status { get; set; }
    public int Sort { get; set; }
}
