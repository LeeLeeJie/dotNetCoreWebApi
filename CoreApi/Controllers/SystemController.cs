using Core.Helper;
using Core.IService;
using Core.Model.Base;
using Core.Model.ConfigModel;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
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
        private IDistributedCache _cache;
        private IHttpContextAccessor _httpContextAccessor;
        private IJwtService service = null;


        public SystemController(IDistributedCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
            service = new JwtService(_cache, _httpContextAccessor);
        }
        #region 模拟登录，获取JWT
        /// <summary>
        /// 模拟登录，获取JWT
        /// </summary>
        /// <param name="UserId">用户Id</param>
        /// <param name="Sub">身份</param>
        /// <param name="expiresSliding">相对过期时间，单位：分</param>
        /// <param name="expiresAbsoulte">绝对对过期时间，单位：分</param>
        /// <returns></returns>
        [HttpPost]
        [Route("GetJWTStr")]
        public JsonResult CreateToken(TokenModel tm)
        {
            
            var result = new ResponseModel();
            try
            {
                if (service != null)
                {
                    result.Data = service.CreateJwtToken(tm);
                    result.returnCode = CodeEnum.success;
                    result.returnMsg = "执行成功";
                }
                else
                {
                    result.returnCode = CodeEnum.failed;
                    result.returnMsg = "IJwtService 接口初始化失败！";
                }
                
            }
            catch(Exception ex)
            {
                result.returnCode = CodeEnum.failed;
                result.returnMsg = "执行失败,异常信息："+ ex;
            }
            return Json(result);
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RefresToken")]
        public JsonResult RefresToken(string token)
        {
            IJwtService service = new JwtService(_cache, _httpContextAccessor);
            var result = new ResponseModel();
            try
            {
                if (service != null)
                {
                    service.RefreshTokenAsync(token);
                    result.returnCode = CodeEnum.success;
                    result.returnMsg = "执行成功";
                }
                else
                {
                    result.returnCode = CodeEnum.failed;
                    result.returnMsg = "IJwtService 接口初始化失败！";
                }
            }
            catch (Exception ex)
            {
                result.returnCode = CodeEnum.failed;
                result.returnMsg = "执行失败,异常信息：" + ex;
            }
            return Json(result);
        }

        /// <summary>
        /// 停用Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("StopToken")]
        public JsonResult StopToken(string token)
        {
            IJwtService service = new JwtService(_cache, _httpContextAccessor);
            var result = new ResponseModel();
            try
            {
                if (service != null)
                {
                     service.DeactivateAsync(token);
                    result.returnCode = CodeEnum.success;
                    result.returnMsg = "执行成功";

                }
                else
                {
                    result.returnCode = CodeEnum.failed;
                    result.returnMsg = "IJwtService 接口初始化失败！";
                }
            }
            catch (Exception ex)
            {
                result.returnCode = CodeEnum.failed;
                result.returnMsg = "执行失败,异常信息：" + ex;
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
