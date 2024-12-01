using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

/// <summary>
/// 菜单模块表
/// </summary>
public partial class TabMenuModule : BaseEntity
{

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
