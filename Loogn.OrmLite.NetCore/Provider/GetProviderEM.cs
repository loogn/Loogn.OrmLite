using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Loogn.OrmLite.NetCore
{
    internal static class GetProviderEM
    {
        public static OrmLiteProviderType GetProviderType(this DbConnection conn)
        {
            var connTypeName = conn.GetType().Name;

            if ("SqlConnection".Equals(connTypeName))
            {
                return OrmLiteProviderType.SqlServer;
            }

            else if ("MySqlConnection".Equals(connTypeName))
            {
                return OrmLiteProviderType.MySql;
            }
            else if ("SQLiteConnection".Equals(connTypeName))
            {
                return OrmLiteProviderType.Sqlite;
            }

            throw new ArgumentException("OrmLiteProviderType 参数错误");
        }

        public static OrmLiteProviderType GetProviderType(this DbTransaction trans)
        {
            return trans.Connection.GetProviderType();
        }
    }
}
