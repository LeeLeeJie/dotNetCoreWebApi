using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Model.Base
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public class ResponseModel
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        [DataMember]
        public CodeEnum returnCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [DataMember]
        public string returnMsg { get; set; }

        /// <summary>
        /// 返回的数据
        /// </summary>
        [DataMember]
        public dynamic Data { get; set; }
    }
}
