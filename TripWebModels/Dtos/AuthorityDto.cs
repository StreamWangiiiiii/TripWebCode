namespace TripWebData.Dtos;

/// <summary>
/// 角色权限输出参数
/// </summary>
public class AuthorityDto
{
    /// <summary>
    /// 菜单模块ID
    /// </summary>
    public long MenuModuleId { get; set; } 
    /// <summary>
    /// 菜单模块名称
    /// </summary>
    public string MenuModuleName { get; set; } = null!;

    /// <summary>
    /// 功能按钮列表
    /// </summary>
    public List<AuthorityButtonDto> Buttons { get; set; } = new();
}

/// <summary>
/// 权限功能按钮输出参数
/// </summary>
public class AuthorityButtonDto
{
    /// <summary>
    /// 功能按钮Id
    /// </summary>
    public long ButtonId { get; set; }
    /// <summary>
    /// 功能按钮名称
    /// </summary>
    public string? ButtonName { get; set; }
}