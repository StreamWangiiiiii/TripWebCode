using Microsoft.EntityFrameworkCore;
using TripWebUtils.Utils.RedisUtil;
using TripWebData;
using TripWebData.Consts;
using TripWebData.Dtos;
using TripWebData.Entity;
using TripWebData.Inputs;

namespace TripWebService.User.Impl;

/// <summary>
/// 角色权限服务实现类
/// </summary>
public class RoleAuthorityBaseService:BaseService,IRoleAuthorityService
{
    /// <summary>
    /// 为角色分配权限
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public Results<int> DistributeAuthority(RoleAuthorityInput input)
    {
        if (!input.ButtonIdList.Any())
        {
            return Results<int>.FailResult("至少选择一个权限进行分配");
        }
        
        // 1. 查询当前的角色有哪些权限 
        var tabRoleAuthorities = TripWebContext.TabRoleAuthorities
            .AsNoTracking().Where(p=>!p.Deleted && p.RoleId == input.RoleId);
        
        // 2. 对比哪些权限(按钮)是需要新增，哪些权限是需要被移除的
        var oldButtonList = tabRoleAuthorities.Select(p => p.MenuButtonId!.Value).ToList();

        // 2.1 获取哪些是按钮是需要新增的
        // [1,2,3,4] - ButtonIdList
        // [4,5,6] - oldButtonList
        // ButtonIdList.Except(oldButtonList) ====> [1,2,3] 
        
        var needAddButtonIdList = input.ButtonIdList.Except(oldButtonList).ToList();
        if (needAddButtonIdList.Any())
        {
            var needButtonEntities = TripWebContext.TabMenuButtons.AsNoTracking()
                .Where(p=>!p.Deleted && needAddButtonIdList.Contains(p.Id));
            
            foreach (var p in needButtonEntities)
            {
                var tabRoleAuthority = new TabRoleAuthority
                {
                    Id = SnowIdWorker.NextId(),
                    MenuButtonId = p.Id,
                    RoleId = input.RoleId,
                    CreatedUserId = LoginUserId,
                    UpdatedUserId = LoginUserId,
                    MenuModuleId = p.MenuModuleId
                };

                TripWebContext.TabRoleAuthorities.Add(tabRoleAuthority);
            }
        }
        
        
        
        // 2.1 获取哪些是按钮是需要删除的
        // [1,2,3,4] - ButtonIdList
        // [4,5,6] - oldButtonList
        // oldButtonList.Except(ButtonIdList) ====> [5,6]
        var needRemoveButtonIdList = oldButtonList.Except(input.ButtonIdList).ToList();
        if (needRemoveButtonIdList.Any())
        {
            var needRemoveAuthorities = TripWebContext.TabRoleAuthorities
                .Where(p => p.RoleId == input.RoleId && 
                            !p.Deleted && needRemoveButtonIdList.Contains(p.MenuButtonId.Value));
            TripWebContext.TabRoleAuthorities.RemoveRange(needRemoveAuthorities);
        }
        
        CacheManager.Remove(String.Format(RedisKey.RoleMenuList,input.RoleId));
        
        return Results<int>.DataResult(TripWebContext.SaveChanges());
    }


    /// <summary>
    /// 获取左边菜单栏
    /// </summary>
    /// <param name="loginRoleId"></param>
    /// <returns></returns>
    public Results<List<LeftMenuDto>> GetLeftMenuByRoleId(long loginRoleId)
    {
        List<LeftMenuDto> result = CacheManager.GetOrSet(string.Format(RedisKey.RoleMenuList, loginRoleId), () =>
        {

            List<LeftMenuDto> leftMenuDtoList = new();
            // 当前的角色所能看到的菜单Id列表
            var menuIdList = TripWebContext.TabRoleAuthorities.AsNoTracking().Where(p=>!p.Deleted && p.RoleId == loginRoleId)
                .Select(p=>p.MenuModuleId).ToList();
            // 当前的角色所能看到的菜单
            var allMenus = TripWebContext.TabMenuModules.AsNoTracking()
                .Include(p=>p.ParentMenuModule)
                .Where(p=>!p.Deleted && menuIdList.Contains(p.Id));

            var allParentMenu = allMenus.Select(p=>p.ParentMenuModule);
            HashSet<long> parentIdList = new(); // 保存已经构建过的父菜单
            foreach (var parentModule in allParentMenu)
            {
                if (parentIdList.Add(parentModule.Id))
                {
                    LeftMenuDto dto = ObjectMapper.Map<LeftMenuDto>(parentModule);
                    var childMenu = allMenus.Where(p=>p.ParentMenuModuleId == parentModule.Id);
                    dto.Child.AddRange(ObjectMapper.Map<List<LeftMenuDto>>(childMenu));
                
                
                    leftMenuDtoList.Add(dto);
                }
            }

            return leftMenuDtoList;
        }, TimeSpan.FromDays(7));

        return Results<List<LeftMenuDto>>.DataResult(result);
    }
    
    
    // /// <summary>
    // /// 递归构造菜单树
    // /// </summary>
    // /// <param name="allMenus"></param>
    // /// <param name="leftMenuList"></param>
    // /// <param name="parentMenuId"></param>
    //  private void BuildMenuTree(List<TabMenuModule> allMenus, List<LeftMenuDto> leftMenuList, long parentMenuId)
    //  {
    //      var child = allMenus.Where(p=>p.ParentMenuModuleId==parentMenuId).ToList();
    //      if (child.Any())
    //      {
    //          foreach (var menu in child)
    //          {
    //              var leftMenuDto = _mapper.Map<LeftMenuDto>(menu);
    //              BuildMenuTree(allMenus,leftMenuDto.Child,menu.Id);
    //              leftMenuList.Add(leftMenuDto);
    //          }
    //      }
    //  }


    /// <summary>
    /// 获取角色的权限列表
    /// </summary>
    /// <returns></returns>
    public Results<List<AuthorityDto>> GetAuthoritiesByRoleId(long loginRoleId)
    {

        List<AuthorityDto> result = new();
        
        var tabRoleAuthorities = TripWebContext.TabRoleAuthorities.AsNoTracking()
            .Where(p => !p.Deleted && p.RoleId == loginRoleId)
            .Include(p=>p.MenuButton)
            .Include(p => p.MenuModule).ToList();

        var roleAuthorityGroup = tabRoleAuthorities.GroupBy(p=>p.MenuModuleId);
        
        foreach (var g in roleAuthorityGroup)
        {
            AuthorityDto dto = new()
            {
                MenuModuleId = g.Key!.Value,
                MenuModuleName = tabRoleAuthorities.FirstOrDefault(p => p.MenuModuleId == g.Key)!.MenuModule.ModuleName
            };
            dto.Buttons.AddRange(g.ToList().Select(p => new AuthorityButtonDto
            {
                ButtonId = p.MenuButtonId!.Value,
                ButtonName = p.MenuButton.ButtonName
            }));
            result.Add(dto);
        }
        


        return Results<List<AuthorityDto>>.DataResult(result);
    }
    
}