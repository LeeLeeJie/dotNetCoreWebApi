using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entity
{
    [SugarTable("tb_city")]
    public partial class tb_city
    {
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public int CityID { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string CityName { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? ProID { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? CitySort { get; set; }

        /// <summary>
        /// Desc:公平价城市映射城市
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string car_short_name { get; set; }
    }
}
