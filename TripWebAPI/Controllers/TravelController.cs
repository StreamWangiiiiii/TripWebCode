using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripWebAPI.Filters;
using TripWebData;
using TripWebData.Consts;
using TripWebData.Dtos;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Inputs;
using TripWebService.TravelBusiness;

namespace TripWebAPI.Controllers
{
    /// <summary>
    /// 旅游管理
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class TravelController : BaseController
    {
        /// <summary>
        /// 旅游管理
        /// </summary>
        private readonly ITravelService _travelService;
        public TravelController(ITravelService travelService)
        {
            _travelService = travelService;
        }

        /// <summary>
        /// 用户端-旅游景点查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<Results<PageDto<TravelDto>>> UserTravelQuery([FromBody]TravelQueryInput input)
        {
            input.IsUserSearch = true;
            return await _travelService.TravelQuery(input);
        }
        
        /// <summary>
        /// 商家端-旅游景点查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [TripWebAuthorize(AuthorizeRoleName.SellerAdministrator)]
        public async Task<Results<PageDto<TravelDto>>> SellerTravelQuery([FromBody]TravelQueryInput input)
        {
            input.CreatedUserId = LoginUserId;
            return await _travelService.TravelQuery(input);
        }
        
        /// <summary>
        /// 管理端-旅游景点查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [TripWebAuthorize(AuthorizeRoleName.Administrator)]
        public async Task<Results<PageDto<TravelDto>>> AdminTravelQuery([FromBody]TravelQueryInput input)
        {
            return await _travelService.TravelQuery(input);
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="travelId">景点ID</param>
        /// <remarks>无需要登录</remarks>
        /// <returns></returns>
        [HttpGet("{travelId}")]
        [AllowAnonymous]
        public Results<TravelDetailDto> GetDetail(long travelId)
        {
            return _travelService.GetDetail(travelId);
        }

        /// <summary>
        /// 添加或者修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [TripWebAuthorize(AuthorizeRoleName.SellerAdministrator)]
        public async Task<Results<int>> TravelAddOrUpdate([FromBody]TravelAddOrUpdateInput input)
        {
            return await _travelService.TravelAddOrUpdate(input);
        }


        /// <summary>
        /// 删除旅游景点
        /// </summary>
        /// <param name="idList">要删除的ID列表</param>
        /// <remarks>只有商家才可操作</remarks>
        /// <returns></returns>
        [TripWebAuthorize(AuthorizeRoleName.SellerAdministrator)]
        [HttpDelete]
        public async Task<Results<int>> TravelDelete([FromBody]List<long> idList)
        {
            return await _travelService.TravelDelete(CurrentRoleId, idList.ToArray());
        }

        /// <summary>
        /// 旅游景点审批
        /// </summary>
        /// <param name="input"></param>
        /// <remarks>管理员才能审核</remarks>
        /// <returns></returns>
        [TripWebAuthorize(AuthorizeRoleName.Administrator)]
        [HttpPost]
        public async Task<Results<int>> TravelAudit([FromBody]TravelAuditInput input)
        {
            return await _travelService.TravelAudit(input);
        }

        /// <summary>
        /// 上架或者下架旅游景点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [TripWebAuthorize(AuthorizeRoleName.AdminOrSeller)]
        [HttpPost]
        public async Task<Results<int>> TravelShelveOrUnShelve([FromBody]TravelShelveOrUnShelveInput input)
        {
            return await _travelService.TravelShelveOrUnShelve(input);
        }


        /// <summary>
        /// 设置领队
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [TripWebAuthorize(AuthorizeRoleName.SellerAdministrator)]
        public Results<int> SetTravelLeader([FromBody]SetTravelLeaderInput input)
        {
            return _travelService.SetTravelLeader(input);
        }

        /// <summary>
        /// 查看游客列表
        /// </summary>
        /// <returns></returns>
        [TripWebAuthorize(AuthorizeRoleName.SellerAdministrator)]
        [HttpPost]
        public Results<QueryTravelUserDto> QueryTravelUserList([FromBody]TravelUserQueryInput input)
        {
            return _travelService.QueryTravelUserList(input);
        }
        
        /// <summary>
        /// 导出游客列表
        /// </summary>
        /// <returns></returns>
        [TripWebAuthorize(AuthorizeRoleName.SellerAdministrator)]
        [HttpPost]
        public Results<QueryTravelUserDto> ExportTravelUserList([FromBody]TravelUserQueryInput input)
        {
            return _travelService.QueryTravelUserList(input);
        }
    }
}
