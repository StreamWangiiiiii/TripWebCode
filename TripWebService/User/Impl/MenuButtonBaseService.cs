using Microsoft.EntityFrameworkCore;
using TripWebData;
using TripWebData.Dtos;
using TripWebData.Entity;
using TripWebData.Inputs;

namespace TripWebService.User.Impl;

public class MenuButtonBaseService:BaseService,IMenuButtonService
{

    /// <summary>
    /// 添加或者修改 菜单按钮
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public Results<int> UpdateOrAdd(MenuButtonInput input)
    {
        var count = TripWebContext.TabMenuButtons
            .Count(p=>input.MenuModuleId==input.MenuModuleId && p.ButtonName == input.ButtonName && !p.Deleted);
        if (count > 0)
        {
            return Results<int>.FailResult("按钮已存在");
        }

        if (input.Id>0)
        {
            var entity = TripWebContext.TabMenuButtons.Find(input.Id);
            ObjectMapper.Map(input, entity);
        }
        else
        {
            input.Id = SnowIdWorker.NextId();
            var entity = ObjectMapper.Map<TabMenuButton>(input);
            TripWebContext.TabMenuButtons.Add(entity);
        }
        
        return Results<int>.DataResult(TripWebContext.SaveChanges());
    }

    /// <summary>
    /// 菜单按钮查询
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public Results<PageDto<MenuButtonDto>> MenuButtonQuery(MenuButtonQueryInput input)
    {
        var tabMenuButtons = TripWebContext.TabMenuButtons
            .AsNoTracking().Include(p=>p.MenuModule)
            .Where(p=>!p.Deleted);
        if (input.MenuModuleId.HasValue && input.MenuModuleId.Value>0)
        {
            tabMenuButtons = tabMenuButtons.Where(p => p.MenuModuleId == input.MenuModuleId);
        }

        PageDto<MenuButtonDto> page = new()
        {
            Total = tabMenuButtons.Count()
        };

        var list = tabMenuButtons.OrderByDescending(p => p.UpdatedTime).Skip((input.PageIndex - 1) * input.PageSize)
            .Take(input.PageSize).ToList();
        page.Data = ObjectMapper.Map<List<MenuButtonDto>>(list);

        return Results<PageDto<MenuButtonDto>>.DataResult(page);
    }
    
    
    /// <summary>
    /// 根据ID获取一条记录
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Results<MenuButtonDto> GetById(long id)
    {
        var entity = TripWebContext.TabMenuButtons.AsNoTracking().FirstOrDefault(p=>!p.Deleted && p.Id == id);
        if (entity == null)
        {
            return Results<MenuButtonDto>.FailResult("记录不存在");
        }
        
        return Results<MenuButtonDto>.DataResult(ObjectMapper.Map<MenuButtonDto>(entity));
    }

    /// <summary>
    /// 批量删除
    /// </summary>
    /// <param name="idList"></param>
    /// <returns></returns>
    public Results<int> Remove( params long[] idList)
    {
        if (!idList.Any())
        {
            return Results<int>.FailResult("必须选择一条记录");
        }

        var entities = TripWebContext.TabMenuButtons.Where(p=>idList.Contains(p.Id) && !p.Deleted);
        if (!entities.Any())
        {
            return Results<int>.FailResult("数据不存在");
        }
        

        foreach (var entity in entities)
        {
            entity.Deleted = true;
            entity.UpdatedUserId = LoginUserId;
        }
        
        return Results<int>.DataResult(TripWebContext.SaveChanges());
    }
}