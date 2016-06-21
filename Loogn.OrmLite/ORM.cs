using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace Loogn.OrmLite
{

    /// <summary>
    /// ORM映射类，从reader到模型
    /// </summary>
    public static class ORM
    {
        public static T ReaderToObject<T>(DbDataReader reader)
        {
            if (reader.Read())
            {
                T obj = Activator.CreateInstance<T>();
                var type = typeof(T);
                var props = type.GetCachedProperties();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var prop = props.FirstOrDefault(p => p.Name.Equals(reader.GetName(i), StringComparison.OrdinalIgnoreCase));
                    if (null != prop)
                    {
                        var value = reader.GetValue(i);
                        SetPropertyValue(obj, prop, value);
                    }
                }
                return obj;
            }
            else
            {
                return default(T);
            }
        }

        private static void SetPropertyValue(object obj, PropertyInfo prop, object value)
        {
            var propType = prop.PropertyType;

            if (null == value || value is DBNull)
                prop.SetValue(obj, null, null);
            else
            {
                if (propType == typeof(bool))
                {
                    if (value.GetType() == typeof(bool))
                    {
                        prop.SetValue(obj, value, null);
                    }
                    else
                    {
                        var boolValue = Convert.ToInt32(value);
                        prop.SetValue(obj, boolValue > 0, null);
                    }
                }
                else if (propType == typeof(byte))
                {
                    var byteValue = Convert.ToByte(value);
                    prop.SetValue(obj, byteValue, null);
                }
                else
                {
                    prop.SetValue(obj, value, null);
                }
            }
        }

        internal static T ConvertToType<T>(object obj)
        {
            if (obj == null || obj is DBNull)
            {
                return default(T);
            }
            else
            {
                var type = typeof(T);
                object newobj = obj;
                if (type == typeof(int))
                {
                    newobj = Convert.ToInt32(obj);
                }
                return (T)newobj;

            }
        }

        public static List<T> ReaderToObjectList<T>(DbDataReader reader)
        {
            if (!reader.HasRows)
            {
                return new List<T>();
            }
            var type = typeof(T);
            var props = type.GetCachedProperties();
            List<T> list = new List<T>();

            var first = true;
            PropertyInfo[] propArr = new PropertyInfo[reader.FieldCount];
            while (reader.Read())
            {
                T obj = Activator.CreateInstance<T>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    PropertyInfo prop = null;
                    if (first)
                    {
                        prop = props.FirstOrDefault(p => p.Name.Equals(reader.GetName(i), StringComparison.OrdinalIgnoreCase));
                        propArr[i] = prop;
                    }
                    else
                    {
                        prop = propArr[i];
                    }
                    if (null != prop)
                    {
                        var value = reader.GetValue(i);
                        SetPropertyValue(obj, prop, value);
                    }
                }
                list.Add(obj);
                first = false;
            }
            return list;
        }

        internal static List<MyTuple<T1, T2>> ReaderToTupleList<T1, T2>(DbDataReader reader)
        {
            if (!reader.HasRows) return new List<MyTuple<T1, T2>>();
            var list = new List<MyTuple<T1, T2>>();
            while (reader.Read())
            {
                var tuple = new MyTuple<T1, T2>();
                tuple.Item1 = (T1)reader[0];
                tuple.Item2 = (T2)reader[1];
                list.Add(tuple);
            }
            return list;
        }

        public static List<T> ReaderToColumnList<T>(DbDataReader reader)
        {
            if (!reader.HasRows) return new List<T>();
            List<T> list = new List<T>();
            while (reader.Read())
            {
                list.Add(ConvertToType<T>(reader[0]));
            }
            return list;
        }

        public static HashSet<T> ReaderToColumnSet<T>(DbDataReader reader)
        {
            if (!reader.HasRows) return new HashSet<T>();
            HashSet<T> set = new HashSet<T>();
            while (reader.Read())
            {
                set.Add(ConvertToType<T>(reader[0]));
            }
            return set;
        }

        public static dynamic ReaderToDynamic(DbDataReader reader)
        {
            if (reader.Read())
            {
                dynamic obj = new ExpandoObject();
                var dict = obj as IDictionary<string, object>;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dict.Add(reader.GetName(i), reader.GetValue(i));
                }
                return obj;
            }
            else
            {
                return null;
            }
        }

        public static List<dynamic> ReaderToDynamicList(DbDataReader reader)
        {
            if (!reader.HasRows)
            {
                return new List<dynamic>();
            }
            List<dynamic> list = new List<dynamic>();
            while (reader.Read())
            {
                dynamic obj = new ExpandoObject();
                var dict = obj as IDictionary<string, object>;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dict.Add(reader.GetName(i), reader.GetValue(i));
                }
                list.Add(obj);
            }
            return list;
        }

        private static void ReaderToJson(DbDataReader reader, StringBuilder result)
        {
            result.Append("{");
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var val = reader.GetValue(i);
                if (val is DBNull)
                {
                    result.AppendFormat("\"{0}\":null", reader.GetName(i));
                }
                else
                {
                    var type = val.GetType();
                    if (type == typeof(DateTime) || type == typeof(string))
                    {
                        result.AppendFormat("\"{0}\":\"{1}\"", reader.GetName(i), val.ToString());
                    }
                    else if (type == typeof(bool))
                    {
                        result.AppendFormat("\"{0}\":{1}", reader.GetName(i), true.Equals(val) ? "true" : "false");
                    }
                    else
                    {
                        result.AppendFormat("\"{0}\":{1}", reader.GetName(i), val.ToString());
                    }
                }

                if (i < reader.FieldCount - 1)
                {
                    result.Append(",");
                }
            }
            result.Append("}");
        }

        public static string ReaderToJsonArray(DbDataReader reader)
        {
            StringBuilder json = new StringBuilder(500);
            while (reader.Read())
            {
                ReaderToJson(reader, json);
                json.Append(",");
            }
            if (json.Length == 0)
            {
                json.Append("[]");
            }
            else
            {
                json.Remove(json.Length - 1, 1);
                json.Insert(0, "[");
                json.Append("]");
            }
            return json.ToString();
        }

        public static string ReaderToJsonObject(DbDataReader reader)
        {
            if (reader.Read())
            {
                StringBuilder sb = new StringBuilder(100);
                ReaderToJson(reader, sb);
                return sb.ToString();
            }
            else
            {
                return "{}";
            }
        }

        public static DbParameter[] AnonTypeToParams(OrmLiteProviderType type, object anonType)
        {
            if (anonType != null)
            {

                var provider = OrmLite.GetProvider(type);

                var props = anonType.GetType().GetCachedProperties();
                var ps = new DbParameter[props.Length];
                for (int i = 0, len = props.Length; i < len; i++)
                {
                    var prop = props[i];
                    var p = provider.CreateParameter("@" + prop.Name, prop.GetValue(anonType, null));

                    ps[i] = p;
                }
                return ps;
            }
            return null;
        }

        public static DbParameter[] AnonTypeToParams(OrmLiteProviderType type, object anonType, StringBuilder appendWhere)
        {
            var props = anonType.GetType().GetCachedProperties();

            if (props.Length > 0)
            {
                var provider = OrmLite.GetProvider(type);

                DbParameter[] ps = new DbParameter[props.Length];
                int i = 0;
                appendWhere.Append(" where ");
                foreach (var prop in props)
                {
                    var p = provider.CreateParameter("@" + prop.Name, prop.GetValue(anonType, null));

                    ps[i++] = p;
                    appendWhere.AppendFormat(" {0}=@{0} and ", prop.Name);
                }
                appendWhere.Length -= 4;
                return ps;
            }
            return null;
        }

        public static DbParameter[] DictionaryToParams(OrmLiteProviderType type, IDictionary<string, object> dict)
        {
            if (dict != null)
            {
                var provider = OrmLite.GetProvider(type);

                DbParameter[] ps = new DbParameter[dict.Count];
                int i = 0;
                foreach (var kv in dict)
                {
                    var p = provider.CreateParameter("@" + kv.Key, kv.Value);
                    ps[i++] = p;
                }
                return ps;
            }
            return null;
        }

        public static DbParameter[] DictionaryToParams(OrmLiteProviderType type, IDictionary<string, object> conditions, StringBuilder appendWhere)
        {
            if (conditions != null && conditions.Count > 0)
            {
                var provider = OrmLite.GetProvider(type);

                DbParameter[] ps = new DbParameter[conditions.Count];
                int i = 0;
                appendWhere.Append(" where ");
                foreach (var kv in conditions)
                {
                    var p = provider.CreateParameter("@" + kv.Key, kv.Value);

                    ps[i++] = p;
                    appendWhere.AppendFormat(" {0}=@{0} and ", kv.Key);
                }
                appendWhere.Length -= 4;
                return ps;
            }
            return null;
        }

        public static Dictionary<K, List<V>> ReaderToLookup<K, V>(DbDataReader reader)
        {
            if (!reader.HasRows) return new Dictionary<K, List<V>>();
            var list = ReaderToTupleList<K, V>(reader);
            var dict = new Dictionary<K, List<V>>(list.Count / 2);
            foreach (var tuple in list)
            {
                List<V> value = null;
                if (!dict.TryGetValue(tuple.Item1, out value))
                {
                    value = new List<V>();
                    dict.Add(tuple.Item1, value);
                }
                value.Add(tuple.Item2);
            }
            return dict;
        }

        public static Dictionary<K, V> ReaderToDictionary<K, V>(DbDataReader reader)
        {
            if (!reader.HasRows) return new Dictionary<K, V>();
            var list = ReaderToTupleList<K, V>(reader);
            var dict = new Dictionary<K, V>(list.Count);
            foreach (var tuple in list)
            {
                dict[tuple.Item1] = tuple.Item2;
            }
            return dict;
        }


    }
    internal class MyTuple<T1, T2>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
    }

    internal enum PartSqlType
    {
        Select,
        Count
    }

}
