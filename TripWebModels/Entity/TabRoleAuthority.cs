using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

/// <summary>
/// 角色权限表
/// </summary>
public partial class TabRoleAuthority : BaseEntity
{
    /// <summary>
    /// 所属菜单ID
    /// </summary>
    public long? MenuModuleId { get; set; }

    /// <summary>
    /// 功能按钮ID
    /// </summary>
    public long? MenuButtonId { get; set; }

    /// <summary>
    /// 角色ID
    /// </summary>
    public long? RoleId { get; set; }
    
    public TabMenuModule MenuModule { get; set; }

    public TabMenuButton MenuButton { get; set; }
}
