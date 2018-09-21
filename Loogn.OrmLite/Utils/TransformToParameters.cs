using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    public static class TransformToParameters
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
                    p.Value = kv.Value.Get(obj);
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
                    var OpenQuote = dialectProvider.OpenQuote;
                    var CloseQuote = dialectProvider.CloseQuote;

                    var ps = new IDbDataParameter[typeInfo.PropInvokerDict.Count];
                    var index = 0;
                    appendWhere.Append(" where ");
                    foreach (var kv in typeInfo.PropInvokerDict)
                    {
                        var p = dialectProvider.CreateParameter();
                        p.ParameterName = "@" + kv.Key;
                        p.Value = kv.Value.Get(obj);
                        ps[index++] = p;
                        appendWhere.AppendFormat(" {1}{0}{2}=@{0} and ", kv.Key, OpenQuote, CloseQuote);
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
                var OpenQuote = dialectProvider.OpenQuote;
                var CloseQuote = dialectProvider.CloseQuote;
                var pList = new List<IDbDataParameter>(dict.Count);
                var index = 0;
                appendWhere.Append(" where ");
                foreach (var kv in dict)
                {
                    if (kv.Key.IndexOf("$") < 0)
                    {
                        var p = dialectProvider.CreateParameter();
                        p.ParameterName = "@" + kv.Key + index++.ToString();
                        p.Value = kv.Value;
                        pList.Add(p);
                        appendWhere.AppendFormat(" {1}{0}{2}={3} and ", kv.Key, OpenQuote, CloseQuote, p.ParameterName);
                    }
                    else if (kv.Key.EndsWith("$>", StringComparison.OrdinalIgnoreCase) ||
                        kv.Key.EndsWith("$>=", StringComparison.OrdinalIgnoreCase) ||
                        kv.Key.EndsWith("$<", StringComparison.OrdinalIgnoreCase) ||
                        kv.Key.EndsWith("$<=", StringComparison.OrdinalIgnoreCase) ||
                        kv.Key.EndsWith("$!=", StringComparison.OrdinalIgnoreCase) ||
                        kv.Key.EndsWith("$<>", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = kv.Key.Split('$');
                        var p = dialectProvider.CreateParameter();
                        var name = parts[0];
                        var opt = parts[1];
                        p.ParameterName = "@" + name + index++.ToString();
                        p.Value = kv.Value;
                        pList.Add(p);
                        appendWhere.AppendFormat(" {0}{1}{2}{3}{4} and ", OpenQuote, name, CloseQuote, opt, p.ParameterName);
                    }

                    else if (kv.Key.EndsWith("$like", StringComparison.OrdinalIgnoreCase))
                    {
                        var name = kv.Key.Substring(0, kv.Key.Length - 5);
                        appendWhere.AppendFormat(" {1}{0}{2} like '{3}' and ", name, OpenQuote, CloseQuote, kv.Value.ToString());
                    }
                    else if (kv.Key.EndsWith("$in", StringComparison.OrdinalIgnoreCase))
                    {
                        InParams(kv, appendWhere, 3, "in", OpenQuote, CloseQuote);
                    }
                    else if (kv.Key.EndsWith("$notin", StringComparison.OrdinalIgnoreCase))
                    {
                        InParams(kv, appendWhere, 6, "not in", OpenQuote, CloseQuote);
                    }
                }
                appendWhere.Length -= 4;
                return pList.ToArray();
            }
            return null;
        }

        private static void InParams(KeyValuePair<string, object> kv, StringBuilder appendWhere, int optlen, string opt, string OpenQuote, string CloseQuote)
        {
            var name = kv.Key.Substring(0, kv.Key.Length - optlen);
            var ids = kv.Value as string;
            if (!string.IsNullOrEmpty(ids))
            {
                appendWhere.AppendFormat(" {1}{0}{2} {4} ({3}) and ", name, OpenQuote, CloseQuote, ids, opt);
                return;
            }
            var idValues = kv.Value as IEnumerable;
            if (idValues != null)
            {
                bool any = false;
                var needQuot = false;
                var enumerator = idValues.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var obj = enumerator.Current;
                    if (!any)
                    {
                        any = true;
                        if (obj is string || obj is DateTime)
                        {
                            needQuot = true;
                        }
                        appendWhere.AppendFormat(" {1}{0}{2} {3} (", name, OpenQuote, CloseQuote, opt);
                    }
                    if (needQuot)
                    {
                        appendWhere.AppendFormat("'{0}',", obj);
                    }
                    else
                    {
                        appendWhere.AppendFormat("{0},", obj);
                    }
                }
                if (any)
                {
                    appendWhere.Remove(appendWhere.Length - 1, 1);
                    appendWhere.Append(") and ");
                }
            }
            else
            {
                appendWhere.AppendFormat(" {1}{0}{2} {4} ({3}) and ", name, OpenQuote, CloseQuote, "null", opt);
                return;
            }
        }
    }
}
