using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripWebData.Dtos;
using TripWebData.Inputs;
using TripWebData;
using TripWebService;
using Utils.Utils;
using DotNetCore.CAP.Dashboard;
using Utils.Utils.RedisUtil;
using Microsoft.EntityFrameworkCore;
using TripWebData.Dtos.TravelBusiness;

namespace TripWebAPI.Controllers
{
    /// <summary>
    /// 邮件管理
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class SendEmailController : BaseController
    {
        private readonly ISendEmailService _sendEmailService;

        public SendEmailController(ISendEmailService sendEmailService)
        {
            _sendEmailService = sendEmailService;
        }

        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<Results<string>> SendEmailCode(string Email)
        {
            //校验邮箱是否为空
            if (string.IsNullOrWhiteSpace(Email))
            {
                return Results<string>.FailResult("邮箱不可为空");
            }
            //校验邮箱格式
            if (!FormatUtil.IsEmail(Email))
            {
                return Results<string>.FailResult("邮箱格式错误");
            }
            //校验邮箱请求频率限制
            if (CacheManager.Exist(Email + "Limited"))
            {
                return Results<string>.FailResult("Too many requests. Please try again later.");
            }

            var SendResult = await _sendEmailService.SendCertifictionCodeAsync(Email);
            
            return SendResult;
        }
    }
}
