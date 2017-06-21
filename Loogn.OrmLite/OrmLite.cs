using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    /// <summary>
    /// OrmLite
    /// </summary>
    public static class OrmLite
    {
        internal static Dictionary<string, ICommandDialectProvider> CommandDialectProviderCache = new Dictionary<string, ICommandDialectProvider>();

        /// <summary>
        /// 注册命令方言提供程序
        /// </summary>
        /// <param name="provider"></param>
        public static void RegisterProvider(ICommandDialectProvider provider)
        {
            var conn = provider.CreateConnection();
            var connName = conn.GetType().Name;
            if (!CommandDialectProviderCache.ContainsKey(connName))
            {
                CommandDialectProviderCache[connName] = provider;
            }
        }
        static OrmLite()
        {
            RegisterProvider(SqlServerCommandDialectProvider.Instance);
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

        #endregion

    }
}
