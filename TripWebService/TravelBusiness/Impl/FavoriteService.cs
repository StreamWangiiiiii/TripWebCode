using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripWebData;
using TripWebData.Dtos;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Entity;
using TripWebData.Inputs;

namespace TripWebService.TravelBusiness.Impl;

public class FavoriteService:BaseService,IFavoriteService
{
    /// <summary>
    /// 添加收藏夹
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public Results<int> AddFavorite(AddFavoriteInput input)
    {
        var count = TripWebContext.TabFavorites.Count(p=>p.CreatedUserId == input.LoginUserId && p.TravelId==input.TravelId && !p.Deleted);
        if (count>0)
        {
            return Results<int>.FailResult("请不要重复收藏");
        }
        TripWebContext.TabFavorites.Add(new()
        {
            CreatedUserId = input.LoginUserId,
            TravelId = input.TravelId,
            UpdatedUserId = input.LoginUserId,
            Id = SnowIdWorker.NextId()
        });
        var tabTravel = TripWebContext.TabTravels.Find(input.TravelId);
        tabTravel.FavoriteCount += 1;

        var save = false;
        int row = 0;
        while (!save)
        {
            try
            {
                row = TripWebContext.SaveChanges();
                save = true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                foreach (var entry in e.Entries)
                {
                    if (entry.Entity is TabTravel)
                    {
                        var databaseValues = entry.GetDatabaseValues();
                        var currentValues = entry.CurrentValues;
                        foreach (var property in currentValues.Properties)
                        {
                            if (property.Name == nameof(tabTravel.FavoriteCount))
                            {
                                currentValues[property] = Convert.ToInt32(databaseValues[property]) + 1;
                            }
                        }
                        
                        // 刷新原始值以绕过下一次并发检查 
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                }
            }
           
        }

        return Results<int>.DataResult(row);
    }

    /// <summary>
    /// 查看我的收藏
    /// </summary>
    /// <returns></returns>
    public Results<PageDto<FavoriteDto>> GetFavoriteList(FavoriteQueryInput input)
    {
        var tabFavorites = TripWebContext.TabFavorites.AsNoTracking().
            Include(p=>p.Travel).ThenInclude(t=>t.Category)
            .Where(p=>p.CreatedUserId == LoginUserId && !p.Deleted);
        
        PageDto<FavoriteDto> page = new()
        {
            Total = tabFavorites.Count()
        };
        var pageList = tabFavorites.OrderByDescending(p=>p.CreatedTime).Skip((input.PageIndex-1)*input.PageSize).Take(input.PageSize);

        var dtoList = pageList.Select(p => new FavoriteDto
        {
            Id = p.Id,
            CategoryName = p.Travel.Category.CategoryName,
            CreatedTime = p.CreatedTime.Value,
            TravelName = p.Travel.TravelName,
            TravelId = p.TravelId,
            Price = p.Travel.Price
        }).ToList();

        page.Data = dtoList;
        
        return Results<PageDto<FavoriteDto>>.DataResult(page);
    }

    /// <summary>
    /// 移除收藏夹
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Results<int> Remove(long id)
    {
        var entity = TripWebContext.TabFavorites.FirstOrDefault(p=>p.Id == id && !p.Deleted);
        if (entity ==null)
        {
            return Results<int>.FailResult("数据不存在");
        }

        entity.Deleted = true;
        var travel = TripWebContext.TabTravels.Find(entity.TravelId);
        travel.FavoriteCount -= 1;

        int row = 0;
        bool save = false;
        while (!save)
        {
            try
            {
                row = TripWebContext.SaveChanges();
                save = true;
            }
            catch (DbUpdateConcurrencyException e)
            {
                foreach (var entry in e.Entries)
                {
                    if (entry.Entity is TabTravel)
                    {
                        var databaseValues = entry.GetDatabaseValues();
                        var currentValues = entry.CurrentValues;
                        foreach (var property in currentValues.Properties)
                        {
                            if (property.Name == nameof(travel.FavoriteCount))
                            {
                                currentValues[property] = Convert.ToInt32(databaseValues[property]) - 1;
                            }
                        }
                        
                        // 刷新原始值以绕过下一次并发检查 
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                }
            }
           
        }
        

        return Results<int>.DataResult(row);
    }
}