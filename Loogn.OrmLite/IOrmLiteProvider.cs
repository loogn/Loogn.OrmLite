using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    /// <summary>
    /// 提供程序类型
    /// </summary>
    public enum OrmLiteProviderType
    {
        /// <summary>
        /// MS SqlServer 数据库
        /// </summary>
        SqlServer,
        /// <summary>
        /// MySql数据库
        /// </summary>
        MySql,

        /// <summary>
        /// sqlite数据库
        /// </summary>
        Sqlite,
    }

    /// <summary>
    /// 提供程序接口
    /// </summary>
    public interface IOrmLiteProvider
    {
        /// <summary>
        /// 创建对应提供程序的参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        DbParameter CreateParameter(string name, object value);
    }

    internal static class EM
    {
        public static OrmLiteProviderType GetProviderType(this DbConnection conn)
        {
            var connTypeName = conn.GetType().Name;

            if (connTypeName == "SqlConnection")
            {
                return OrmLiteProviderType.SqlServer;
            }

            else if (connTypeName == "MySqlConnection")
            {
                return OrmLiteProviderType.MySql;
            }
            else if (connTypeName == "SQLiteConnection")
            {
                return OrmLiteProviderType.Sqlite;
            }

            throw new ArgumentException("OrmLiteProviderType 参数错误");
        }

        public static OrmLiteProviderType GetProviderType(this DbTransaction trans)
        {
            var transTypeName = trans.GetType().Name;

            if (transTypeName == "SqlTransaction")
            {
                return OrmLiteProviderType.SqlServer;
            }

            else if (transTypeName == "MySqlTransaction")
            {
                return OrmLiteProviderType.MySql;
            }
            else if (transTypeName == "SQLiteTransaction")
            {
                return OrmLiteProviderType.Sqlite;
            }
            throw new ArgumentException("OrmLiteProviderType 参数错误");
        }
    }
}
