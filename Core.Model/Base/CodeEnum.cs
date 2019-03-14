using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Core.Model.Base
{
    [DataContract]
    public enum CodeEnum
    {
        /// <summary>
        /// 失败
        /// </summary>
        [EnumMember]
        failed = 0,

        /// <summary>
        /// 成功
        /// </summary>
        [EnumMember]
        success = 1,

        /// <summary>
        /// 超时
        /// </summary>
        [EnumMember]
        timeOut,

        /// <summary>
        /// 异常
        /// </summary>
        [EnumMember]
        exception
    }
}
