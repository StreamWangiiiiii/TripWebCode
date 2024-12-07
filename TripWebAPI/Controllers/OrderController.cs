using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TripWebAPI.Filters;
using TripWebAPI.Hubs;
using TripWebData;
using TripWebData.Consts;
using TripWebData.Dtos;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Inputs;
using TripWebService.TravelBusiness;
using TripWebAPI.Controllers;


namespace TripWeb.API.Controllers;

/// <summary>
/// 订单管理
/// </summary>
[Route("[controller]/[action]")]
[ApiController]
public class OrderController:BaseController
{
    private readonly IHubContext<TripWebHub> _travelHub;
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService, IHubContext<TripWebHub> travelHub)
    {
        _orderService = orderService;
        _travelHub = travelHub;
    }

    /// <summary>
    /// 创建订单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    [CapSubscribe(EventBus.OrderAdd,Group = EventBus.OrderAdd)]
    [NonAction]
    public async Task OrderAdd(OrderAddInput input)
    {
        var result = await _orderService.OrderAdd(input);

        await _travelHub.Clients.Client(input.ConnectionId).SendAsync(SignalMethod.ReceiveMessage, result);
    }

    /// <summary>
    /// 订单查询-管理员端
    /// </summary>
    /// <remarks>管理员端调用</remarks>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [TripWebAuthorize(AuthorizeRoleName.Administrator)]
    public async Task<Results<PageDto<OrderDto>>> OrderQuery([FromBody]OrderQueryInput input)
    {
        return await _orderService.OrderQuery(input);
    }
    
    
    /// <summary>
    /// 订单查询-个人中心
    /// </summary>
    /// <remarks>个人中心调用</remarks>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [TripWebAuthorize(AuthorizeRoleName.TravelUser)]
    public async Task<Results<PageDto<OrderDto>>> UserOrderQuery([FromBody]OrderQueryInput input)
    {
        input.LoginUserId = LoginUserId;
        return await _orderService.OrderQuery(input);
    }
    
    /// <summary>
    /// 订单查询-商家端
    /// </summary>
    /// <remarks>个商家端调用</remarks>
    /// <param name="input"></param>
    /// <returns></returns>
    [HttpPost]
    [TripWebAuthorize(AuthorizeRoleName.SellerAdministrator)]
    public async Task<Results<PageDto<OrderDto>>> SellerOrderQuery([FromBody]OrderQueryInput input)
    {
        input.SellerUserId = LoginUserId;
        return await _orderService.OrderQuery(input);
    }
}