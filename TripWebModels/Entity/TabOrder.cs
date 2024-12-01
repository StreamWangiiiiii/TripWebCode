using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

/// <summary>
/// 订单表
/// </summary>
public partial class TabOrder : BaseEntity
{
    /// <summary>
    /// 旅游项目ID
    /// </summary>
    public long? TravelId { get; set; }

    public DateTime Version { get; set; }
}
