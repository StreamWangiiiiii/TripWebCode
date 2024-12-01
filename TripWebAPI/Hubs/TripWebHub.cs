using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using Utils.Utils.RedisUtil;

namespace TripWebAPI.Hubs
{
    /// <summary>
    /// 通讯中心
    /// </summary>
    [Authorize]
    public class TripWebHub:Hub
    {
        /// <summary>
        /// （前端调用）发送消息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        /// <summary>
        /// 当客户端连接成功时,保存当前用户对应的signalR连接ID
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            var userId = Convert.ToInt64(Context.User.Claims.FirstOrDefault(p =>
            p.Type.Equals("UserId")).Value);
            CacheManager.Set(string.Format(TripWebData.Consts.RedisKey.UserWebSocketConnectionId,userId), Context.ConnectionId);
            return Task.CompletedTask;
        }
        /// <summary>
        /// 当断开连接时，移除当前用户对应的SignalR连接ID
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Convert.ToInt64(Context.User.Claims.FirstOrDefault(p =>
            p.Type.Equals("UserId")).Value);
            CacheManager.Remove(string.Format(TripWebData.Consts.RedisKey.UserWebSocketConnectionId,userId));
            return Task.CompletedTask; 
        }
    }
}
