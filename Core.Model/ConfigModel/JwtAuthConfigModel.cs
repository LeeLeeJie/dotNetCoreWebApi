using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model.ConfigModel
{
   public class JwtAuthConfigModel:BaseConfigModel
    {
        public JwtAuthConfigModel()
        {
            try
            {
                JWTSecretKey = Configuration["JwtAuth:SecurityKey"];
                WebExp = int.Parse(Configuration["JwtAuth:WebExp"]);
                AppExp = int.Parse(Configuration["JwtAuth:AppExp"]);
                MiniProgramExp = int.Parse(Configuration["JwtAuth:MiniProgramExp"]);
                OtherExp = int.Parse(Configuration["JwtAuth:OtherExp"]);
                Issuer = Configuration["JwtAuth:Issuer"];
                Audience = Configuration["JwtAuth:Audience"];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string JWTSecretKey = "This is JWT Secret Key";
        /// <summary>
        /// 
        /// </summary>
        public int WebExp = 12;
        /// <summary>
        /// 
        /// </summary>
        public int AppExp = 12;
        /// <summary>
        /// 
        /// </summary>
        public int MiniProgramExp = 12;
        /// <summary>
        /// 
        /// </summary>
        public int OtherExp = 12;

        /// <summary>
        /// 签发者
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// 接收者
        /// </summary>
        public string Audience { get; set; }
    }
}
