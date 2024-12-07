using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

/// <summary>
/// 收藏夹
/// </summary>
public partial class TabFavorite : BaseEntity
{
    /// <summary>
    /// 旅游产品ID
    /// </summary>
    public long TravelId { get; set; }
    
    /// <summary>
    /// 旅游产品
    /// </summary>
    public TabTravel Travel { get; set; } = null!;
}
