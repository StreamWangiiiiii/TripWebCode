using DotNetCore.CAP.Dashboard;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using TripWebData.Dtos.TravelBusiness;
using TripWebData;
using TripWebData.Entity;
using TripWebUtils.Utils;
using TripWebUtils.Utils.RedisUtil;
using TripWebUtils.Utils.Snowflake;
using TripWebData.Dtos;

namespace TripWebService
{
    public class SendEmailCodeService : ISendEmailService
    {
        private readonly IdWorker _idWorker;

        public SendEmailCodeService()
        {
            _idWorker = SnowflakeUtil.CreateIdWorker();
        }

        public async Task<Results<string>> SendCertifictionCodeAsync(string Email)
        {
            var Code = new Random().Next(10000,100000);

            CacheManager.Set(Email+"Code",Code, TimeSpan.FromMinutes(10));
            CacheManager.Set(Email+"Limited","1", TimeSpan.FromMinutes(1));// 设置频率限制

            EmailUtil.NetSendEmail($"欢迎注册，您的验证码是：{Code}", "注册账号验证码", Email);

            return Results<string>.SuccessResult(Code.ToString());
        }
    }
}
