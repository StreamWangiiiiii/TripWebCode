using TripWebUtils.Utils.RedisUtil;
using TripWebUtils.Utils.Snowflake;
using TripWebData.Dtos.TravelBusiness;
using TripWebData;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripWebData.Consts;

namespace TripWebService.TravelBusiness.Impl
{
    public class CategoryService : BaseService,ICategoryService
    {
        /// <summary>
        /// 查询所有分类
        /// </summary>
        /// <returns></returns>
        public Results<List<CategoryDto>> Getlist()
        {
            var list = CacheManager.GetOrSet(RedisKey.AllCategoryList, () =>
            {
                var tabCategories = TripWebContext.TabCategories.Where(p => !p.Deleted);
                return ObjectMapper.Map<List<CategoryDto>>(tabCategories);
            },TimeSpan.FromDays(10));

            return Results<List<CategoryDto>>.DataResult(list);
        }

        public Results<CategoryDto> GetCategory(long id)
        {
            var entity = TripWebContext.TabCategories.AsNoTracking().FirstOrDefault(p => p.Id == id);
            if (entity == null){
                return Results<CategoryDto>.FailResult("未找到记录");
            }
            return Results<CategoryDto>.DataResult(ObjectMapper.Map<CategoryDto>(entity));
        }
    }
}
