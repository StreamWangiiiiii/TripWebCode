using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Travel.Data.Inputs;

/// <summary>
/// 更新(完善)个人档案（信息）
/// </summary>
public class UserProfileInput
{
    /// <summary>
    /// 用户名(前端不可被修改)
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