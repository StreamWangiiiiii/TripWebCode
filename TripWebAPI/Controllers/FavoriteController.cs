using Microsoft.AspNetCore.Mvc;
using TripWebAPI.Filters;
using TripWebData;
using TripWebData.Consts;
using TripWebData.Dtos;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Inputs;
using TripWebService.TravelBusiness;
using TripWebAPI.Controllers;
using TripWebData.Dtos;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Inputs;
using TripWebService.TravelBusiness;

namespace TripWebAPI.Controllers;

/// <summary>
/// 收藏夹相关服务
/// </summary>
[Route("[controller]/[action]")]
[ApiController]
public class FavoriteController:BaseController
{
    private readonly IFavoriteService _favoriteService;

    public FavoriteController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }


    /// <summary>
    /// 添加收藏夹
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [TripWebAuthorize]
    public Results<int> AddFavorite([FromBody]AddFavoriteInput input)
    {
        return _favoriteService.AddFavorite(input);
    }


    /// <summary>
    /// 查看我的收藏
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [TripWebAuthorizeAttribute]
    public Results<PageDto<FavoriteDto>> GetFavoriteList([FromBody]FavoriteQueryInput input)
    {
        return _favoriteService.GetFavoriteList(input);
    }

    /// <summary>
    /// 移除收藏夹
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [TripWebAuthorizeAttribute]
    public Results<int> Remove(long id)
    {
        return _favoriteService.Remove(id);
    }
}