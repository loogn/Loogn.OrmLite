using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite2
{
    public static class OrmLite
    {
        public static IDbConnection Open(string connectionString, ICommandDialectProvider provider)
        {
            var conn = new OrmLiteConnection(provider);
            conn.ConnectionString = connectionString;
            return conn;
        }
        public static IDbConnection Open(string connectionString)
        {
            return Open(connectionString, OrmLiteConfig.DefaultCommandDialectProvider);
        }
    }
}
