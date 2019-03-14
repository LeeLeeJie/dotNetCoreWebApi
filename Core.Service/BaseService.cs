using Core.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Core.Service
{
    
    /// <summary>
    /// 服务层基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseService<T> where T : class, new()
    {
        public SqlSugarClient db;
        public SimpleClient sdb;
        public BaseService()
        {
            db = GetClient();
            sdb = db.GetSimpleClient();
        }
        /// <summary>
        /// 获取客户端
        /// </summary>
        /// <returns></returns>
        private SqlSugarClient GetClient()
        {
            SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig()
                {
                    ConnectionString = BaseDBConfig.ConnectionString,
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

        #region CRUD
        public PageInfo<T> GetPageList(int pageIndex, int pageSize)
        {
            PageModel p = new PageModel() { PageIndex = pageIndex, PageSize = pageSize };
            Expression<Func<T, bool>> ex = (it => 1 == 1);
            List<T> data = sdb.GetPageList(ex, p);
            var t = new PageInfo<T>
            {
                PageIndex = pageIndex,
                Count = p.PageCount,
                Data = data,
                PageSize = pageSize
            };
            return t;
        }

        public T SelectOne(long id)
        {
            return sdb.GetById<T>(id);
        }

        public bool Add(T entity)
        {
            return sdb.Insert(entity);
        }

        public bool Update(T entity)
        {
            return sdb.Update(entity);
        }

        public bool Dels(dynamic[] ids)
        {
            return sdb.DeleteByIds<T>(ids);
        }
        #endregion
    }
}
