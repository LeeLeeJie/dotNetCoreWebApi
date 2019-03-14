using Core.Entity;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.IService
{
   public interface ICityInfo:IServiceBase<tb_city>
    {
        List<tb_city> GetCityListByProID(int proID);
    }
}
