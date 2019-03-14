using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Model
{
   public class PageInfo<T>
    {
        public int PageIndex { get; set; }
        
        public int PageSize { get; set; }

        public int Count { get; set; }

        public List<T> Data { get; set; }
    }
}
