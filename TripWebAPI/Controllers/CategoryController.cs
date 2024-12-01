using Microsoft.AspNetCore.Mvc;
using TripWebData.Dtos.TravelBusiness;
using TripWebData;
using Consul;
using TripWebService.TravelBusiness;
using Microsoft.AspNetCore.Authorization;

namespace TripWebAPI.Controllers
{
    /// <summary>
    /// 分类管理
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class CategoryController:BaseController
    {
        private readonly ICategoryService _categoryservice;

        public CategoryController(ICategoryService categoryservice)
        {
            _categoryservice = categoryservice;
        }


        /// <summary>
        /// 查询所有分类
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public Results<List<CategoryDto>> Getlist()
        {
            return _categoryservice.Getlist();
        }

        /// <summary>
        /// 查询单个分类
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public Results<CategoryDto> GetCategory(long id)
        {
            return _categoryservice.GetCategory(id);
        }
    }
}
