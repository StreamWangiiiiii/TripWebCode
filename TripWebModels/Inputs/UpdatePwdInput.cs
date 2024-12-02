using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Travel.Data.Inputs;

/// <summary>
/// 修改个人密码
/// </summary>
public class UpdatePwdInput:BaseInput
{

    /// <summary>
    /// 原始密码
    /// </summary>
    [Required]
    public string OldPwd { get; set; } = null!;
    
    /// <summary>
    /// 新密码
    /// </summary>
    [Required]
    public string NewPwd { get; set; } = null!;
    
    /// <summary>
    /// 确认密码
    /// </summary>
    [Required]
    public string ConfirmPwd { get; set; } = null!;
}