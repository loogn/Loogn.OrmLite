using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    public class SqlServerOrmLiteProvider : IOrmLiteProvider
    {
        private SqlServerOrmLiteProvider() { }

        public static SqlServerOrmLiteProvider Instance = new SqlServerOrmLiteProvider();

        public DbParameter CreateParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }
    }
}
