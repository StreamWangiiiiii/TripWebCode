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
/// 菜单按钮相关服务
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
public class MenuButtonController:BaseController
{
    private readonly IMenuButtonService _menuButtonService;

    public MenuButtonController(IMenuButtonService menuButtonService)
    {
        _menuButtonService = menuButtonService;
    }

    /// <summary>
    /// 添加或者修改 菜单按钮
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [TripWebAuthorize(AuthorizeRoleName.Administrator)]
    public Results<int> UpdateOrAdd([FromBody]MenuButtonInput input)
    {
        return _menuButtonService.UpdateOrAdd(input);
    }

    /// <summary>
    /// 菜单按钮查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [TripWebAuthorize(AuthorizeRoleName.Administrator)]
    public Results<PageDto<MenuButtonDto>> MenuButtonQuery([FromBody]MenuButtonQueryInput input)
    {
        return _menuButtonService.MenuButtonQuery(input);
    }
    
    /// <summary>
    /// 根据ID获取一条记录
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [TripWebAuthorize(AuthorizeRoleName.Administrator)]
    public Results<MenuButtonDto> GetById(long id)
    {
        return _menuButtonService.GetById(id);
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
        return _menuButtonService.Remove(idList.ToArray());
    }
}