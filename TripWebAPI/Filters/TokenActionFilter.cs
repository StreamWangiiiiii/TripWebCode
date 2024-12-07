using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using TripWebData.Dtos;
using TripWebData.Options;
using TripWebService;
using TripWebUtils.Utils;

namespace TripWebAPI.Filters
{
    /// <summary>
    /// Token 自动刷新
    /// </summary>
    public class TokenActionFilter : IActionFilter
    {
        private readonly ITokenService _tokenService;
        private readonly JwtTokenOption _jwtTokenOption;
        public TokenActionFilter(ITokenService tokenService, IOptionsMonitor<JwtTokenOption> monitor)
        {
            _tokenService = tokenService;
            _jwtTokenOption = monitor.CurrentValue;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // 只对需要认证授权的方法刷新token
            if (!HasIAuthorize(context))
            {
                return;
            } 
            if (context.HttpContext.User.Identity is { IsAuthenticated: true })
            {
                var objectResult = context.Result as ObjectResult;
                var val = objectResult?.Value;
                if (val == null)
                {
                    return;
                }
                var type = objectResult!.DeclaredType; // 实际返回的类型
                var userClaims = context.HttpContext.User.Claims.ToList();
                // 到期时间
                var exp = Convert.ToInt64(userClaims.FirstOrDefault(p => p.Type =="exp")!.Value);

                // 判断token 是否过期: 拿到期时间-当前过期 = token 剩余可用时间
                var timeSpan = DateTimeUtil.GetDataTime(exp).Subtract(DateTime.Now);
                // 剩余的时间
                var minutes = timeSpan.TotalMinutes;

                // 如果剩余时间少于Token有效时间的一半，我们返回一个新的Token给前端
                if (minutes < _jwtTokenOption.TokenExpireTime / 2.0)
                {
                    var token = _tokenService.GenerateToken(new UserDto
                    {
                        Nickname = userClaims.FirstOrDefault(p => p.Type ==
                        "NickName")!.Value,
                        Username = userClaims.FirstOrDefault(p => p.Type ==
                        "UserName")!.Value,
                        RoleName = userClaims.FirstOrDefault(p => p.Type ==
                        "RoleName")!.Value,
                        RoleId = Convert.ToInt64(userClaims.FirstOrDefault(p =>
                        p.Type == "RoleId")!.Value),
                        Id = Convert.ToInt64(userClaims.FirstOrDefault(p => p.Type
                        == "UserId")!.Value)
                    });

                    // 设置新的token，给前端重新存储
                    type!.GetProperty("Token")!.SetValue(val, token);
                    context.Result = new JsonResult(val);
                }
            }
        }

        // 判断是否含有Authorize
        private bool HasIAuthorize(ActionExecutedContext context)
        {
            if (context.Filters.Any(filter => filter is IAuthorizationFilter))
            {
                return true;
            }
            // 终节点：里面包含了路由方法的所有元素信息（特性等信息）
            var endpoint = context.HttpContext.GetEndpoint();
            return endpoint?.Metadata.GetMetadata<IAuthorizeData>() != null;
        }
    }
}
