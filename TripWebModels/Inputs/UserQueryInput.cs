using System.ComponentModel.DataAnnotations;

namespace Travel.Data.Inputs;

/// <summary>
/// 用户分页查询-输入参数
/// </summary>
public class UserQueryInput:PageInput
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long? RoleId { get; set; }
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = null!;
    
    /// <summary>
    /// 真实姓名
    /// </summary>
    public string Nickname { get; set; } = null!;
    
    /// <summary>
    /// 手机号
    /// </summary>
    public string Mobile { get; set; } = null!;

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; } = null!;
    /// <summary>
    /// 时间查询区间-开始,默认一个月前
    /// </summary>
    [Required] public DateTime? StartTime { get; set; } =DateTime.Now.AddMonths(-1);

    /// <summary>
    /// 时间查询区间-结束,默认当前时间
    /// </summary>
    [Required]
    public DateTime? EndTime { get; set; } = DateTime.Now;
}