using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

/// <summary>
/// 订单人员详情表
/// </summary>
public partial class TabOrderDetail
{
    /// <summary>
    /// 订单人员详情ID
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// 旅游项目ID
    /// </summary>
    public long? TravelId { get; set; }

    /// <summary>
    /// 所属订单ID
    /// </summary>
    public long? OrderId { get; set; }

    /// <summary>
    /// 亲友姓名
    /// </summary>
    public string? FriendName { get; set; }

    /// <summary>
    /// 亲友电话
    /// </summary>
    public string? FriendMobile { get; set; }

    /// <summary>
    /// 下单人ID
    /// </summary>
    public long? OrderUserId { get; set; }
}
