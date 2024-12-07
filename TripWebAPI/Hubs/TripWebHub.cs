using DotNetCore.CAP;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using TripWebAPI.Filters;
using TripWebData;
using TripWebData.Consts;
using TripWebData.Inputs;
using TripWebUtils.Utils.RedisUtil;

namespace TripWebAPI.Hubs
{
    /// <summary>
    /// 通讯中心
    /// </summary>
    [TripWebAuthorize]
    public class TripWebHub:Hub
    {
        private readonly ICapPublisher _capPublisher;

        public TripWebHub(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }
        
        /// <summary>
        /// 下订单
        /// </summary>
        /// <param name="input"></param>
        public async Task OrderAdd(OrderAddInput input)
        {
            input.ConnectionId = Context.ConnectionId;
            input.LoginUserId = Convert.ToInt64(Context.User!.Claims.FirstOrDefault(
                p => p.Type.Equals("UserId"))!.Value);
            await _capPublisher.PublishAsync(EventBus.OrderAdd,input);
            await Clients.Client(Context.ConnectionId).SendAsync(SignalMethod.ReceiveMessage,
                Results<string>.GetResult(msg:"队列提交成功，请等待处理结果"));
        }
        
        /// <summary>
        /// 批量导入用户
        /// </summary>
        /// <param name="filePath">excel 服务端的路径(如：/upload/20230316.xlsx)</param>
        public async Task ImportUser(string filePath)
        {
            ImportUserInput input = new()
            {
                ConnectionId = Context.ConnectionId,
                FilePath = filePath,
                LoginUserId = Convert.ToInt64(Context.User!.Claims.FirstOrDefault(
                    p => p.Type.Equals("UserId"))!.Value)
            };

            await _capPublisher.PublishAsync(EventBus.ImportUser, input);
            await Clients.Client(Context.ConnectionId).SendAsync(SignalMethod.ReceiveMessage,
                Results<string>.GetResult(msg:"队列提交成功，请等待处理结果"));
        }
        
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
