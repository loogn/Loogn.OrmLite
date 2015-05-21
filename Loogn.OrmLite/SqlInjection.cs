using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    /// <summary>
    /// sql注入
    /// </summary>
    public class SqlInjection
    {
        /// <summary>
        /// 过滤单引号注入
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Filter(string value)
        {
            return value == null ? string.Empty : value.Replace("'", "''");
        }
    }
}
