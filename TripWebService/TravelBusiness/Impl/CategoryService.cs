using Utils.Utils.RedisUtil;
using Utils.Utils.Snowflake;
using TripWebData.Dtos.TravelBusiness;
using TripWebData;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TripWebData.Consts;

namespace TripWebService.TravelBusiness.Impl
{
    public class CategoryService : ICategoryService
    {
        private readonly TripWebContext _context;
        private readonly IMapper _mapper;
        private readonly IdWorker _idWorker;

        public CategoryService(TripWebContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _idWorker = SnowflakeUtil.CreateIdWorker();
        }

        /// <summary>
        /// 查询所有分类
        /// </summary>
        /// <returns></returns>
        public Results<List<CategoryDto>> Getlist()
        {
            Console.WriteLine(_context.Database);
            var list = CacheManager.GetOrSet(RedisKey.AllCategoryList, () =>
            {
                var tabCategories = _context.TabCategories.Include("UpdatedUser").Where(p => !p.Deleted);
                return _mapper.Map<List<CategoryDto>>(tabCategories);
            },TimeSpan.FromDays(10));

            return Results<List<CategoryDto>>.DataResult(list);
        }

        public Results<CategoryDto> GetCategory(long id)
        {
            var entity = _context.TabCategories.AsNoTracking().FirstOrDefault(p => p.Id == id);
            if (entity == null){
                return Results<CategoryDto>.FailResult("未找到记录");
            }
            return Results<CategoryDto>.DataResult(_mapper.Map<CategoryDto>(entity));
        }
    }
}
