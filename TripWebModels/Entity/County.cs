using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

public partial class County : BaseEntity
{
    public long CityId { get; set; }

    public string Name { get; set; } = null!;
}
