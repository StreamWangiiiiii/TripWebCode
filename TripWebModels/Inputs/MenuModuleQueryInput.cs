namespace Travel.Data.Inputs;

/// <summary>
/// 菜单模板查询 - 输入参数
/// </summary>
public class MenuModuleQueryInput:PageInput
{
    /// <summary>
    /// 父菜单ID
    /// </summary>
    public long? ParentId { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string? MenuName { get; set; }
}