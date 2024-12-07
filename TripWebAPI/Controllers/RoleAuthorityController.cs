using Microsoft.AspNetCore.Mvc;
using TripWebAPI.Filters;
using TripWebData;
using TripWebData.Consts;
using TripWebData.Dtos;
using TripWebData.Inputs;
using TripWebService.User;
using TripWebAPI.Controllers;
using TripWebAPI.Filters;
using TripWebData.Consts;
using TripWebData.Dtos;
using TripWebData.Inputs;
using TripWebService.User;

namespace TripWeb.API.Controllers;

/// <summary>
/// 角色权限
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class RoleAuthorityController:BaseController
{
    private readonly IRoleAuthorityService _authority;

    public RoleAuthorityController(IRoleAuthorityService authority)
    {
        _authority = authority;
    }

    /// <summary>
    /// 为角色分配权限
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [TripWebAuthorize(AuthorizeRoleName.Administrator)]
    public Results<int> DistributeAuthority([FromBody]RoleAuthorityInput input)
    {
        return _authority.DistributeAuthority(input);
    }


    /// <summary>
    /// 获取左边菜单栏
    /// </summary>
    /// <returns></returns>
    [TripWebAuthorize(AuthorizeRoleName.Administrator)]
    [HttpGet]
    public Results<List<LeftMenuDto>> GetLeftMenuByRoleId()
    {
        return _authority.GetLeftMenuByRoleId(CurrentRoleId);
    }

    /// <summary>
    /// 获取角色的权限列表
    /// </summary>
    /// <returns></returns>
    [TripWebAuthorize(AuthorizeRoleName.Administrator)]
    [HttpGet]
    public Results<List<AuthorityDto>> GetAuthoritiesByRoleId()
    {
        return _authority.GetAuthoritiesByRoleId(CurrentRoleId);
    }
}