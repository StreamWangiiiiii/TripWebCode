using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Entity;
using TripWebData.Inputs;

namespace TripWebData.Profiles
{
    /// <summary>
    /// 和旅游相关的AutoMap映射
    /// </summary>
    public class TravelProfile:Profile
    {
        public TravelProfile()
        {
            CreateMap<TabCategory, CategoryDto>().
                ForMember(p=>p.UpdatedUserName,opt=>opt.MapFrom(src=>src.UpdatedUser.Nickname));

            CreateMap<CategoryInput, TabCategory>()
                .ForMember(p=>p.UpdatedUserId, opt=>opt.MapFrom(src=>src.LoginUserId))
                .ForMember(p=>p.CreatedUserId, opt=>opt.MapFrom(src=>src.LoginUserId));


            CreateMap<TabTravel, TravelDto>()
                .ForMember(p=>p.CategoryName,opt=>opt.MapFrom(src=>src.Category.CategoryName));

            CreateMap<TabTravel, TravelDetailDto>()
                .ForMember(p=>p.CategoryName,opt=>opt.MapFrom(src=>src.Category.CategoryName));
            CreateMap<TabTravelImg, TravelImageDto>();
        
            CreateMap<TravelAddOrUpdateInput, TabTravel>()
                .ForMember(p=>p.CreatedUserId,opt=>opt.MapFrom(src=>src.LoginUserId))
                .ForMember(p=>p.UpdatedUserId,opt=>opt.MapFrom(src=>src.LoginUserId));
        
            CreateMap<TravelImageInput, TabTravelImg>();
        
            CreateMap<TravelUserDto, TabUser>().ReverseMap();
        
            // 设置领队
            CreateMap<TabTravelLeader, LeaderUserDto>();
        }
    }
}
