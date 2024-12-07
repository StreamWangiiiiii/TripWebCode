using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripWebData.Dtos;
using TripWebData;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Entity;
using TripWebData.Enums;
using TripWebData.Inputs;
using TripWebUtils.Utils.Snowflake;

namespace TripWebService.TravelBusiness.Impl
{
    /// <summary>
    /// 旅游相关服务实现类
    /// </summary>
    public class TravelService : BaseService,ITravelService
    {
        /// <summary>
        /// 旅游景点查询
        /// </summary>
        public async Task<Results<PageDto<TravelDto>>> TravelQuery(TravelQueryInput input)
        {
            var query = TripWebContext.TabTravels.AsNoTracking()
                .Include(p=>p.Category)
                .Where(p=>!p.Deleted);

            if(input.CategoryId > 0)
            {
                query = query.Where(p=>p.CategoryId == input.CategoryId);
            }

            if(input.StartLaunchedTime.HasValue && !input.StartLaunchedTime.Value.ToString().Contains("1991-01-01"))
            { 
                query = query.Where(p => p.LaunchedTime > input.StartLaunchedTime);
            }

            if (input.EndLaunchedTime.HasValue && !input.EndLaunchedTime.Value.ToString().Contains("1991-01-01"))
            {
                query = query.Where(p => p.LaunchedTime <= input.EndLaunchedTime);
            }

            if (!string.IsNullOrWhiteSpace(input.TravelName))
            {
                query = query.Where(p => p.TravelName.Contains(input.TravelName));
            }

            if (input.StartPrice>0 && input.EndPrice>0)
            {
                query = query.Where(p => p.Price >= input.StartPrice && p.Price <= input.EndPrice);
            }

            if (input.IsLaunched.HasValue)
            {
                query = query.Where(p => p.IsLaunched == input.IsLaunched);
            }

            if (input.AuditStatus.HasValue)
            {
                query = query.Where(p => p.AuditStatus == input.AuditStatus);
            }

            if (input.IsUserSearch)
            {
                query = query.Where(p => p.StartTime >= DateTime.Now);
            }

            if (input.CreatedUserId > 0) 
            {
                query = query.Where(p => p.CreatedUserId == input.CreatedUserId);
            }

            PageDto<TravelDto> result = new()
            {
                Total = query.Count()
            };

            var list = query.OrderByDescending(p => p.StartTime)
                .Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize).ToList() ;

            result.Data = ObjectMapper.Map<List<TravelDto>>(list);

            // 查询用户
            var createUserIdList = list.Select(p => p.CreatedUserId);
            Dictionary<long,TabUser> dict = 
                TripWebContext.TabUsers.Where(p => createUserIdList.Contains(p.Id)).ToDictionary(p => p.Id);
            foreach (var travel in result.Data)
            {
                travel.SellerUserName = dict[travel.CreatedUserId].Nickname;
            }

            return Results<PageDto<TravelDto>>.DataResult(result);
        }
        
        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="travelId"></param>
        /// <returns></returns>
        public Results<TravelDetailDto> GetDetail(long travelId)
        {
            var travel = TripWebContext.TabTravels.AsNoTracking().Include(p=>p.Category)
                .FirstOrDefault(p => !p.Deleted && p.Id == travelId);
            if (travel == null)
            {
                return Results<TravelDetailDto>.FailResult("未找到记录");
            }
            if (travel.AuditStatus != AuditStatusEnum.Pass)
            {
                return Results<TravelDetailDto>.FailResult("当前旅游景点审核未通过");
            }
            if (!travel.IsLaunched)
            {
                return Results<TravelDetailDto>.FailResult("当前旅游景点未上架");
            }
            if (travel.EndTime< DateTime.Now)
            {
                return Results<TravelDetailDto>.FailResult("活动时间已过");
            }

            var travelDetailDto = ObjectMapper.Map<TravelDetailDto>(travel);
            // 查询图片列表
            var imgs = TripWebContext.TabTravelImgs.AsNoTracking().Where(p=>p.TravelId == travelId);
            travelDetailDto.ImageList = ObjectMapper.Map<List<TravelImageDto>>(imgs);

            return Results<TravelDetailDto>.DataResult(travelDetailDto);
        }

