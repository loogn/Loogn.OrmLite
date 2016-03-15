using Loogn.OrmLite;
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

        public static SqlConnection Open()
        {
            return OrmLite.Open(ConnectionString);
        }

        public static SqlConnection Open(bool open)
        {
            return OrmLite.Open(ConnectionString, open);
        }
    }
}
