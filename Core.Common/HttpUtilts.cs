using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Core.Common
{
    public class HttpUtilts
    {
        #region http请求
        /// <summary>
        /// 默认UserAgent
        /// </summary>
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        /// <summary>
        /// 默认Timeout:10000毫秒(10秒)
        /// </summary>
        private static readonly int DefaultTimeout = 10000;
        /// <summary>
        /// 默认contentType
        /// </summary>
        private static readonly string DefaultcontentType = "application/x-www-form-urlencoded";

        /// <summary>
        /// http请求
        /// </summary>
        /// <param name="url">请求地址(必须)</param>
        /// <param name="content">请求内容</param>
        /// <param name="method">请求方式(必须)</param>
        /// <param name="headers">需要添加的标头信息</param>
        /// <param name="encoding">字符编码(默认:UTF8)</param>
        /// <param name="contentType">字符格式(默认:application/x-www-form-urlencoded)</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码(与用户名同时存在)</param>
        /// <param name="timeout">超时时间默认(10秒)</param>
        /// <returns>响应返回值</returns>
        public static string CreateHttpResponse(string url, string content, string method, Dictionary<string, string> headers, Encoding encoding, string contentType, string userName, string password, ref bool IsAbnormal, int timeout = 0)
        {
            //记录日志
            encoding = encoding == null ? Encoding.UTF8 : encoding;
            if (string.IsNullOrEmpty(url)) { return "URL错误!"; }
            if (string.IsNullOrEmpty(method)) { return "method错误!"; }

            string responseData = String.Empty;
            try
            {
                //实例化请求
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

                //请求类型
                request.Method = method;

                request.KeepAlive = false;

                request.AllowAutoRedirect = true;

                //标头
                if (headers != null)
                {
                    for (var i = 0; i < headers.Count; i++)
                    {
                        var item = headers.ElementAt(i);
                        request.Headers.Add(item.Key, item.Value);
                    }
                }
                //超时时间()
                request.Timeout = timeout == 0 ? DefaultTimeout : timeout;

                //类型
                request.ContentType = string.IsNullOrEmpty(contentType) ? DefaultcontentType : contentType;

                //请求参数
                if (!string.IsNullOrEmpty(content))
                {
                    byte[] Data = encoding.GetBytes(content);
                    using (Stream reqStream = request.GetRequestStream())
                    {
                        reqStream.Write(Data, 0, Data.Length);
                        reqStream.Close();
                    }
                    //请求参数长度
                    if (request.ContentLength != Data.Length)
                    {
                        request.ContentLength = Data.Length;
                    }   
                }
                else
                {
                    request.ContentLength = 0;
                }

                //向路由提交基本身份验证
                if (!string.IsNullOrEmpty(userName) || !string.IsNullOrEmpty(password))
                {
                    string code = Convert.ToBase64String(encoding.GetBytes(string.Format("{0}:{1}", userName, password)));
                    request.Headers.Add("Authorization", "Basic " + code);
                }

                //发起请求,返回响应内容
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), encoding))
                    {
                        IsAbnormal = true;
                        responseData = reader.ReadToEnd().ToString();
                    }
                }
            }
            catch (WebException ex)
            {
                IsAbnormal = false;
                //记录日志
                //CustomMethod.LogErrorLog("Url:" + url, "CustomData_CreateHttpResponse");
                //CustomMethod.LogErrorLog(ex.ToString(), "CustomData_CreateHttpResponse");
                //返回的响应
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                //如果是错误400或者404则返回错误信息
                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        using (Stream _Stream = response.GetResponseStream())
                        {
                            using (StreamReader _Reader = new StreamReader(_Stream))
                            {
                                string _JsonData = _Reader.ReadToEnd();

                                //记录日志
                                //CustomMethod.LogErrorLog(_JsonData, "CustomData_CreateHttpResponse");

                                ExceptionModel Em = JsonConvert.DeserializeObject<ExceptionModel>(_JsonData);

                                return Em.error_description;
                            }
                        }
                    }
                }
                return "服务器异常,请联系管理员!";

            }
            return responseData;
        }
        public class ExceptionModel
        {
            public string error { get; set; }
            public string error_description { get; set; }
        }
        #endregion
    }
}
