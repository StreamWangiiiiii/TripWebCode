using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripWebData.Dtos
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public long RoleId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; } = null!;

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = null!;

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string? Nickname { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public string? Birthday { get; set; }
        /// <summary>
        /// 1-男，2-女,3-未知
        /// </summary>
        public short? Sex { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; } = null!;
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = null!;
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime UpdatedTime { get; set; }
    }
}
