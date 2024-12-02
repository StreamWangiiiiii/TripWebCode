using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Travel.Data.Inputs;

/// <summary>
/// 订单入参
/// </summary>
public class OrderAddInput:BaseInput
{
    /// <summary>
    /// 景点ID
    /// </summary>
    public long TravelId { get; set; }

    /// <summary>
    /// SignalR 的连接ID
    /// </summary>
    public string ConnectionId { get; set; } = null!;

    /// <summary>
    /// 亲友列表(如果没有亲友，就可以空着)
    /// </summary>
    public List<Friend> Friends { get; set; } = new();
}

/// <summary>
/// 亲友信息
/// </summary>
public class Friend
{
    /// <summary>
    /// 亲友手机
    /// </summary>
    public string Mobile { get; set; } = null!;
    /// <summary>
    /// 昵称
    /// </summary>
    public string NickName { get; set; }= null!;
}