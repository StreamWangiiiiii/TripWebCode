﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripWebData.Dtos.TravelBusiness;
using TripWebData;
using TripWebData.Dtos;
using TripWebData.Inputs;

namespace TripWebService.User
{
    /// <summary>
    /// 用户相关接口服务
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 登录：手机，邮箱或用户名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Results<UserDto> Login(LoginInput input);
    }
}