using System.ComponentModel.DataAnnotations;

namespace Travel.Data.Inputs;

/// <summary>
/// 找回密码-输入参数
/// </summary>
public class UpdatePwdWhenForgetInput
{
    /// <summary>
    /// 登录账号
    /// </summary>
    [Required]
    public string LoginAccount { get; set; } = null!;

    /// <summary>
    /// 验证码
    /// </summary>
    public long AuthorizeCode { get; set; }

    /// <summary>
    /// 新的密码
    /// </summary>
    [Required]
    public string Password { get; set; } = null!;

    /// <summary>
    /// 确认密码
    /// </summary>
    [Required]
    public string ConfirmPwd { get; set; } = null!;
}