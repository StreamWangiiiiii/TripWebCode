using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripWebData.Inputs
{
    /// <summary>
    /// 登录入参
    /// </summary>
    public class LoginInput
    {
        /// <summary>
        /// 登录账号
        /// </summary>
        [Required]
        public string LoginAccount { get; set; } = null!;

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Pwd { get; set; } = null!;
    }
}
