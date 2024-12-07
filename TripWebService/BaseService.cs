using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TripWebUtils.Utils.Snowflake;
using TripWebData;

namespace TripWebService;

/// <summary>
/// 基础服务
/// </summary>
public class BaseService
{
    /// <summary>
    /// 当前登录人ID
    /// </summary>
    protected long LoginUserId { get; set; }
    /// <summary>
    /// 数据库上下文
    /// </summary>
    protected TripWebContext TripWebContext => LocalServiceProvider.Instance.GetService<TripWebContext>()!;
    
    /// <summary>
    /// AutoMap 实例
    /// </summary>
    protected IMapper ObjectMapper => LocalServiceProvider.Instance.GetService<IMapper>()!;
    
    /// <summary>
    /// 雪花ID 实例
    /// </summary>
    protected IdWorker SnowIdWorker => SnowflakeUtil.CreateIdWorker();

    public BaseService()
    {
        var httpContextAccessor = LocalServiceProvider.Instance.GetService<IHttpContextAccessor>();

        if (httpContextAccessor?.HttpContext?.User.Identity is { IsAuthenticated: true })
        {
            LoginUserId= Convert.ToInt64(httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(
                p => p.Type.Equals("UserId"))!.Value);
        }
    }
}