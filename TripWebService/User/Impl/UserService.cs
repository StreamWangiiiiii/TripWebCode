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
using TripWebData.Enum;
using TripWebData.Inputs;
using Utils.Utils;
using Utils.Utils.RedisUtil;
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

        public async Task<Results<UserDto>> RegisterAsync(RegisterInput input)
        {
            //校验用户名，邮箱，手机号是否已被注册
            if (_context.TabUsers.Count(p=>!p.Deleted && p.Username == input.UserName) > 0)
            {
                return Results<UserDto>.FailResult("用户名已被注册");
            }
            if (_context.TabUsers.Count(p => !p.Deleted && p.Mobile == input.Mobile) > 0)
            {
                return Results<UserDto>.FailResult("手机号已被注册");
            }
            if (_context.TabUsers.Count(p => !p.Deleted && p.Email == input.Email) > 0)
            {
                return Results<UserDto>.FailResult("邮箱已被注册");
            }
            if (input.EmailCode != CacheManager.Get<long>(input.Email + "Code"))
            {
                return Results<UserDto>.FailResult("邮箱验证码错误请重新获取");
            }

            input.Password = DesEncryptUtil.Md5Encrypt(input.Password);

            var tabUser = _mapper.Map<TabUser>(input);
            var userId = _idWorker.NextId();
            tabUser.Id = userId;
            tabUser.RoleId = (int)RoleEnum.User;
            tabUser.UpdatedUserId = userId;
            tabUser.CreatedUserId = userId;

            tabUser.ActiveStatus = true;
            tabUser.ActiveCode = input.EmailCode;

            _context.TabUsers.Add(tabUser);

            await _context.SaveChangesAsync();

            return Results<UserDto>.SuccessResult("注册成功");
        }
    }
}
