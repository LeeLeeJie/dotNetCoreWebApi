using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
    //public class BaseDBConfig
    //{
    //    public static string ConnectionString = ConfigHelper.GetSectionValue("db:BmConnString");
    //}
    public static class SystemConfig
    {
        /// <summary>
        /// 数据库连接串
        /// </summary>
        public static IConfigurationRoot Configuration { get; set; }
    }
    public class BaseDBConfig
    {
        //public static string ConnectionString = SystemConfig.Configuration.GetSection("db")["BmConnString"];
        public static string ConnectionString = ConfigHelper.GetSectionValue("db:BmConnString");
    }
}
