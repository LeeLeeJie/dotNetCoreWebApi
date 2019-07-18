using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.IService;
using Core.Model.Base;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;

namespace CoreApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : Controller
    {
        private IAreaService IService = new AreaService();
        private readonly Logger Logger;

        public AreaController()
        {
            Logger = LogManager.GetLogger("FileLogger");
        }


        /// <summary>
        /// 获取分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        // GET: api/<controller>
        [HttpGet]
        [Route("GetPageList")]
        /*[Authorize(Policy = "Client")]
        [EnableCors("AllowAnyOrigin")]*/
        public JsonResult GetPageList(int pageIndex = 1, int pageSize = 10)
        {
            var result = new ResponseModel();
            try
            {
                Logger.Info("调用接口开始~~~~~~~~~~");
                result.returnCode = CodeEnum.success;
                result.Data = IService.GetPageList(pageIndex, pageSize);
                result.returnMsg = "执行成功";
                Logger.Info("调用接口结束，返回参数：" + JsonConvert.SerializeObject(result));
            }
            catch (Exception ex)
            {
                result.returnCode = CodeEnum.failed;
                result.returnMsg = ex.Message;
                Logger.Error(ex);
            }

            return Json(result);
        }

    }
}