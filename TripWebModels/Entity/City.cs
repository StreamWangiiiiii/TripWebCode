using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

public partial class City : BaseEntity
{
    public string Name { get; set; } = null!;

    public string AreaCode { get; set; } = null!;

    public ulong CenterFlag { get; set; }

    public long ProvinceId { get; set; }

    public string Keyword { get; set; } = null!;
}
