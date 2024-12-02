using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripWebData.Inputs
{
    /// <summary>
    /// 注册入参
    /// </summary>
    public class RegisterInput
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string UserName { get; set; } = null!;

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; } = null!;

        /// <summary>
        /// 确认密码
        /// </summary>
        [Required]
        public string ConfirmPwd { get; set; } = null!;

        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        public string Mobile { get; set; } = null!;

        /// <summary>
        /// 邮箱
        /// </summary>
        [Required]
        public string Email { get; set; } = null!;

        /// <summary>
        /// 邮箱验证码
        /// </summary>
        [Required]
        public long? EmailCode { get; set; } = null!;
    }
}
