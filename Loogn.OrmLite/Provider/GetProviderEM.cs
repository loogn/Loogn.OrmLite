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
            var connTypeName = string.Intern(conn.GetType().Name);

            if (ReferenceEquals(connTypeName, "SqlConnection"))
            {
                return OrmLiteProviderType.SqlServer;
            }

            else if (ReferenceEquals(connTypeName, "MySqlConnection"))
            {
                return OrmLiteProviderType.MySql;
            }
            else if (ReferenceEquals(connTypeName, "SQLiteConnection"))
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