        /// <summary>
        /// 添加或者修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Results<int>> TravelAddOrUpdate(TravelAddOrUpdateInput input)
        {
            // 验重(只验重当前的商家)
            var count = TripWebContext.TabTravels.AsNoTracking()
                .Count(p => !p.Deleted && p.TravelName == input.TravelName && p.Id != input.Id && p.CreatedUserId==input.LoginUserId);
            if (count > 0)
            {
                return Results<int>.FailResult("旅游路线已存在");
            }

            var tran = TripWebContext.Database.BeginTransaction();
            try
            {
                TabTravel entity;
                if (input.Id > 0)
                {
                    entity = TripWebContext.TabTravels.Find(input.Id);
                    ObjectMapper.Map(input, entity);
                }
                else
                {
                    entity = ObjectMapper.Map<TabTravel>(input);
                    entity.Id = SnowIdWorker.NextId();
                    entity.AuditStatus = AuditStatusEnum.UnAudit;
                    entity.IsLaunched = false;
                    entity.FavoriteCount = 0;

                    TripWebContext.TabTravels.Add(entity);
                }

                // 操作旅游图片
                if (input.ImageList.Any())
                {
                    // 先清理掉这条数据下所有的图片（先教大家粗暴的办法解决）
                    var imgList = TripWebContext.TabTravelImgs.Where(p => p.TravelId == input.Id);
                    if (imgList.Any())
                    {
                        TripWebContext.TabTravelImgs.RemoveRange(imgList);
                    }

                    var newImageList = ObjectMapper.Map<List<TabTravelImg>>(input.ImageList);
                    foreach (var tabTravelImg in newImageList)
                    {
                        tabTravelImg.Id = SnowIdWorker.NextId();
                        tabTravelImg.CreatedUserId = input.LoginUserId;
                        tabTravelImg.UpdatedUserId = input.LoginUserId;
                        tabTravelImg.TravelId = entity.Id;

                        TripWebContext.TabTravelImgs.Add(tabTravelImg);
                    }
                }

                var row = await TripWebContext.SaveChangesAsync();
                await tran.CommitAsync();
                return Results<int>.DataResult(row);

            }
            catch (Exception e)
            {
                await tran.RollbackAsync();
                return Results<int>.FailResult(e.Message);
            }
            finally
            {
                tran.Dispose();
            }
        }

        /// <summary>
        /// 删除旅游景点
        /// </summary>
        /// <param name="roleId">当前登录者的角色ID</param>
        /// <param name="idList">要删除的ID列表</param>
        /// <returns></returns>
        public async Task<Results<int>> TravelDelete(long roleId, params long[] idList)
        {
            if (!idList.Any())
            {
                return Results<int>.FailResult("必须选中一条记录");
            }

            var list = TripWebContext.TabTravels.Where(p => !p.Deleted && idList.Contains(p.Id));
            if (!list.Any())
            {
                return Results<int>.FailResult("记录不存在");
            }
            foreach (var tabTravel in list)
            {
                if (tabTravel.IsLaunched)
                {
                    return Results<int>.FailResult("存在未下架的商品");
                }

                if ((RoleEnum)roleId == RoleEnum.Seller && tabTravel.CreatedUserId !=LoginUserId)
                {
                    return Results<int>.FailResult("商家只能删除自家商品");
                }

                tabTravel.Deleted = true;
                var imageList = TripWebContext.TabTravelImgs.Where(p=>p.TravelId == tabTravel.Id);
                foreach (var tabTravelImg in imageList)
                {
                    tabTravelImg.Deleted = true;
                }
            }

            return Results<int>.DataResult(await TripWebContext.SaveChangesAsync());
        }

        /// <summary>
        /// 旅游景点审批
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Results<int>> TravelAudit(TravelAuditInput input)
        {
            var list = TripWebContext.TabTravels.Where(p =>
                !p.Deleted && input.TravelIdList.Contains(p.Id) && p.AuditStatus == AuditStatusEnum.UnAudit);
            if (!list.Any())
            {
                return Results<int>.FailResult("请选择待审批的记录");
            }

            foreach (var tabTravel in list)
            {
                if (input.HasPass)
                {
                    tabTravel.IsLaunched = true;
                    tabTravel.AuditStatus = AuditStatusEnum.Pass;
                    tabTravel.LaunchedTime = DateTime.Now;
                }
                else
                {
                    tabTravel.AuditStatus = AuditStatusEnum.Refuse;
                }

                tabTravel.UpdatedUserId = input.LoginUserId;
            }
            
            
            return Results<int>.DataResult(await TripWebContext.SaveChangesAsync());
        }


