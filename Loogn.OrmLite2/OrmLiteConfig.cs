using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite2
{
    public class OrmLiteConfig
    {
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
    }
}
