using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite2
{
    /// <summary>
    /// 分页查询参数类
    /// </summary>
    public class OrmLitePageFactor
    {
        /// <summary>
        /// 条件，可带参数
        /// </summary>
        public string Conditions { get; set; }
        /// <summary>
        /// 表名，可选
        /// </summary>
        public string TableName { get; set; }


        /// <summary>
        /// 查询字段，可选，默认为*
        /// </summary>
        public string Fields { get; set; }

        /// <summary>
        /// 排序语句，必须
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// 参数集，匿名对象 或 Dictionary[string, object]
        /// </summary>
        public object Params { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页大小
        /// </summary>
        public int PageSize { get; set; }
    }
}
