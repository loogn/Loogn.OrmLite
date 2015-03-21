using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    internal static class ORM
    {
        public static T ReaderToObject<T>(SqlDataReader reader)
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
                        if (null == value || value is DBNull)
                            prop.SetValue(obj, null, null);
                        else
                            prop.SetValue(obj, value, null);
                    }
                }
                return obj;
            }
            else
            {
                return default(T);
            }
        }

        public static List<T> ReaderToObjectList<T>(SqlDataReader reader)
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
                        if (null == value || value is DBNull)
                            prop.SetValue(obj, null, null);
                        else
                            prop.SetValue(obj, value, null);
                    }
                }
                list.Add(obj);
                first = false;
            }
            return list;
        }

        internal static List<MyTuple<T1, T2>> ReaderToTupleList<T1, T2>(SqlDataReader reader)
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

        public static List<T> ReaderToColumnList<T>(SqlDataReader reader)
        {
            if (!reader.HasRows) return new List<T>();
            List<T> list = new List<T>();
            while (reader.Read())
            {
                list.Add((T)reader[0]);
            }
            return list;
        }

        public static HashSet<T> ReaderToColumnSet<T>(SqlDataReader reader)
        {
            if (!reader.HasRows) return new HashSet<T>();
            HashSet<T> set = new HashSet<T>();
            while (reader.Read())
            {
                set.Add((T)reader[0]);
            }
            return set;
        }

        public static dynamic ReaderToDynamic(SqlDataReader reader)
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

        public static List<dynamic> ReaderToDynamicList(SqlDataReader reader)
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

        private static void ReaderToJson(SqlDataReader reader, StringBuilder result)
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

        public static string ReaderToJsonArray(SqlDataReader reader)
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

        public static string ReaderToJsonObject(SqlDataReader reader)
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

        public static SqlParameter[] AnonTypeToParams(object anonType)
        {
            if (anonType != null)
            {
                var props = anonType.GetType().GetCachedProperties();
                var ps = new SqlParameter[props.Length];
                for (int i = 0, len = props.Length; i < len; i++)
                {
                    var prop = props[i];
                    ps[i] = new SqlParameter("@" + prop.Name, prop.GetValue(anonType, null));
                }
                return ps;
            }
            return null;
        }

        public static SqlParameter[] AnonTypeToParams(object anonType, StringBuilder appendWhere)
        {
            var props = anonType.GetType().GetCachedProperties();

            if (props.Length>0)
            {
                SqlParameter[] ps = new SqlParameter[props.Length];
                int i = 0;
                appendWhere.Append(" where ");
                foreach (var prop in props)
                {
                    ps[i++] = new SqlParameter("@" + prop.Name, prop.GetValue(anonType, null));
                    appendWhere.AppendFormat(" [{0}]=@{0} and ", prop.Name);
                }
                appendWhere.Length -= 4;
                return ps;
            }
            return null;
        }

        public static SqlParameter[] DictionaryToParams(Dictionary<string, object> dict)
        {
            if (dict != null)
            {
                SqlParameter[] ps = new SqlParameter[dict.Count];
                int i = 0;
                foreach (var kv in dict)
                {
                    ps[i++] = new SqlParameter("@" + kv.Key, kv.Value);
                }
                return ps;
            }
            return null;
        }

        public static SqlParameter[] DictionaryToParams(Dictionary<string, object> conditions, StringBuilder appendWhere)
        {
            if (conditions != null && conditions.Count > 0)
            {
                SqlParameter[] ps = new SqlParameter[conditions.Count];
                int i = 0;
                appendWhere.Append(" where ");
                foreach (var kv in conditions)
                {
                    ps[i++] = new SqlParameter("@" + kv.Key, kv.Value);
                    appendWhere.AppendFormat(" [{0}]=@{0} and ", kv.Key);
                }
                appendWhere.Length -= 4;
                return ps;
            }
            return null;
        }

        public static Dictionary<K, List<V>> ReaderToLookup<K, V>(SqlDataReader reader)
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

        public static Dictionary<K, V> ReaderToDictionary<K, V>(SqlDataReader reader)
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

        public static string FullPartSql<T>(string sql, PartSqlType type)
        {
            sql = sql.TrimStart();
            if (sql.StartsWith("SELECT"))
            {
                return sql;
            }
            var tableName = typeof(T).GetCachedTableName();
            StringBuilder sb = new StringBuilder(sql.Length + 50);
            switch (type)
            {
                case PartSqlType.Select:
                    return sb.AppendFormat("SELECT * FROM [{0}] where {1}", tableName, sql).ToString();
                case PartSqlType.Single:
                    return sb.AppendFormat("SELECT TOP 1 * FROM [{0}] where {1}", tableName, sql).ToString();
                case PartSqlType.Count:
                    return sb.AppendFormat("SELECT COUNT(0) FROM [{0}] where {1}", tableName, sql).ToString();
                default:
                    return sql;
            }
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
        Single,
        Count
    }

}
