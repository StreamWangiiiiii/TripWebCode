using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

/// <summary>
/// 旅游产品图片
/// </summary>
public partial class TabTravelImg : BaseEntity
{
    /// <summary>
    /// 旅游产品Id
    /// </summary>
    public long TravelId { get; set; }

    /// <summary>
    /// 大图片地址
    /// </summary>
    public string BigImg { get; set; } = null!;

    /// <summary>
    /// 小图片地址
    /// </summary>
    public string? SmallImg { get; set; }
}
