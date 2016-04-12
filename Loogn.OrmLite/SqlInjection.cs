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

        /// <summary>
        /// 对于like操作，需要进行替换
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ReplaceForLike(string value)
        {
            /*
            对于like操作，需要进行以下替换（注意顺序也很重要）
            [ -> [[]     (这个必须是第一个替换的!!)
            % -> [%]    (这里%是指希望匹配的字符本身包括的%而不是专门用于匹配的通配符)
            _ -> [_]
            ^ -> [^]
            */
            return value == null ? string.Empty : value.Replace("[", "[[]")
                .Replace("%", "[%]")
                .Replace("_", "[_]")
                .Replace("^", "[^]");
        }
    }
}
