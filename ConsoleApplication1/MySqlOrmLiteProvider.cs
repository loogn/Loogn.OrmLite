using Loogn.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace ConsoleApplication1
{
    public class MySqlOrmLiteProvider : IOrmLiteProvider
    {
        private MySqlOrmLiteProvider() { }

        public static MySqlOrmLiteProvider Instance = new MySqlOrmLiteProvider();

        public DbConnection CreateConnection()
        {
            return new MySqlConnection();
        }

        public DbParameter CreateParameter(string name, object value)
        {
            return new MySqlParameter(name, value);
        }
    }
}
