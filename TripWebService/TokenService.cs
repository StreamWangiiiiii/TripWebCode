using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TripWebData.Dtos;
using TripWebData.Options;

namespace TripWebService
{
    /// <summary>
    /// Jwt token 服务
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly JwtTokenOption _jwtTokenOption;
        /// <summary>
        /// 注入服务
        /// </summary>
        /// <param name="jwtTokenOption"></param>
        public TokenService(IOptionsMonitor<JwtTokenOption> jwtTokenOption)
        {
            _jwtTokenOption = jwtTokenOption.CurrentValue;
        }
        /// <summary>
        /// 生成Token
        /// </summary>
        /// <param name="dto">当前登录人的用户</param>
        /// <returns></returns>
        public string GenerateToken(UserDto dto)
        {
            // 保存用户个人信息（不要放用户非常私密的信息）
            var claims = new[]
            {
                new Claim("UserName",dto.Username),
                new Claim("RoleName",dto.RoleName),
                new Claim("RoleId",dto.RoleId.ToString()),
                new Claim("UserId",dto.Id.ToString())
            };

            var rsa = RSA.Create();
            rsa.ImportRSAPrivateKey(Convert.FromBase64String(_jwtTokenOption.SecurityKey), out _);
            var credential = new SigningCredentials(new RsaSecurityKey(rsa),SecurityAlgorithms.RsaSha256);

            // payload 中的信息声明
            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtTokenOption.Issuer,
                audience: _jwtTokenOption.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtTokenOption.TokenExpireTime),
                signingCredentials: credential
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return token;
        }
    }
}
