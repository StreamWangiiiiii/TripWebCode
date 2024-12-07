namespace TripWebData.Dtos;

/// <summary>
/// 左边菜单栏输出参数
/// </summary>
public class LeftMenuDto
{
    /// <summary>
    /// 菜单模块ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string ModuleName { get; set; } = null!;
    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; set; }
    
    /// <summary>
    /// 链接地址
    /// </summary>
    public string? LinkUrl { get; set; }
    
    /// <summary>
    /// 左边子菜单列表
    /// </summary>
    public List<LeftMenuDto> Child { get; set; } = new();
}