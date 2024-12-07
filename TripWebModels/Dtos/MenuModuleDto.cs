namespace TripWebData.Dtos;

/// <summary>
/// 菜单模块 - 输出参数
/// </summary>
public class MenuModuleDto
{
    /// <summary>
    /// 主键：0-添加，大于0-修改
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// 模块名|菜单名
    /// </summary>
    public string ModuleName { get; set; } = null!;
    /// <summary>
    /// 前端页面地址
    /// </summary>
    public string? LinkUrl { get; set; }
    /// <summary>
    /// 菜单图标地址
    /// </summary>
    public string? IconUrl { get; set; }
    /// <summary>
    /// 上级模块ID，0-表示没有上级菜单
    /// </summary>
    public long? ParentMenuModuleId { get; set; }
    /// <summary>
    /// 上级模块名称
    /// </summary>
    public string ParentMenuModuleName { get; set; }
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedTime { get; set; }
}