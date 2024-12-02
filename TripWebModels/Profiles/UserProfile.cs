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
using Utils.Utils;

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
        }
    }
}
