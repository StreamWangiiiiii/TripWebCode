using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripWebData;
using TripWebData.Dtos.TravelBusiness;

namespace TripWebService.TravelBusiness
{
    public interface ICategoryService
    {
        /// <summary>
        /// 查询所有的分类
        /// </summary>
        /// <returns></returns>
        Results<List<CategoryDto>> Getlist();

        /// <summary>
        /// 获取一条分类记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Results<CategoryDto> GetCategory(long id);
    }
}
