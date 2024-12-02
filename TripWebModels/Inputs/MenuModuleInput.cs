using System.Text.Json.Serialization;

namespace Travel.Data.Inputs;

/// <summary>
/// 菜单模块 - 输入参数
/// </summary>
public class MenuModuleInput:BaseInput
{
    /// <summary>
    /// 主键,id>0 修改，id=0表示添加
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
    /// 上级模块ID
    /// </summary>
    public long? ParentMenuModuleId { get; set; }
}