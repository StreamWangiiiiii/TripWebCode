using Microsoft.AspNetCore.Mvc;

namespace TripWebAPI.Controllers
{
    /// <summary>
    /// 控制器基类
    /// </summary>
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 当前用户登录ID
        /// </summary>
        public long LoginUserId
        {
            get
            {
                if (HttpContext.User.Identity is { IsAuthenticated: true })
                {
                    return Convert.ToInt64(HttpContext.User.Claims.FirstOrDefault(
                    p => p.Type.Equals("UserId"))!.Value);
                }
                return 0;
            }
        }
        /// <summary>
        /// 当前用户RoleID
        /// </summary>
        public long CurrentRoleId
        {
            get
            {
                if (HttpContext.User.Identity is { IsAuthenticated: true })
                {
                    return Convert.ToInt64(HttpContext.User.Claims.FirstOrDefault(
                    p => p.Type.Equals("RoleId"))!.Value);
                }
                return 0;
            }
        }
    }
}
