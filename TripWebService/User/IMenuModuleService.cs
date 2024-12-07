using TripWebData;
using TripWebData.Dtos;
using TripWebData.Inputs;

namespace TripWebService.User;

/// <summary>
/// 菜单模块相关服务
/// </summary>
public interface IMenuModuleService
{
    /// <summary>
    /// 添加或者修改菜单模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Results<int> UpdateOrAdd(MenuModuleInput input);

    /// <summary>
    /// 菜单模块查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Results<PageDto<MenuModuleDto>> MenuModuleQuery(MenuModuleQueryInput input);

    /// <summary>
    /// 根据菜单模块ID获取一条记录
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Results<MenuModuleDto> GetMenuModuleById(long id);
    
    /// <summary>
    /// 批量删除
    /// </summary>
    /// <param name="idList"></param>
    /// <returns></returns>
    Results<int> Remove(params long[] idList);
}