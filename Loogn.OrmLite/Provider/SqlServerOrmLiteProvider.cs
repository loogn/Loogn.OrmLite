using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    /// <summary>
    /// SqlServer提供程序
    /// </summary>
    public class SqlServerOrmLiteProvider : IOrmLiteProvider
    {
        private SqlServerOrmLiteProvider() { }

        /// <summary>
        /// 单件实例
        /// </summary>
        public static SqlServerOrmLiteProvider Instance = new SqlServerOrmLiteProvider();

        /// <summary>
        /// 实现提供程序，创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        public DbParameter CreateParameter(string name, object value)
        {
            var size = OrmLite.GetParameterSize(value);
            var p = new SqlParameter(name, value);
            if (size != null)
            {
                p.Size = size.Value;
            }
            return p;
        }
    }
}
