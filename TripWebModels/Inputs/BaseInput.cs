using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace TripWebData.Inputs;

/// <summary>
/// 基类-输入参数
/// </summary>
public abstract class BaseInput
{
    /// <summary>
    /// 当前登录人
    /// </summary>
    [JsonIgnore]
    public long LoginUserId { get; set; }
    
    /// <summary>
    /// 基类初始化
    /// </summary>
    protected BaseInput()
    {
        var httpContextAccessor = LocalServiceProvider.Instance.GetService<IHttpContextAccessor>();
        if (httpContextAccessor?.HttpContext?.User.Identity is {IsAuthenticated: true})
        {
            LoginUserId= Convert.ToInt64(httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(
                p => p.Type.Equals("UserId"))!.Value);
        }
    }
}