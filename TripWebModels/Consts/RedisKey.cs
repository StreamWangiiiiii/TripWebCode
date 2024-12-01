using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripWebData.Consts
{
    public class RedisKey
    {
        /// <summary>
        /// 用户对应的SignalR的ConnectionID
        /// </summary>
        public const string UserWebSocketConnectionId = "UserWebSocketConnectionId_{0}";

        /// <summary>
        /// 所有的分类列表
        /// </summary>
        public const string AllCategoryList = "AllCategoryList";

        /// <summary>
        /// 用户的激活码
        /// </summary>
        public const string UserActiveCode = "UserActiveCode{0}";

        /// <summary>
        /// 用户忘记密码时发送的授权码
        /// </summary>
        public const string UserAuthorizeCode = "UserAuthorizeCode{0}";

        /// <summary>
        /// 角色权限列表
        /// </summary>
        public const string RoleMenuList = "RoleMenuList{0}";
    }
}
