using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripWebData.Dtos;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Entity;
using TripWebData.Inputs;
using TripWebUtils.Utils;

namespace TripWebData.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            
            
            CreateMap<TabUser, UserDto>()
                .ForMember(p => p.Mobile, opt => opt.MapFrom(src => FormatUtil.TxtReplace(src.Mobile,3,4,'*')))
                .ForMember(p => p.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));


            CreateMap<RegisterInput, TabUser>();
            CreateMap<UserProfileInput, TabUser>()
                .ForMember(p => p.Username, opt => opt.Ignore());

            CreateMap<UserAddOrUpdateInput, TabUser>()
                .ForMember(p => p.CreatedUserId, opt => opt.MapFrom(src => src.LoginUserId))
                .ForMember(p => p.UpdatedUserId, opt => opt.MapFrom(src => src.LoginUserId));
        
        
            CreateMap<MenuButtonInput,TabMenuButton>()
                .ForMember(p => p.CreatedUserId, opt => opt.MapFrom(src => src.LoginUserId))
                .ForMember(p => p.UpdatedUserId, opt => opt.MapFrom(src => src.LoginUserId));
        
            CreateMap<MenuModuleInput,TabMenuModule>()
                .ForMember(p => p.CreatedUserId, opt => opt.MapFrom(src => src.LoginUserId))
                .ForMember(p => p.UpdatedUserId, opt => opt.MapFrom(src => src.LoginUserId));


            CreateMap<TabMenuModule, MenuModuleDto>();
            CreateMap<TabMenuButton, MenuButtonDto>()
                .ForMember(p => p.MenuModuleName, opt => opt.MapFrom(src => src.MenuModule.ModuleName));

            CreateMap<TabMenuModule, LeftMenuDto>();
        }
    }
}
