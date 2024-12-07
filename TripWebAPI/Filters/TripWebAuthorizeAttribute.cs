using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TripWebData;

namespace TripWebAPI.Filters
{
    /// <summary>
    /// 授权过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class TripWebAuthorizeAttribute : Attribute, IAuthorizationFilter, IAuthorizeData
    {
        public TripWebAuthorizeAttribute() { }
        /// <summary>
        /// 策略名称
        /// </summary>
        public string? Policy { get; set; }
        /// <summary>
        /// 可支持角色
        /// </summary>
        public string? Roles { get; set; }
        /// <summary>
        /// 以逗号分隔的方案列表，从中构造用户信息
        /// </summary>
        public string? AuthenticationSchemes { get; set; }
        public TripWebAuthorizeAttribute(string policy) => this.Policy = policy;

        /// <summary>
        /// 授权时执行此方法
        /// </summary>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 需要排除具有AllowAnymons 这个标签的控制器
            // 过滤掉带有AllowAnonymousFilter
            if (HasAllowAnonymous(context))
            {
                return;
            }
            // 如果用户没有登录，则给出一个友好的提示（而不是返回401给前端）
            if (!context.HttpContext.User.Identity.IsAuthenticated) // 判断是否登录
            {
                context.Result = new JsonResult(Results<string>.FailResult("Token无效"));
            }
        }

        // 判断是否含有IAllowAnonymous
        private bool HasAllowAnonymous(AuthorizationFilterContext context)
        {
            if (context.Filters.Any(filter => filter is IAllowAnonymousFilter))
            {
                return true;
            }
            // 终节点：里面包含了路由方法的所有元素信息（特性等信息）
            var endpoint = context.HttpContext.GetEndpoint();
            return endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null;
        }
    }
}
