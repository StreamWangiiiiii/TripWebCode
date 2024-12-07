using AutoMapper;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Entity;

namespace TripWebData.Profiles;
/// <summary>
/// 订单相关映射
/// </summary>
public class OrderProfile:Profile
{
    /// <summary>
    /// 
    /// </summary>
    public OrderProfile()
    {
        CreateMap<TabOrderDetail, OrderDetail>();

    }
}