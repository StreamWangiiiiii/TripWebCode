using System.ComponentModel.DataAnnotations;

namespace TripWebData.Dtos.TravelBusiness;

/// <summary>
/// 游客用户数据传输对象
/// </summary>
public class TravelUserDto
{   
    /// <summary>
    /// 姓名
    /// </summary>
    [Display(Name = "姓名")]
    public string Nickname { get; set; } = null!;
    
    /// <summary>
    /// 手机
    /// </summary>
    [Display(Name = "手机")]
    public string Mobile { get; set; } = null!;
    
    /// <summary>
    /// 手机
    /// </summary>
    [Display(Name = "邮箱")]
    public string Email { get; set; } = null!;
    
    /// <summary>
    /// 账号
    /// </summary>
    [Display(Name = "账号")]
    public string Username { get; set; } = null!;
}