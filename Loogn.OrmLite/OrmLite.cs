using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.SqlClient;

namespace Loogn.OrmLite
{


    public class OrmLite
    {
        private static IOrmLiteProvider SqlServerProvider;
        private static IOrmLiteProvider MySqlProvider;

        public static void RegisterProvider(OrmLiteProviderType type, IOrmLiteProvider provider)
        {
            if (type == OrmLiteProviderType.SqlServer && SqlServerProvider == null)
            {
                SqlServerProvider = provider;
            }
            else if (type == OrmLiteProviderType.MySql && MySqlProvider == null)
            {
                MySqlProvider = provider;
            }
        }


        static OrmLite()
        {
            RegisterProvider(OrmLiteProviderType.SqlServer, SqlServerOrmLiteProvider.Instance);
        }

        public static IOrmLiteProvider GetProvider(OrmLiteProviderType type)
        {
            if (type == OrmLiteProviderType.SqlServer)
            {
                return SqlServerProvider;
            }
            else if (type == OrmLiteProviderType.MySql)
            {
                return MySqlProvider;
            }
            throw new ArgumentException("OrmLiteProviderType 参数错误");
        }

        public const string KeyName = "ID";

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
