using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    public static class CommandDialectProviderExtendMethods
    {
        public static ICommandDialectProvider GetCommandDialectProvider(this IDbConnection conn)
        {
            var connTypeName = string.Intern(conn.GetType().Name);

            ICommandDialectProvider provider;
            if (OrmLite.CommandDialectProviderCache.TryGetValue(connTypeName, out provider))
            {
                return provider;
            }
            throw new Exception("未找到所需的CommandDialectProvider，请先调用OrmLite.RegisterProvider()方法注册");
        }

        public static ICommandDialectProvider GetCommandDialectProvider(this IDbTransaction trans)
        {
            return trans.Connection.GetCommandDialectProvider();
        }



        public static IDbDataParameter[] Object2Params(this ICommandDialectProvider provider, object obj)
        {
            return Transform2Parameters.Object2Params(provider, obj);
        }

        public static IDbDataParameter[] Object2Params(this ICommandDialectProvider provider, object obj, StringBuilder appendWhere)
        {
            return Transform2Parameters.Object2Params(provider, obj, appendWhere);
        }

        public static IDbDataParameter[] Dictionary2Params(this ICommandDialectProvider provider, IDictionary<string, object> dict)
        {
            return Transform2Parameters.Dictionary2Params(provider, dict);
        }

        public static IDbDataParameter[] Dictionary2Params(this ICommandDialectProvider provider, IDictionary<string, object> dict, StringBuilder appendWhere)
        {
            return Transform2Parameters.Dictionary2Params(provider, dict, appendWhere);
        }
    }
}
