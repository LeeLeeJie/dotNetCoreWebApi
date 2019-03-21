using Core.Helper;
using Core.IService;
using Core.Model.Base;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreApi.AuthHelper
{
    /// <summary>
    /// Token验证授权中间件    
    /// </summary>
    public class JwtAuthorizationFilter
    {
        /// <summary>
        /// http委托
        /// </summary>
        private readonly RequestDelegate _next;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="next"></param>
        public JwtAuthorizationFilter(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// 验证授权
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext httpContext)
        {
            try
            {
                //检测是否包含'Authorization'请求头，如果不包含则直接放行
                if (!httpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    return _next(httpContext);
                }
                var tokenHeader = httpContext.Request.Headers["Authorization"];
                tokenHeader = tokenHeader.ToString().Substring("Bearer ".Length).Trim();
                //redis jwt 验证
                RedisCacheHelper redisCacheHelper = new RedisCacheHelper();
                if (!redisCacheHelper.Exist(tokenHeader))
                {
                    throw new MyException("token不存在！");
                }

                TokenModel tm = new TokenModel();
                if (JwtHelper.ValidateRuleBase(tokenHeader, out tm))
                {
                    //授权
                    var claimList = new List<Claim>();
                    var claim = new Claim(ClaimTypes.Role, tm.Role);
                    claimList.Add(claim);
                    var identity = new ClaimsIdentity(claimList);
                    var principal = new ClaimsPrincipal(identity);
                    httpContext.User = principal;
                    return _next(httpContext);
                }
                else
                {
                    throw new MyException("验证不通过");
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
