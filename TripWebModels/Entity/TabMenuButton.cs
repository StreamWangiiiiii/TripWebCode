using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

/// <summary>
/// 菜单页面上对应的按钮表
/// </summary>
public partial class TabMenuButton : BaseEntity
{
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
