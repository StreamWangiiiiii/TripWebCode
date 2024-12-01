using System;
using System.Collections.Generic;

namespace TripWebData.Entity;

public partial class Province : BaseEntity
{
    public string Name { get; set; } = null!;

    public string ShortName { get; set; } = null!;

    public string Area { get; set; } = null!;
}
