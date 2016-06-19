using Loogn.OrmLite.MySql;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.GeneralParadigm.Service
{
    public class DB
    {
        //public static readonly string ConnectionString = ConfigHelper.ConnString("conn");
        public static readonly string ConnectionString = "server=.;uid=sa;pwd=123456;database=test";

        public static MySqlConnection Open()
        {
            return OrmLite.Open(ConnectionString);
        }

        public static MySqlConnection Open(bool open)
        {
            return OrmLite.Open(ConnectionString, open);
        }
    }
}
