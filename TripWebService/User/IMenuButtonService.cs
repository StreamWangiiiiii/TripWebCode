using TripWebData;
using TripWebData.Dtos;
using TripWebData.Inputs;

namespace TripWebService.User;

/// <summary>
/// 菜单按钮接口
/// </summary>
public interface IMenuButtonService
{
    /// <summary>
    /// 添加或者修改 菜单按钮
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Results<int> UpdateOrAdd(MenuButtonInput input);

    /// <summary>
    /// 菜单按钮查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Results<PageDto<MenuButtonDto>> MenuButtonQuery(MenuButtonQueryInput input);
    
    /// <summary>
    /// 根据ID获取一条记录
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Results<MenuButtonDto> GetById(long id);
    
    /// <summary>
    /// 批量删除
    /// </summary>
    /// <param name="idList"></param>
    /// <returns></returns>
    Results<int> Remove(params long[] idList);
}