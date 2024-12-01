using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripWebData.Options
{
    public class JwtTokenOption
    {
        /// <summary>
        /// Token 过期时间，默认为60分钟
        /// </summary>
        public int TokenExpireTime { get; set; } = 60;
        /// <summary>
        /// 接收人
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 秘钥
        /// </summary>
        public string SecurityKey { get; set; }
        /// <summary>
        /// 签发人
        /// </summary>
        public string Issuer { get; set; }
    }
}
