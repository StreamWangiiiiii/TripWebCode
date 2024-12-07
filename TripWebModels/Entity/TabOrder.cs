using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    /// <summary>
    /// 乐观锁
    /// </summary>
    [Timestamp]
    public DateTime Version { get; set; }
}
