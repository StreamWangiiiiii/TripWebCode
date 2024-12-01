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
                loginResult.Token = _tokenService.GenerateToken(loginResult.Data    );
            }

            return loginResult;
        }
    }
}
