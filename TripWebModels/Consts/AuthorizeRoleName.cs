namespace TripWebAPI.Filters
{
    /// <summary>
    /// 授权策略角色名称
    /// </summary>
    public class AuthorizeRoleName
    {
        /// <summary>
        /// 管理员角色
        /// </summary>
        public const string Administrator = "管理员";
        /// <summary>
        /// 商家
        /// </summary>
        public const string SellerAdministrator = "商家";
        /// <summary>
        /// 普通用户
        /// </summary>
        public const string TravelUser = "普通用户";
        /// <summary>
        /// 管理员或者商家都可操作
        /// </summary>
        public const string AdminOrSeller = "管理员或者商家都可操作";
    }
}
