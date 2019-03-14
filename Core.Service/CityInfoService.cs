using Core.Entity;
using Core.IService;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.Service
{
    public class CityInfoService : BaseService<tb_city>, ICityInfo
    {
        List<tb_city> ICityInfo.GetCityListByProID(int proID)
        {
            Expression<Func<tb_city, bool>> ex = (it => 1 == 1 && it.ProID.Equals(proID));
            //List<T> data = sdb.GetPageList(ex, p);
            return sdb.GetList(ex);
        }
    }
}
