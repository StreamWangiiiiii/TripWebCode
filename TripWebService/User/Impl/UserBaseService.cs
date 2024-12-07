using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TripWebData;
using TripWebData.Dtos;
using TripWebData.Entity;
using TripWebData.Enums;
using TripWebData.Inputs;
using TripWebUtils.Utils;
using TripWebUtils.Utils.RedisUtil;

namespace TripWebService.User.Impl
{
    /// <summary>
    /// 用户相关接口服务实现
    /// </summary>
    public class UserBaseService : BaseService,IUserService
    {
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

            var user = TripWebContext.TabUsers.AsNoTracking().Include(p=>p.Role).FirstOrDefault(exp);

            if (user == null)
            {
                return Results<UserDto>.FailResult("账号或密码输入错误");
            }

            if (!user.ActiveStatus)
            {
                return Results<UserDto>.FailResult("账号未激活");
            }

            return Results<UserDto >.DataResult(ObjectMapper.Map<UserDto>(user));
        }

        public async Task<Results<UserDto>> RegisterAsync(RegisterInput input)
        {
            //校验用户名，邮箱，手机号是否已被注册
            if (TripWebContext.TabUsers.Count(p=>!p.Deleted && p.Username == input.UserName) > 0)
            {
                return Results<UserDto>.FailResult("用户名已被注册");
            }
            if (TripWebContext.TabUsers.Count(p => !p.Deleted && p.Mobile == input.Mobile) > 0)
            {
                return Results<UserDto>.FailResult("手机号已被注册");
            }
            if (TripWebContext.TabUsers.Count(p => !p.Deleted && p.Email == input.Email) > 0)
            {
                return Results<UserDto>.FailResult("邮箱已被注册");
            }
            if (input.EmailCode != CacheManager.Get<long>(input.Email + "Code"))
            {
                return Results<UserDto>.FailResult("邮箱验证码错误请重新获取");
            }

            input.Password = DesEncryptUtil.Md5Encrypt(input.Password);

            var tabUser = ObjectMapper.Map<TabUser>(input);
            var userId = SnowIdWorker.NextId();
            tabUser.Id = userId;
            tabUser.RoleId = (int)RoleEnum.User;
            tabUser.UpdatedUserId = userId;
            tabUser.CreatedUserId = userId;

            tabUser.ActiveStatus = true;
            tabUser.ActiveCode = input.EmailCode;

            TripWebContext.TabUsers.Add(tabUser);

            await TripWebContext.SaveChangesAsync();

            return Results<UserDto>.SuccessResult("注册成功");
        }

        /*public async Task<Results<int>> UpdateUserProfileAsync(UserProfileInput input)
        {
            var entity = await _context.TabUsers.FirstOrDefaultAsync(p => p.Username == input.Username && !p.Deleted);
            
            if (entity == null) 
            {
                return Results<int>.FailResult("未找到记录");
            }
        }
        public Results<T> CheckUserInput<T>(T input)
        {
            if (input == null)
            {
                return Results<T>.InValidParameter();
            }

            // 反射获取对象的所有属性并进行格式校验
            var properties = input.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(input);

                //非空校验
                if (value is string str && string.IsNullOrWhiteSpace(str) || value == null)
                {
                    return Results<T>.InValidParameter(property + "为空");
                }

                //邮箱格式校验
            }

            return Results<T>.SuccessResult();
        }*/
    }
}
