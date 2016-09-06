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
        private static IOrmLiteProvider SqliteProvider;

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
            else if (type == OrmLiteProviderType.Sqlite && SqliteProvider == null)
            {
                SqliteProvider = provider;
            }
        }


        static OrmLite()
        {
            //默认注册sqlserver
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
            else if (type == OrmLiteProviderType.Sqlite)
            {
                return SqliteProvider;
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

    }
}
