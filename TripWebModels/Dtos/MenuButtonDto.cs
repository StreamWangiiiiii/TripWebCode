namespace TripWebData.Dtos;

public class MenuButtonDto
{
    /// <summary>
    /// 功能按钮ID
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// 所属菜单ID
    /// </summary>
    public long MenuModuleId { get; set; }

    /// <summary>
    /// 按钮名称
    /// </summary>
    public string ButtonName { get; set; } = null!;
    /// <summary>
    /// 菜单图标地址
    /// </summary>
    public string? IconUrl { get; set; }

    /// <summary>
    /// 菜单模块名称
    /// </summary>
    public string MenuModuleName { get; set; } = null!;
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedTime { get; set; }
}