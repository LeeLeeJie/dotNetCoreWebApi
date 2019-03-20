using Core.Helper;
using Core.IService;
using Core.Model.ConfigModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Service
{
    public class JwtService : IJwtService
    {
        #region Initialize

        /// <summary>
        /// 已授权的 Token 信息集合
        /// </summary>
        private static ISet<JwtAuthorizationDto> _tokens = new HashSet<JwtAuthorizationDto>();
        /// <summary>
        /// 配置信息
        /// </summary>
        private readonly JwtAuthConfigModel jwtConfig = new JwtAuthConfigModel();

        /// <summary>
        /// 获取 HTTP 请求上下文
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected RedisCacheHelper redisCacheHelper;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="configuration"></param>
        public JwtService(IDistributedCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            redisCacheHelper = new RedisCacheHelper(cache);
        }

        #endregion
        public JwtAuthorizationDto CreateJwtToken(TokenModel tokenModel)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            //秘钥
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.JWTSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            DateTime authTime = DateTime.UtcNow;
            DateTime expiresAt = authTime;

            #region 计算jwt有效时间
            //过期时间
            int exp = 0;
            switch (tokenModel.TokenType.ToLower())
            {
                case "web":
                    exp = jwtConfig.WebExp;
                    break;
                case "app":
                    exp = jwtConfig.AppExp;
                    break;
                case "miniprogram":
                    exp = jwtConfig.MiniProgramExp;
                    break;
                case "other":
                    exp = jwtConfig.OtherExp;
                    break;
            }
            switch (tokenModel.EffectiveTimeType)
            {
                case "year":
                    expiresAt = expiresAt.AddYears(exp);
                    break;
                case "month":
                    expiresAt = expiresAt.AddMonths(exp);
                    break;
                case "day":
                    expiresAt = expiresAt.AddDays(exp);
                    break;
                case "hours":
                    expiresAt = expiresAt.AddHours(exp);
                    break;
                case "min":
                    expiresAt = expiresAt.AddMinutes(exp);
                    break;
                case "sec":
                    expiresAt = expiresAt.AddSeconds(exp);
                    break;
            }
            #endregion
            //将用户信息添加到 Claim 中
            var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);

            IEnumerable<Claim> claims = new Claim[] {
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),//用户Id
                new Claim(ClaimTypes.Role, tokenModel.Role),//身份
                new Claim("Project", tokenModel.Project),//项目名称
                new Claim(JwtRegisteredClaimNames.Iat,authTime.ToString(),ClaimValueTypes.Integer64),
                new Claim(ClaimTypes.Expiration,expiresAt.ToString())//过期时间
            };
            identity.AddClaims(claims);

            //签发一个加密后的用户信息凭证，用来标识用户的身份
            _httpContextAccessor.HttpContext.SignInAsync(JwtBearerDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            //生成jwt token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),//创建声明信息
                Issuer = jwtConfig.Issuer,//Jwt token 的签发者
                Audience = jwtConfig.Audience,//Jwt token 的接收者
                Expires = expiresAt,//过期时间
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)//创建 token
            };

            var tokenInfo = tokenHandler.CreateToken(tokenDescriptor);
            string tokenStr = tokenHandler.WriteToken(tokenInfo);

            //存储 Token 信息
            var jwt = new JwtAuthorizationDto
            {
                UserId = Guid.NewGuid().ToString(),
                Token = tokenStr,
                Auths = new DateTimeOffset(authTime).ToUnixTimeSeconds(),
                Expires = new DateTimeOffset(expiresAt).ToUnixTimeSeconds(),
                Success = true
            };
            //写入Redis
            redisCacheHelper.Set(tokenStr, tokenModel, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Convert.ToDouble(jwtConfig.WebExp))
            });

            return jwt;
        }


        /// <summary>
        /// 获取 HTTP 请求的 Token 值
        /// </summary>
        /// <returns></returns>
        private string GetCurrentAsync()
        {
            //http header
            var authorizationHeader = _httpContextAccessor
                .HttpContext.Request.Headers["authorization"];

            //token
            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();// bearer tokenvalue
        }
        /// <summary>
        /// 停用当前 Token
        /// </summary>
        /// <returns></returns>
        public async Task DeactivateCurrentAsync()
        => await DeactivateAsync(GetCurrentAsync());
        /// <summary>
        /// 停用 Token
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns></returns>
        public async Task DeactivateAsync(string token)
        {
            redisCacheHelper.Remove(token);
        }
        /// <summary>
        /// 判断 Token 是否有效
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns></returns>
        public async Task<bool> IsActiveAsync(string token)
        {
            if (redisCacheHelper.Exist(token))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //=> await _cache.GetStringAsync(GetKey(token)) == null;

        /// <summary>
        /// 判断当前 Token 是否有效
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsCurrentActiveTokenAsync()
        => await IsActiveAsync(GetCurrentAsync());

        /// <summary>
        /// 刷新 Token
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="dto">用户信息</param>
        /// <returns></returns>
        public async Task RefreshTokenAsync(string token)
        {
             await Task.Run(() =>
            {
                if (redisCacheHelper.Exist(token))
                {
                    bool isExisted = false;
                    TokenModel tokenModel = redisCacheHelper.Get<TokenModel>(token, out isExisted);
                    //先重新生成token
                    redisCacheHelper.Set(token,tokenModel, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(Convert.ToDouble(jwtConfig.WebExp))
                    });
                }
                else
                {
                    throw new Exception("未获取到当前 Token 信息");
                }
            }).ConfigureAwait(false);
        }
    }


}
