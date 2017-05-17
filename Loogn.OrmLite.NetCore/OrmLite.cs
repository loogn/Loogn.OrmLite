using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.SqlClient;

namespace Loogn.OrmLite.NetCore
{
    /// <summary>
    /// OrmLite配置类
    /// </summary>
    public class OrmLite
    {
        private static IOrmLiteProvider SqlServerProvider;
        private static IOrmLiteProvider MySqlProvider;
        private static IOrmLiteProvider SqliteProvider;

        /// <summary>
        /// 注册提供程序，SqlServer默认已注册
        /// </summary>
        /// <param name="type">OrmLiteProviderType枚举</param>
        /// <param name="provider">实现IOrmLiteProvider接口的提供程序类</param>
        public static void RegisterProvider(OrmLiteProviderType type, IOrmLiteProvider provider)
        {
            if (type == OrmLiteProviderType.SqlServer && SqlServerProvider == null)
            {
                SqlServerProvider = new WrapOrmLiteProvider(provider);
            }
            else if (type == OrmLiteProviderType.MySql && MySqlProvider == null)
            {
                MySqlProvider = new WrapOrmLiteProvider(provider);
            }
            else if (type == OrmLiteProviderType.Sqlite && SqliteProvider == null)
            {
                SqliteProvider = new WrapOrmLiteProvider(provider);
            }
        }


        static OrmLite()
        {
            //默认注册sqlserver
            RegisterProvider(OrmLiteProviderType.SqlServer, SqlServerOrmLiteProvider.Instance);
        }

        internal static IOrmLiteProvider GetProvider(OrmLiteProviderType type)
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

        /// <summary>
        /// 默认主键
        /// </summary>
        public const string KeyName = "ID";

        private static string defaultKeyName = KeyName;
        /// <summary>
        /// 设置默认主键
        /// </summary>
        public static string DefaultKeyName
        {
            get { return defaultKeyName; }
            set { defaultKeyName = value; }
        }

        private static List<string> updateIgnoreFields = new List<string>() { "AddDate", "AddTime" };
        /// <summary>
        /// 整体修改时，默认忽略修改的字段集合，集合里默认有AddDate,AddTime
        /// </summary>
        public static List<string> UpdateIgnoreFields
        {
            get { return updateIgnoreFields; }
        }
        
    }
}
