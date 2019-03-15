using Core.Token;
using Core.Token.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Controllers
{
    [Route("api/[controller]")]
    public class SystemController: Controller
    {
        #region Token
        /// <summary>
        /// 模拟登录，获取JWT
        /// </summary>
        /// <param name="UserId">用户Id</param>
        /// <param name="Sub">身份</param>
        /// <param name="expiresSliding">相对过期时间，单位：分</param>
        /// <param name="expiresAbsoulte">绝对对过期时间，单位：分</param>
        /// <returns></returns>
        [HttpGet]
        [Route("Token")]
        public JsonResult GetJWTStr(string UserId, string Sub, int expiresSliding, int expiresAbsoulte)
        {
            TokenModel tm = new TokenModel();
            tm.Uid = UserId;
            tm.Sub = Sub;
            return Json(CoreToken.IssueJWT(tm, new TimeSpan(0, expiresSliding, 0), new TimeSpan(0, expiresSliding, 0)));
        }
        #endregion
    }
}
