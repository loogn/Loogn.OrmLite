using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    static class TransformToParameters
    {
        public static IDbDataParameter[] ObjectToParams(ICommandDialectProvider dialectProvider, object obj)
        {
            if (obj != null)
            {
                var typeInfo = TypeCachedDict.GetTypeCachedInfo(obj.GetType());
                var ps = new IDbDataParameter[typeInfo.PropInvokerDict.Count];
                var index = 0;
                foreach (var kv in typeInfo.PropInvokerDict)
                {
                    var p = dialectProvider.CreateParameter();
                    p.ParameterName = "@" + kv.Key;
                    p.Value = kv.Value.GetterInvoker(obj);
                    ps[index++] = p;
                }
                return ps;
            }
            return null;
        }

        public static IDbDataParameter[] ObjectToParams(ICommandDialectProvider dialectProvider, object obj, StringBuilder appendWhere)
        {
            if (obj != null)
            {
                var typeInfo = TypeCachedDict.GetTypeCachedInfo(obj.GetType());
                if (typeInfo.PropInvokerDict.Count > 0)
                {
                    var ps = new IDbDataParameter[typeInfo.PropInvokerDict.Count];
                    var index = 0;
                    appendWhere.Append(" where ");
                    foreach (var kv in typeInfo.PropInvokerDict)
                    {
                        var p = dialectProvider.CreateParameter();
                        p.ParameterName = "@" + kv.Key;
                        p.Value = kv.Value.GetterInvoker(obj);
                        ps[index++] = p;
                        appendWhere.AppendFormat(" {0}=@{0} and ", kv.Key);
                    }
                    return ps;
                }
            }
            return null;
        }

        public static IDbDataParameter[] DictionaryToParams(ICommandDialectProvider dialectProvider, IDictionary<string, object> dict)
        {
            if (dict != null && dict.Count > 0)
            {
                var ps = new IDbDataParameter[dict.Count];
                var index = 0;
                foreach (var kv in dict)
                {
                    var p = dialectProvider.CreateParameter();
                    p.ParameterName = "@" + kv.Key;
                    p.Value = kv.Value;
                    ps[index++] = p;
                }
                return ps;
            }
            return null;
        }

        public static IDbDataParameter[] DictionaryToParams(ICommandDialectProvider dialectProvider, IDictionary<string, object> dict, StringBuilder appendWhere)
        {
            if (dict != null && dict.Count > 0)
            {
                var ps = new IDbDataParameter[dict.Count];
                var index = 0;
                appendWhere.Append(" where ");
                foreach (var kv in dict)
                {
                    var p = dialectProvider.CreateParameter();
                    p.ParameterName = "@" + kv.Key;
                    p.Value = kv.Value;
                    ps[index++] = p;
                    appendWhere.AppendFormat(" {0}=@{0} and ", kv.Key);
                }
                appendWhere.Length -= 4;
                return ps;
            }
            return null;
        }
    }
}
