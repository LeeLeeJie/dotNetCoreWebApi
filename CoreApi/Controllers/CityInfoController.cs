using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.IService;
using Core.Model.Base;
using Core.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CoreApi.Controllers
{
    [Route("api/[controller]")]
    public class CityInfoController : Controller
    {
        private ICityInfo IService = new CityInfoService();
        static Logger Logger = LogManager.GetCurrentClassLogger();
        // GET: api/<controller>
        [HttpGet]
        public JsonResult Get(int pageIndex = 1, int pageSize = 10)
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

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public JsonResult GetCityListByProID(int id)
        {
            var result = new ResponseModel();
            try
            {
                Logger.Info("调用接口开始~~~~~~~~~~");
                result.returnCode = CodeEnum.success;
                result.Data = IService.GetCityListByProID(id).Select(o => o.CityName);
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

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
