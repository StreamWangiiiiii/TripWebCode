using Microsoft.EntityFrameworkCore;
using TripWebData;
using TripWebData.Dtos;
using TripWebData.Entity;
using TripWebData.Inputs;

namespace TripWebService.User.Impl;

/// <summary>
/// 菜单模块服务类
/// </summary>
public class MenuModuleBaseService:BaseService,IMenuModuleService
{
    /// <summary>
    /// 添加或者修改菜单模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public Results<int> UpdateOrAdd(MenuModuleInput input)
    {
        var count = TripWebContext.TabMenuModules
            .Count(p=>p.ParentMenuModuleId==input.ParentMenuModuleId 
                                                     && p.ModuleName == input.ModuleName && !p.Deleted);
        if (count > 0)
        {
            return Results<int>.FailResult("菜单名已存在");
        }

        if (input.Id>0)
        {
            var entity = TripWebContext.TabMenuModules.Find(input.Id);
            ObjectMapper.Map(input, entity);
        }
        else
        {
            input.Id = SnowIdWorker.NextId();
            var entity = ObjectMapper.Map<TabMenuModule>(input);
            TripWebContext.TabMenuModules.Add(entity);
        }
        
        return Results<int>.DataResult(TripWebContext.SaveChanges());
    }

    /// <summary>
    /// 菜单模块查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public Results<PageDto<MenuModuleDto>> MenuModuleQuery(MenuModuleQueryInput input)
    {
        var query = from p in TripWebContext.TabMenuModules.AsNoTracking()
            join t in TripWebContext.TabMenuModules.AsNoTracking() on p.ParentMenuModuleId equals t.Id into grouping
            from g in grouping.DefaultIfEmpty()
            select new MenuModuleDto
            {
                Id = p.Id,
                ModuleName=p.ModuleName,
                LinkUrl=p.LinkUrl,
                IconUrl= p.IconUrl,
                ParentMenuModuleId = p.ParentMenuModuleId,
                ParentMenuModuleName = g.ModuleName,
                UpdatedTime= p.UpdatedTime.Value
            };

        if (input.ParentId.HasValue && input.ParentId.Value>0)
        {
            query = query.Where(p => p.ParentMenuModuleId == input.ParentId);
        }

        if (!string.IsNullOrWhiteSpace(input.MenuName))
        {
            query = query.Where(p => p.ModuleName.Contains(input.MenuName));
        }
        
        PageDto<MenuModuleDto> page = new()
        {
            Total = query.Count()
        };

        var list = query.OrderByDescending(p => p.UpdatedTime).Skip((input.PageIndex - 1) * input.PageSize)
            .Take(input.PageSize)
            .Select(p => new MenuModuleDto
            {
                Id = p.Id,
                ModuleName=p.ModuleName,
                LinkUrl=p.LinkUrl,
                IconUrl= p.IconUrl,
                ParentMenuModuleId = p.ParentMenuModuleId,
                ParentMenuModuleName = p.ParentMenuModuleName,
                UpdatedTime= p.UpdatedTime
            }).ToList();

        page.Data = list;
        
        return Results<PageDto<MenuModuleDto>>.DataResult(page);
    }

    /// <summary>
    /// 根据菜单模块ID获取一条记录
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Results<MenuModuleDto> GetMenuModuleById(long id)
    {
        var entity = TripWebContext.TabMenuModules.AsNoTracking().FirstOrDefault(p=>!p.Deleted && p.Id == id);
        if (entity == null)
        {
            return Results<MenuModuleDto>.FailResult("记录不存在");
        }
        
        return Results<MenuModuleDto>.DataResult(ObjectMapper.Map<MenuModuleDto>(entity));
    }

    /// <summary>
    /// 批量删除
    /// </summary>
    /// <param name="idList"></param>
    /// <returns></returns>
    public Results<int> Remove(params long[] idList)
    {
        if (!idList.Any())
        {
            return Results<int>.FailResult("必须选择一条记录");
        }

        var tabMenuModules = TripWebContext.TabMenuModules.Where(p=>idList.Contains(p.Id) && !p.Deleted);
        if (!tabMenuModules.Any())
        {
            return Results<int>.FailResult("数据不存在");
        }
        
        // 1. 判断需要删除的模块中是否包含有子模块或者子菜单按钮，如果有，则不让删
        var menuIdList = tabMenuModules.Select(t=>t.Id);
        var count = TripWebContext.TabMenuModules.Count(p=>menuIdList.Contains(p.ParentMenuModuleId!.Value));
        if (count>0)
        {
            return Results<int>.FailResult("当前要删除的菜单模块中存在子模块");
        }

        var buttonCount = TripWebContext.TabMenuButtons.Count(p=>menuIdList.Contains(p.MenuModuleId!.Value));
        if (buttonCount>0)
        {
            return Results<int>.FailResult("当前要删除的菜单模块中存在子按钮");
        }

        foreach (var tabMenuModule in tabMenuModules)
        {
            tabMenuModule.Deleted = true;
            tabMenuModule.UpdatedUserId = LoginUserId;
        }
        
        return Results<int>.DataResult(TripWebContext.SaveChanges());
    }
}