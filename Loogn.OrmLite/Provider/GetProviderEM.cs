using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Loogn.OrmLite
{
    internal static class GetProviderEM
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
            return trans.Connection.GetProviderType();
        }
    }
}
