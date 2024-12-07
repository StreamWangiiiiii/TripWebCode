using System.ComponentModel.DataAnnotations;

namespace TripWebData.Inputs;

/// <summary>
/// 添加或者修改-输入参数
/// </summary>
public class UserAddOrUpdateInput:BaseInput
{
    /// <summary>
    /// 主键
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// 角色ID
    /// </summary>
    [Required] public long RoleId { get; set; }
    /// <summary>
    /// 用户名
    /// </summary>
    [Required] public string Username { get; set; } = null!;
   
    /// <summary>
    /// 真实姓名
    /// </summary>
    [Required] public string Nickname { get; set; } = null!;
    /// <summary>
    /// 生日
    /// </summary>
    public DateTime? Birthday { get; set; }
    /// <summary>
    /// 1-男，2-女,3-未知
    /// </summary>
    public short Sex { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [Required] public string Mobile { get; set; } = null!;

    /// <summary>
    /// 邮箱
    /// </summary>
    [Required]public string Email { get; set; } = null!;
   
}