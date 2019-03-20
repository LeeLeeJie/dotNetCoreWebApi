using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.ConfigModel
{
   public class JwtAuthorizationDto
    {
        /// <summary>
        /// 用户唯一标识
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// jwt Token 
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 生效时间
        /// </summary>
        public long Auths { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        public long Expires { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Success { get; set; }
    }
}
