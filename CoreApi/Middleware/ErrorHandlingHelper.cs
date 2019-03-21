using Core.Model.Base;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CoreApi.Middleware
{
   public class ErrorHandlingHelper
    {

        private readonly RequestDelegate next;
        /// <summary>
        /// 请求异常处理方法
        /// </summary>
        /// <param name="next"></param>
        public ErrorHandlingHelper(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            bool isCatched = false;
            try
            {
                await next(context);
            }
            catch (Exception ex) //发生异常
            {
                //自定义业务异常
                if (ex is MyException)
                {
                    context.Response.StatusCode = ((MyException)ex).GetCode();
                }
                //未知异常
                else
                {
                    context.Response.StatusCode = 500;
                    //LogHelper.SetLog(LogLevel.Error, ex);
                }
                await HandleExceptionAsync(context, context.Response.StatusCode, ex.Message);
                isCatched = true;
            }
            finally
            {
                if (!isCatched && context.Response.StatusCode != 200)//未捕捉过并且状态码不为200
                {
                    string msg = "";
                    switch (context.Response.StatusCode)
                    {
                        case 401:
                            msg = "未授权";
                            break;
                        case 404:
                            msg = "未找到服务";
                            break;
                        case 502:
                            msg = "请求错误";
                            break;
                        default:
                            msg = "未知错误";
                            break;
                    }
                    await HandleExceptionAsync(context, context.Response.StatusCode, msg);
                }
            }
        }
        //异常错误信息捕获，将错误信息用Json方式返回
        private static Task HandleExceptionAsync(HttpContext context, int statusCode, string msg)
        {
            var result = JsonConvert.SerializeObject(new ResponseModel() { Success = false, returnMsg = msg, returnCode =CodeEnum.failed });
            context.Response.ContentType = "application/json;charset=utf-8";
            return context.Response.WriteAsync(result);
        }
    }
    public static class ErrorHandlingExtensions
    {
        public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandlingHelper>();
        }
    }
}
