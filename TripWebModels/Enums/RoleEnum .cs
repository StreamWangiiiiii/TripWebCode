using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripWebData.Enum
{
    public enum RoleEnum
    {
        [Description("管理员")]
        Admin = 1,
        [Description("普通用户")]
        User = 2,
        [Description("商家")]
        Seller = 3
    }
}
