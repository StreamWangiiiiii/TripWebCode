using Microsoft.AspNetCore.Mvc;
using TripWebAPI.Controllers;
using TripWebAPI.Filters;
using TripWebData;
using TripWebData.Consts;
using TripWebData.Dtos;
using TripWebData.Inputs;
using TripWebService.User;

namespace TripWeb.API.Controllers;

/// <summary>
/// 菜单模板
/// </summary>
[Route("[controller]/[action]")]
[ApiController]
public class MenuModuleController:BaseController
{
    private readonly IMenuModuleService _menuModuleService;


    public MenuModuleController(IMenuModuleService menuModuleService)
    {
        _menuModuleService = menuModuleService;
    }

    /// <summary>
    /// 添加或者修改菜单模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [TripWebAuthorize(AuthorizeRoleName.Administrator)]
    public Results<int> UpdateOrAdd([FromBody]MenuModuleInput input)
    {
        return _menuModuleService.UpdateOrAdd(input);
    }

    /// <summary>
    /// 菜单模块查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [TripWebAuthorize(AuthorizeRoleName.Administrator)]
    public Results<PageDto<MenuModuleDto>> MenuModuleQuery([FromBody]MenuModuleQueryInput input)
    {
        return _menuModuleService.MenuModuleQuery(input);
    }

    /// <summary>
    /// 根据菜单模块ID获取一条记录
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [TripWebAuthorize(AuthorizeRoleName.Administrator)]
    [HttpGet("{id}")]
    public Results<MenuModuleDto> GetMenuModuleById(long id)
    {
        return _menuModuleService.GetMenuModuleById(id);
    }


    /// <summary>
    /// 批量删除
    /// </summary>
    /// <param name="idList"></param>
    /// <returns></returns>
    [HttpDelete]
    [TripWebAuthorize(AuthorizeRoleName.Administrator)]
    public Results<int> Remove([FromBody]List<long> idList)
    {
        return _menuModuleService.Remove(idList.ToArray());
    }
}