using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripWebData;
using TripWebData.Dtos;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Entity;
using TripWebData.Inputs;

namespace TripWebService.TravelBusiness.Impl;

/// <summary>
/// 订单服务实现类
/// </summary>
public class OrderService:BaseService,IOrderService
{
   

    /// <summary>
    /// 创建订单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<Results<string>> OrderAdd(OrderAddInput input)
    {
        // 1.检查是否重复下单
        var count = TripWebContext.TabOrders.Count(p=>!p.Deleted && p.CreatedUserId == input.LoginUserId && p.TravelId == input.TravelId);
        if (count>0)
        {
            return Results<string>.FailResult("请不要重复下单");
        }

        // 2. 是否超出人数限制
        
        // 2.1 查询人数限制
        var travel = TripWebContext.TabTravels.FirstOrDefault(p=>!p.Deleted && p.Id==input.TravelId);
        // 2.2 查询当前已经下单的人数
        var details = TripWebContext.TabOrderDetails.Where(p=>p.TravelId == input.TravelId);
        // 已经下单的人数 = 亲友人数+订单数
        var currentPersonCount = travel.HaveOrdered;
        
        var currentOrderPerson = currentPersonCount + input.Friends.Count + 1;
        if (currentOrderPerson>travel.PersonLimit)
        {
            return Results<string>.FailResult($"此景点最多能接待 {travel.PersonLimit} 人数");
        }

        TripWebContext.Database.ExecuteSqlInterpolated($"update tab_travel set have_ordered=15 where id={travel.Id}");
        travel.HaveOrdered = currentOrderPerson; // 更新订购人数
        
        
        var orderId = SnowIdWorker.NextId();
        TripWebContext.TabOrders.Add(new TabOrder()
        {
            TravelId = input.TravelId,
            CreatedUserId = input.LoginUserId,
            UpdatedUserId = input.LoginUserId,
            Id = orderId
        });

        if (input.Friends.Any())
        {
            foreach (var item in input.Friends)
            {
                var detailEntity = new TabOrderDetail()
                {
                    Id = SnowIdWorker.NextId(),
                    FriendMobile = item.Mobile,
                    FriendName = item.NickName,
                    OrderId = orderId,
                    OrderUserId = input.LoginUserId
                };
                TripWebContext.TabOrderDetails.Add(detailEntity);
            }
        }

        var save = false;

        var orderdFlag = false; // 标识是否可以下单
        
        while (!save)
        {
            try
            {
                await TripWebContext.SaveChangesAsync();
                save = true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                foreach (var entry in e.Entries)
                {
                    if (entry.Entity is TabTravel)
                    {
                        var databaseValues = entry.GetDatabaseValues(); // 出现并发时，数据库中的值
                        var proposedValues = entry.CurrentValues; // 当前实体中的值
                    
                        foreach (var property in proposedValues.Properties)
                        {
                            if (property.Name==nameof(travel.HaveOrdered))
                            {
                                var proposedValue = proposedValues[property]; // 当前实体HaveOrdered的值
                                var databaseValue = databaseValues[property];// 数据库中HaveOrdered的值
                                if (Convert.ToInt32(databaseValue)+details.Count()+1>=travel.PersonLimit)
                                {
                                    orderdFlag = false;
                                    // 决定哪个值应该写入数据库 
                                    proposedValues[property] = databaseValue;
                                }
                                else
                                {
                                    orderdFlag = true; // 标识下单成功
                                    proposedValues[property] = input.Friends.Count + 1+ Convert.ToInt32(databaseValues[property]);
                                }
                            }
                        }
                        // 刷新原始值以绕过下一次并发检查 
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                
                }
            }   
        }

        if (orderdFlag)
        {
            return Results<string>.GetResult(msg:"下单成功");
        }
        return Results<string>.FailResult("下单失败, 人数超出限制");
    }

    /// <summary>
    /// 订单查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<Results<PageDto<OrderDto>>> OrderQuery(OrderQueryInput input)
    {
        PageDto<OrderDto> result = new();

        
         // 查询字段：订单ID,景点名称，价格(订单总价),下单时间，所属商家，订单详情
         var query = from o in TripWebContext.TabOrders.AsNoTracking()
             join t in TripWebContext.TabTravels.AsNoTracking() on o.TravelId equals t.Id
             select new
             {
                 o.Id,
                 t.TravelName,
                 t.Price,
                 o.CreatedTime,
                 o.CreatedUserId, // 下单人ID
                 SellerUserId= t.CreatedUserId  // 商家ID
             };

         if (!string.IsNullOrWhiteSpace(input.TravelName))
         {
             query = query.Where(p => p.TravelName.Contains(input.TravelName));
         }

         if (input.StartTime.HasValue 
             && !input.StartTime.ToString()!.StartsWith("1991")
             && input.EndTime.HasValue
             && !input.EndTime.ToString()!.StartsWith("1991"))
         {
             query = query.Where(p => p.CreatedTime > input.StartTime.Value 
                                      && p.CreatedTime < input.EndTime.Value);
         }

         if (input.SellerUserId.HasValue)
         {
             query = query.Where(p => p.SellerUserId == input.SellerUserId);
         }
         if (input.LoginUserId.HasValue)
         {
             query = query.Where(p => p.CreatedUserId == input.LoginUserId);
         }

         result.Total = query.Count();

         // 分页
         var list = query.OrderByDescending(p => p.CreatedTime).Skip((input.PageIndex - 1) * input.PageSize)
             .Take(input.PageSize)
             .Select(p => new OrderDto
             {
                  Id = p.Id,
                  CreatedTime = p.CreatedTime.Value,
                  CreatedUserId = p.CreatedUserId,
                  TotalPrice = p.Price,
                  TravelName = p.TravelName
             }).ToList();


         // if (list.Any())
         // {
         //     var details = TripWebContext.TabOrderDetails.AsNoTracking()
         //         .Where(p => list.Select(o => o.Id).Contains(p.OrderId));
         //     var users = TripWebContext.TabUsers.Where(p => list.Select(u => u.CreatedUserId).Contains(p.Id)).ToDictionary(t=>t.Id);
         //     foreach (var orderDto in list)
         //     {
         //         var tabOrderDetails = details.Where(p=>p.OrderId == orderDto.Id).ToList();
         //         orderDto.TotalPrice += tabOrderDetails.Count * orderDto.TotalPrice;
         //         orderDto.FriendList = ObjectMapper.Map<List<OrderDetail>>(tabOrderDetails);
         //         orderDto.UserName = users[orderDto.CreatedUserId].Username;
         //     }
         // }

         result.Data = list;
         return Results<PageDto<OrderDto>>.DataResult(result);
    }
}