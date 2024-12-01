using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

/// <summary>
///  管理员|游客|入驻商家
/// </summary>
public partial class TabUser : BaseEntity
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long RoleId { get; set; }

    /// <summary>
    /// 角色类型
    /// </summary>
    public TabRole Role { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// 真实姓名
    /// </summary>
    public string? Nickname { get; set; }

    /// <summary>
    /// 生日
    /// </summary>
    public DateOnly? Birthday { get; set; }

    /// <summary>
    /// 1-男，2-女,3-未知
    /// </summary>
    public bool? Sex { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string? Mobile { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// 激活状态，1-已激活，0-未激活
    /// </summary>
    public bool ActiveStatus { get; set; }

    /// <summary>
    /// 激活码
    /// </summary>
    public long? ActiveCode { get; set; }
}
