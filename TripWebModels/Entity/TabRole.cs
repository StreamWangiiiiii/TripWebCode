using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

/// <summary>
/// 角色表
/// </summary>
public partial class TabRole : BaseEntity
{
    /// <summary>
    /// 角色名称
    /// </summary>
    public string? RoleName { get; set; }
}
