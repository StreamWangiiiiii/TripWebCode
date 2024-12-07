namespace TripWebData.Inputs;

/// <summary>
/// 菜单按钮输入参数
/// </summary>
public class MenuButtonInput:BaseInput
{
    /// <summary>
    /// 主键，Id>0 表示修改,id=0 添加
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// 所属菜单ID
    /// </summary>
    public long? MenuModuleId { get; set; }
    /// <summary>
    /// 按钮名称
    /// </summary>
    public string? ButtonName { get; set; }
    /// <summary>
    /// 菜单图标地址
    /// </summary>
    public string? IconUrl { get; set; }
}