using Core.Model.ConfigModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Helper
{
   public class SqlSugarHelper
    {
        /// <summary>
        /// 获取SqlSugar
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient GetClient()
        {
            SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = BaseConfigModel.Configuration["db:BmConnString"],
                    DbType = DbType.MySql,
                    IsAutoCloseConnection = true
                }
            );
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" + db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
            return db;
        }
    }
}
