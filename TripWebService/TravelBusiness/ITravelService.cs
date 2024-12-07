using TripWebData;
using TripWebData.Dtos;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Inputs;

namespace TripWebService.TravelBusiness
{
    public interface ITravelService 
    {
        /// <summary>
        /// 旅游景点查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Results<PageDto<TravelDto>>> TravelQuery(TravelQueryInput input);

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="travelId"></param>
        /// <returns></returns>
        Results<TravelDetailDto> GetDetail(long travelId);

        /// <summary>
        /// 添加或者修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Results<int>> TravelAddOrUpdate(TravelAddOrUpdateInput input);

        /// <summary>
        /// 删除旅游景点
        /// </summary>
        /// <param name="roleId">当前登录者的角色ID</param>
        /// <param name="idList">要删除的ID列表</param>
        /// <returns></returns>
        Task<Results<int>> TravelDelete(long roleId,params long[] idList);

        /// <summary>
        /// 旅游景点审批
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Results<int>> TravelAudit(TravelAuditInput input);

        /// <summary>
        /// 上架或者下架旅游景点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Results<int>> TravelShelveOrUnShelve(TravelShelveOrUnShelveInput input);


        /// <summary>
        /// 设置领队
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Results<int> SetTravelLeader(SetTravelLeaderInput input);
    
    
        /// <summary>
        /// 查看游客列表
        /// </summary>
        /// <returns></returns>
        Results<QueryTravelUserDto> QueryTravelUserList(TravelUserQueryInput input);
    }
}
