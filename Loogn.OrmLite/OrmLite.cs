using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    public static class OrmLite
    {
        internal static Dictionary<string, ICommandDialectProvider> CommandDialectProviderCache = new Dictionary<string, ICommandDialectProvider>();

        public static IDbConnection Open(string connectionString, ICommandDialectProvider provider)
        {
            var conn = provider.CreateConnection();
            conn.ConnectionString = connectionString;

            var name = conn.GetType().Name;
            if (!CommandDialectProviderCache.ContainsKey(name))
            {
                CommandDialectProviderCache[name] = provider;
            }
            return conn;
        }
        public static IDbConnection Open(string connectionString)
        {
            return Open(connectionString, OrmLite.DefaultCommandDialectProvider);
        }

        #region config

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

        private static ICommandDialectProvider defaultCommandDialectProvider = SqlServerCommandDialectProvider.Instance;
        /// <summary>
        /// 默认命令方言提供程序，不设置的时候是SqlServer
        /// </summary>
        public static ICommandDialectProvider DefaultCommandDialectProvider
        {
            get
            {
                return defaultCommandDialectProvider;
            }
            set
            {
                defaultCommandDialectProvider = value;
            }
        }

        #endregion

    }
}
