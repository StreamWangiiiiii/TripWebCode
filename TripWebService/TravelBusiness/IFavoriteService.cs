using TripWebData;
using TripWebData.Dtos;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Inputs;

namespace TripWebService.TravelBusiness;

/// <summary>
/// 我的收藏相关服务
/// </summary>
public interface IFavoriteService
{
    /// <summary>
    /// 添加收藏夹
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Results<int> AddFavorite(AddFavoriteInput input);
    
    /// <summary>
    /// 查看我的收藏
    /// </summary>
    /// <returns></returns>
    Results<PageDto<FavoriteDto>> GetFavoriteList(FavoriteQueryInput input);

    /// <summary>
    /// 移除收藏夹
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Results<int> Remove(long id);
}