using System.ComponentModel;

namespace TripWebData.Enums
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
