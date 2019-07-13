using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.IService;
using Core.Model.Base;
using Core.RabbitMQ;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
        private  Logger Logger;

        public CityInfoController()
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
        //[Authorize(Policy = "Client")]
        //[EnableCors("AllowAnyOrigin")]
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

        /// <summary>
        /// 根据省份ID获取下属城市
        /// </summary>
        /// <param name="id">省份ID</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCityListByProID")]
        [Authorize(Policy = "AdminOrClient")]
        public JsonResult GetCityListByProID(int ProID)
        {
            var result = new ResponseModel();
            try
            {
                Logger.Info("调用接口开始~~~~~~~~~~");
                result.returnCode = CodeEnum.success;
                result.Data = IService.GetCityListByProID(ProID).Select(o => o.CityName);
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


        /// 将日志写入数据库
        /// 
        /// <returns></returns>
        [HttpGet]
        [Route("WriteLogToDb")]
        public JsonResult WriteLogToDb()
        {
            var result = new ResponseModel();
            Logger _dblogger = LogManager.GetLogger("DbLogger");
            LogEventInfo ei = new LogEventInfo();
            ei.Properties["Desc"] = "我是自定义消息";
            _dblogger.Info(ei);
            _dblogger.Debug(ei);
            _dblogger.Trace(ei);
            return Json(result);
        }

        /// <summary>
        /// MQ测试
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        // GET: api/<controller>
        [HttpGet]
        [Route("MQSendMsg")]
        public JsonResult MQSendMsg(string  queueName= "TestQueue111", string message="hello MQ")
        {
            var result = new ResponseModel();
            try
            {
                result.returnCode = CodeEnum.success;
                MQService service = new MQService(queueName, message);
                service.ExeSend();
                Thread.Sleep(1000);
                service.ExeReceive();
                result.returnMsg = "执行成功";
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
