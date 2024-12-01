using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

/// <summary>
/// 旅游分类
/// </summary>
public partial class TabCategory : BaseEntity
{
    /// <summary>
    /// 分类名称
    /// </summary>
    public string CategoryName { get; set; } = null!;

    public TabUser UpdatedUser { get; set; }
}
