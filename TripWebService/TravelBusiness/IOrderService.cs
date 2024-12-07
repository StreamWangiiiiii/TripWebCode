using TripWebData;
using TripWebData.Dtos;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Inputs;

namespace TripWebService.TravelBusiness;

/// <summary>
/// 订单相关服务
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// 创建订单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<Results<string>> OrderAdd(OrderAddInput input);

    /// <summary>
    /// 订单查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<Results<PageDto<OrderDto>>> OrderQuery(OrderQueryInput input);
}