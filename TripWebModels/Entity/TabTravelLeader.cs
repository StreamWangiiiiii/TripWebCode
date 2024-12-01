using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

/// <summary>
/// 景点领队表
/// </summary>
public partial class TabTravelLeader : BaseEntity
{
    /// <summary>
    /// 领队姓名
    /// </summary>
    public string LeaderNickname { get; set; } = null!;

    /// <summary>
    /// 领队手机
    /// </summary>
    public string LeaderMobile { get; set; } = null!;

    /// <summary>
    /// 领队用户ID
    /// </summary>
    public long LeaderUserId { get; set; }

    /// <summary>
    /// 旅游景点ID
    /// </summary>
    public long TravelId { get; set; }
}
