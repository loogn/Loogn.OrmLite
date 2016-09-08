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
    public static class Mapping
    {
        /// <summary>
        /// 用reader填充T类型的对象，并返回
        /// </summary>
        /// <typeparam name="T">类型参数</typeparam>
        /// <param name="reader">dataReader</param>
        /// <returns></returns>
        public static T ReaderToObject<T>(DbDataReader reader)
        {
            if (reader.Read())
            {
                var refInfo = ReflectionHelper.GetInfo<T>();

                T obj = refInfo.NewInstance(); //Activator.CreateInstance<T>();

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var accessor = refInfo.GetAccessor(reader.GetName(i));
                    if (accessor != null)
                    {
                        accessor.Set(obj, reader.GetValue(i));
                    }
                }
                return obj;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 用Reader填充T类型列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static List<T> ReaderToObjectList<T>(DbDataReader reader)
        {
            if (!reader.HasRows)
            {
                return new List<T>();
            }
            var refInfo = ReflectionHelper.GetInfo<T>();
            List<T> list = new List<T>();
            var first = true;
            int length = reader.FieldCount;
            ReflectionInfo<T>.Accessor[] accessorArray = new ReflectionInfo<T>.Accessor[length];
            while (reader.Read())
            {
                T obj = refInfo.NewInstance();// Activator.CreateInstance<T>();

                if (first)
                {
                    for (int i = 0; i < length; i++)
                    {
                        var fieldName = reader.GetName(i);
                        var accessor = refInfo.GetAccessor(fieldName);
                        accessorArray[i] = accessor;
                        accessor.Set(obj, reader[i]);
                    }
                    first = false;
                }
                else
                {
                    for (var i = 0; i < length; i++)
                    {
                        accessorArray[i].Set(obj, reader[i]);
                    }
                }
                list.Add(obj);
            }

            return list;
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

        internal static List<Tuple<T1, T2>> ReaderToTupleList<T1, T2>(DbDataReader reader)
        {
            if (!reader.HasRows) return new List<Tuple<T1, T2>>();
            var list = new List<Tuple<T1, T2>>();
            while (reader.Read())
            {
                list.Add(new Tuple<T1, T2>((T1)reader[0], (T2)reader[1]));
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
}
