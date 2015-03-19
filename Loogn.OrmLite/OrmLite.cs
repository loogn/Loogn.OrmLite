using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    public class OrmLite
    {
        public static SqlConnection Open(string connectionString, bool open=false)
        {
            var conn = new SqlConnection(connectionString);
            if (open) conn.Open();
            return conn;
        }
        public static SqlConnection Open(bool open = false)
        {
            var conn = new SqlConnection(defaultConnectionString);
            if (open) conn.Open();
            return conn;
        }


        private static string defaultConnectionString;
        public static void SetDefaultConnectionString(string connectionString)
        {
            defaultConnectionString = connectionString;
        }
        public static readonly string KeyFieldName = "ID";

        public static int SqlStringBuilderCapacity = 100;
        public static void SetSqlStringBuilderCapacity(int capacity,bool enforce=false)
        {
            if(enforce)
            {
                SqlStringBuilderCapacity = capacity;
            }
            else
            {
                if(capacity>SqlStringBuilderCapacity)
                {
                    SqlStringBuilderCapacity = capacity;
                }
            }
        }

        public static void SetSqlStringBuilderCapacity(string sql, bool enforce = false)
        {
            var capacity = sql.Length;
            //Console.WriteLine("Sql Capacity:" + SqlStringBuilderCapacity);
            //Console.WriteLine("Sql Length:" + capacity);
            //Console.WriteLine("Sql Statement:" + sql);
            SetSqlStringBuilderCapacity(capacity, enforce);
        }
    }
}
