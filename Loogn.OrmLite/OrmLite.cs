using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    public class OrmLite
    {
        public const string KeyName = "ID";

        public static SqlConnection Open(string connectionString, bool open = false)
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
        public static string DefaultConnectionString
        {
            get { return defaultConnectionString; }
            set { defaultConnectionString = value; }
        }



        private static string defaultKeyName = KeyName;
        public static string DefaultKeyName
        {
            get { return defaultKeyName; }
            set { defaultKeyName = value; }
        }

        private static List<string> updateIgnoreFields = new List<string>() { "AddDate", "AddTime" };
        public static List<string> UpdateIgnoreFields
        {
            get { return updateIgnoreFields; }
        }

        public static bool WriteSqlLog
        {
            get;
            set;
        }


        public static int SqlStringBuilderCapacity = 100;
        public static void SetSqlStringBuilderCapacity(int capacity, bool enforce = false)
        {
            if (enforce)
            {
                SqlStringBuilderCapacity = capacity;
            }
            else
            {
                if (capacity > SqlStringBuilderCapacity)
                {
                    SqlStringBuilderCapacity = capacity;
                }
            }
        }

        public static void SetSqlStringBuilderCapacity(string sql, bool enforce = false)
        {
            var capacity = sql.Length;
            if (WriteSqlLog)
            {
                File.AppendAllText("ormlite.sqllog.txt",
                    string.Format("Sql Leng:{0}\tSql Capacity:{1}{2}{3}{2}{4}{2}",
                    capacity,
                    SqlStringBuilderCapacity,
                    Environment.NewLine,
                    sql,
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            }
            SetSqlStringBuilderCapacity(capacity, enforce);
        }
    }
}
