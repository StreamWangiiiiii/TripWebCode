namespace TripWebData.Inputs;

/// <summary>
/// 菜单按钮查询-输入参数
/// </summary>
public class MenuButtonQueryInput:PageInput
{
    /// <summary>
    /// 菜单模块ID
    /// </summary>
    public long? MenuModuleId { get; set; }
}