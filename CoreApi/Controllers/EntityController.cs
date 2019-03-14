
using Core.Entity;
using Core.IService;
using Core.Service;
using CoreApi;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CoreWebApi.Controllers
{
    /// <summary>
    /// 实体操作模块
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class EntityController : Controller
    {
        private IEntity IEntity = new EntityService();
        private readonly IHostingEnvironment _hostingEnvironment;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        public EntityController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        /// <summary>
        /// 生成实体类
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult CreateEntity(string entityName = null)
        {
            if (entityName == null) return Json("参数为空");
            string filePath = Directory.GetCurrentDirectory();
            filePath=filePath.Substring(0, filePath.LastIndexOf('\\'))+"\\"+ "Core.Entity";

            return Json(IEntity.CreateEntity(entityName, filePath));
        }
    }
}
