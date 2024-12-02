using Microsoft.AspNetCore.Mvc;
using TripWebData.Dtos.TravelBusiness;
using TripWebData;
using TripWebService.TravelBusiness;
using TripWebService.TravelBusiness.Impl;
using TripWebService.User;
using Microsoft.AspNetCore.Authorization;
using TripWebData.Inputs;
using TripWebData.Dtos;
using TripWebService;
using Utils.Utils;

namespace TripWebAPI.Controllers
{
    /// <summary>
    /// 用户管理
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserController : BaseController
    {
        private readonly IUserService _UserService;
        private readonly ITokenService _tokenService;

        public UserController(IUserService userservice, ITokenService tokenservice)
        {
            _UserService = userservice;
            _tokenService = tokenservice;
        }


        /// <summary>
        /// 登录管理
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public Results<UserDto> Login([FromBody]LoginInput input)
        {
            if (string.IsNullOrWhiteSpace(input.LoginAccount) || string.IsNullOrWhiteSpace(input.Pwd))
            {
                return Results<UserDto>.InValidParameter();
            }
            
            var loginResult = _UserService.Login(input);

            if (loginResult.Success)
            {
                loginResult.Token = _tokenService.GenerateToken(loginResult.Data);
            }

            return loginResult;
        }

        /// <summary>
        /// 注册管理
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<Results<UserDto>> Register([FromBody] RegisterInput input)
        {
            //判断是否有空值
            if (string.IsNullOrWhiteSpace(input.UserName) || 
                string.IsNullOrWhiteSpace(input.Password) ||
                    string.IsNullOrWhiteSpace(input.Email) ||
                    string.IsNullOrWhiteSpace(input.ConfirmPwd) ||
                    string.IsNullOrWhiteSpace(input.Mobile) ||
                    string.IsNullOrWhiteSpace(input.EmailCode.ToString()))
            {
                return Results<UserDto>.InValidParameter();
            }

            //校验手机和邮箱格式
            if (!FormatUtil.IsEmail(input.Email)) {
                return Results<UserDto>.FailResult("邮箱格式错误");
            }
            if (!FormatUtil.IsPhone(input.Mobile))
            {
                return Results<UserDto>.FailResult("手机号格式错误");
            }

            //判断两次输入是否一致
            if (!input.ConfirmPwd.Equals(input.Password))
            {
                return Results<UserDto>.FailResult("两次输入不一致");
            }

            var RegisterResult = _UserService.RegisterAsync(input);

            return await RegisterResult;
        }
    }
}
