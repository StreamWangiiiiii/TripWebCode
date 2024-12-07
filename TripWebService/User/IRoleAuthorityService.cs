using TripWebData;
using TripWebData.Dtos;
using TripWebData.Inputs;

namespace TripWebService.User;

/// <summary>
/// 角色权限相关服务
/// </summary>
public interface IRoleAuthorityService
{
    /// <summary>
    /// 为角色分配权限
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public Results<int> DistributeAuthority(RoleAuthorityInput input);


    /// <summary>
    /// 获取左边菜单栏
    /// </summary>
    /// <param name="loginRoleId"></param>
    /// <returns></returns>
    public Results<List<LeftMenuDto>> GetLeftMenuByRoleId(long loginRoleId);

    /// <summary>
    /// 获取角色的权限列表
    /// </summary>
    /// <returns></returns>
    public Results<List<AuthorityDto>> GetAuthoritiesByRoleId(long loginRoleId);
}