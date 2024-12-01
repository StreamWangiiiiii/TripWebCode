using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Entity;

namespace TripWebData.Profiles
{
    /// <summary>
    /// 和旅游相关的AutoMap映射
    /// </summary>
    public class TravelProfile:Profile
    {
        public TravelProfile()
        {
            CreateMap<TabCategory, CategoryDto>();
        }
    }
}
