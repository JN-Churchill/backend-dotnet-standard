using SqlSugar;

namespace backendStd.Core.Entity;

/// <summary>
/// 演示实体
/// </summary>
[SugarTable("demo")]
public class Demo : Base.EntityBase
{
    /// <summary>
    /// 名称
    /// </summary>
    [SugarColumn(ColumnDescription = "名称", Length = 100)]
    public string Name { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    [SugarColumn(ColumnDescription = "描述", Length = 500, IsNullable = true)]
    public string Description { get; set; }

    /// <summary>
    /// 状态 0=禁用 1=启用
    /// </summary>
    [SugarColumn(ColumnDescription = "状态")]
    public int Status { get; set; } = 1;

    /// <summary>
    /// 排序号
    /// </summary>
    [SugarColumn(ColumnDescription = "排序号")]
    public int Sort { get; set; }
}
