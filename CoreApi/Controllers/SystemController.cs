using Core.Helper;
using Core.IService;
using Core.Model.Base;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreApi.Controllers
{
    [Route("api/[controller]")]
    public class SystemController: Controller
    {
        #region 模拟登录，获取JWT
        /// <summary>
        /// 模拟登录，获取JWT
        /// </summary>
        /// <param name="UserId">用户Id</param>
        /// <param name="Sub">身份</param>
        /// <param name="expiresSliding">相对过期时间，单位：分</param>
        /// <param name="expiresAbsoulte">绝对对过期时间，单位：分</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetJWTStr")]
        public JsonResult GetJWTStr(Core.Helper.TokenModel tm)
        {
            var result = new ResponseModel();
            try
            {
                result.Data=JwtHelper.IssueJWT(tm);
                result.returnCode = CodeEnum.success;
                result.returnMsg = "执行成功";
            }
            catch(Exception ex)
            {
                result.returnCode = CodeEnum.failed;
                result.returnMsg = "执行失败,异常信息："+ ex;
            }
            return Json(result);
        }
        #endregion

        #region 生成实体类
        /// <summary>
        /// 生成实体类
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("CreateEntity")]
        public JsonResult CreateEntity(string entityName = null)
        {
            IEntity IEntity = new EntityService();
            if (entityName == null) return Json("参数为空");
            string filePath = Directory.GetCurrentDirectory();
            filePath = filePath.Substring(0, filePath.LastIndexOf('\\')) + "\\" + "Core.Entity";
            return Json(IEntity.CreateEntity(entityName, filePath));
        } 
        #endregion
    }
}
