namespace TripWebData.Dtos.TravelBusiness;

public class OrderDto
{
    /// <summary>
    /// 订单编号
    /// </summary>
    public long Id { get; set; }
    /// <summary>
    /// 景点名称
    /// </summary>
    public string TravelName { get; set; } = null!;
    /// <summary>
    /// 订单总价
    /// </summary>
    public double TotalPrice { get; set; }
    /// <summary>
    /// 下单时间
    /// </summary>
    public DateTime CreatedTime { get; set; }
    /// <summary>
    /// 下单人账号
    /// </summary>
    public string UserName { get; set; } = null!;
    
    /// <summary>
    /// 下单人ID
    /// </summary>
    public long CreatedUserId { get; set; }
    /// <summary>
    /// 亲友列表
    /// </summary>
    public List<OrderDetail> FriendList { get; set; } = new();
}

/// <summary>
/// 亲友列表
/// </summary>
public class OrderDetail
{
    /// <summary>
    /// 亲友姓名
    /// </summary>
    public string? FriendName { get; set; }
    /// <summary>
    /// 亲友电话
    /// </summary>
    public string? FriendMobile { get; set; }
}