        /// <summary>
        /// 上架或者下架旅游景点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Results<int>> TravelShelveOrUnShelve(TravelShelveOrUnShelveInput input)
        {
            var list = TripWebContext.TabTravels.Where(p => !p.Deleted && input.TravelIdList.Contains(p.Id)
                                                                 && p.IsLaunched!=input.HasShelve);
            if (!list.Any())
            {
                return Results<int>.FailResult($"您选中的数据未存在{(input.HasShelve ? "上架" : "下架")}");
            }
            
            foreach (var tabTravel in list)
            {
                if (input.HasShelve)
                {
                    tabTravel.IsLaunched = true;
                    tabTravel.LaunchedTime = DateTime.Now;
                }
                else
                {
                    tabTravel.IsLaunched = false;
                    tabTravel.LaunchedTime = null;
                }
                tabTravel.UpdatedUserId = input.LoginUserId;
            }
            
            return Results<int>.DataResult(await TripWebContext.SaveChangesAsync());
        }

        /// <summary>
        /// 设置领队
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Results<int> SetTravelLeader(SetTravelLeaderInput input)
        {
            var oldList = TripWebContext.TabTravelLeaders.Where(p=>!p.Deleted && p.TravelId == input.TravelId);
            if (oldList.Any())
            {
                foreach (var tabTravelLeader in oldList)
                {
                    tabTravelLeader.Deleted = true;
                    tabTravelLeader.UpdatedUserId = LoginUserId;
                }
            }

            var tabUsers = TripWebContext.TabUsers.Where(p=>!p.Deleted && input.UserIdList.Contains(p.Id));
            foreach (var tabUser in tabUsers)
            {
                TabTravelLeader entity = new()
                {
                    Id = SnowIdWorker.NextId(),
                    CreatedUserId = LoginUserId,
                    UpdatedUserId = LoginUserId,
                    LeaderMobile = tabUser.Mobile,
                    LeaderNickname = tabUser.Nickname,
                    LeaderUserId = tabUser.Id,
                    TravelId = input.TravelId
                };
                TripWebContext.TabTravelLeaders.Add(entity);
            }
            
            return Results<int>.DataResult(TripWebContext.SaveChanges());
        }

        /// <summary>
        /// 查看游客列表
        /// </summary>
        /// <returns></returns>
        public Results<QueryTravelUserDto> QueryTravelUserList(TravelUserQueryInput input)
        {

            QueryTravelUserDto result = new();
            
            var userOrderList = TripWebContext.TabOrders.AsNoTracking()
                .Where(p => !p.Deleted && p.TravelId == input.TravelId);
            

            if (!userOrderList.Any())
            {
                return Results<QueryTravelUserDto>.FailResult("没有下单用户");
            }

            // var details = TripWebContext.TabOrderDetails.AsNoTracking()
            //     .Where(p => userOrderList.Select(i => i.Id).Contains(p.OrderId));
            //
            // result.UserList.AddRange(details.Select(p => new TravelUserDto
            // {
            //     Username = "亲友",
            //     Nickname = p.FriendName,
            //     Mobile = p.FriendMobile,
            //     Email = string.Empty
            // }));
            
            // 下单人的信息
            var tabUsers = TripWebContext.TabUsers.AsNoTracking().
                Where(p=>!p.Deleted && userOrderList.Select(p=>p.CreatedUserId).Contains(p.Id));
            result.UserList.AddRange(ObjectMapper.Map<List<TravelUserDto>>(tabUsers));


            var tabTravelLeaders = TripWebContext.TabTravelLeaders.AsNoTracking().Where(p=>!p.Deleted && p.TravelId == input.TravelId);
            if (tabTravelLeaders.Any())
            {
                result.LeaderList.AddRange(ObjectMapper.Map<List<LeaderUserDto>>(tabTravelLeaders));
            }
            
            return Results<QueryTravelUserDto>.DataResult(result);
        }
    }
}
