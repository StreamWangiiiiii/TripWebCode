using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TripWebData;
using TripWebData.Dtos;
using TripWebData.Dtos.TravelBusiness;
using TripWebData.Entity;
using TripWebData.Inputs;
using Utils.Utils;
using Utils.Utils.Snowflake;

namespace TripWebService.User.Impl
{
    /// <summary>
    /// 用户相关接口服务实现
    /// </summary>
    public class UserService : IUserService
    {
        private readonly TripWebContext _context;
        private readonly IMapper _mapper;
        private readonly IdWorker _idWorker;

        public UserService(TripWebContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _idWorker = SnowflakeUtil.CreateIdWorker();
        }

        /// <summary>
        /// 登录：手机，邮箱或用户名
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public Results<UserDto> Login(LoginInput input)
        {
            var pwd = DesEncryptUtil.Md5Encrypt(input.Pwd);

            Expression<Func<TabUser, bool>> exp = p =>
                !p.Deleted && p.Username == input.LoginAccount && p.Password == pwd;

            if (FormatUtil.IsPhone(input.LoginAccount))
            {
                exp = p => !p.Deleted && p.Mobile == input.LoginAccount && p.Password == pwd;
            }

            if (FormatUtil.IsEmail(input.LoginAccount))
            {
                exp = p => !p.Deleted && p.Email == input.LoginAccount && p.Password == pwd;
            }

            var user = _context.TabUsers.AsNoTracking().Include(p=>p.Role).FirstOrDefault(exp);

            if (user == null)
            {
                return Results<UserDto>.FailResult("账号或密码输入错误");
            }

            if (!user.ActiveStatus)
            {
                return Results<UserDto>.FailResult("账号未激活");
            }

            return Results<UserDto >.DataResult(_mapper.Map<UserDto>(user));
        }


    }
}
