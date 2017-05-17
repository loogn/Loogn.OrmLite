using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Data.Common;

namespace Loogn.OrmLite.NetCore
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
                var setAction = refInfo.GetSetterAction(reader);
                T obj = refInfo.NewInstance();
                var length = reader.FieldCount;
                object[] values = new object[length];
                reader.GetValues(values);

                setAction(obj, values);
                return obj;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 取reader的首行首列
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static TValue ReaderToScalar<TValue>(DbDataReader reader)
        {
            if (reader.Read())
            {
                var obj = reader.GetValue(0);
                return ConvertToPrimitiveType<TValue>(obj);
            }
            else
            {
                return default(TValue);
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
            var setAction = refInfo.GetSetterAction(reader);
            List<T> list = new List<T>();
            int length = reader.FieldCount;
            ReflectionInfo<T>.Accessor[] accessorArray = new ReflectionInfo<T>.Accessor[length];
            object[] values = new object[length];
            while (reader.Read())
            {
                reader.GetValues(values);
                T obj = refInfo.NewInstance();
                setAction(obj, values);
                list.Add(obj);
            }

            return list;
        }

        internal static T ConvertToPrimitiveType<T>(object obj)
        {
            if (obj == null || obj is DBNull)
            {
                return default(T);
            }
            else
            {
                var type = typeof(T);
                object newobj = obj;

                if (ReferenceEquals(type, PrimitiveTypes.Int32))
                {
                    newobj = Convert.ToInt32(obj);
                }
                return (T)newobj;

            }
        }

        /// <summary>
        /// 用只有一个列的Reader填充成一个列表
        /// </summary>
        /// <typeparam name="T">对应reader第一个列的类型</typeparam>
        /// <param name="reader">DataReader</param>
        /// <returns></returns>
        public static List<T> ReaderToColumnList<T>(DbDataReader reader)
        {
            if (!reader.HasRows) return new List<T>();
            List<T> list = new List<T>();
            while (reader.Read())
            {
                list.Add(ConvertToPrimitiveType<T>(reader[0]));
            }
            return list;
        }

        /// <summary>
        /// 用只有一个列的reader填充成一个集合
        /// </summary>
        /// <typeparam name="T">对应reader第一个列的类型</typeparam>
        /// <param name="reader">DataReader</param>
        /// <returns></returns>
        public static HashSet<T> ReaderToColumnSet<T>(DbDataReader reader)
        {
            if (!reader.HasRows) return new HashSet<T>();
            HashSet<T> set = new HashSet<T>();
            while (reader.Read())
            {
                set.Add(ConvertToPrimitiveType<T>(reader[0]));
            }
            return set;
        }

        /// <summary>
        /// 用Reader填充dynamic类型
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static dynamic ReaderToDynamic(DbDataReader reader)
        {
            if (reader.Read())
            {
                dynamic obj = new ExpandoObject();
                var dict = obj as IDictionary<string, object>;
                object[] values = new object[reader.FieldCount];
                reader.GetValues(values);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dict.Add(reader.GetName(i), values[i]);
                }
                return obj;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 用Reader填充dynamic列表
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
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
                object[] values = new object[reader.FieldCount];
                reader.GetValues(values);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    dict.Add(reader.GetName(i), values[i]);
                }
                list.Add(obj);
            }
            return list;
        }

        /// <summary>
        /// 用reader的第一列做为key，把第二列聚合成列表
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Dictionary<K, List<V>> ReaderToLookup<K, V>(DbDataReader reader)
        {
            if (!reader.HasRows) return new Dictionary<K, List<V>>();

            var dict = new Dictionary<K, List<V>>();
            var values = new object[2];
            while (reader.Read())
            {
                reader.GetValues(values);
                var key = (K)values[0];
                var value = (V)values[1];
                List<V> valueList = null;
                if (dict.TryGetValue(key, out valueList))
                {
                    valueList.Add(value);
                }
                else
                {
                    valueList = new List<V>();
                    valueList.Add(value);
                    dict.Add(key, valueList);
                }
            }
            return dict;
        }


        /// <summary>
        /// 用reader的第一列做为key，用第二列作为Key
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Dictionary<K, V> ReaderToDictionary<K, V>(DbDataReader reader)
        {
            if (!reader.HasRows) return new Dictionary<K, V>();

            var dict = new Dictionary<K, V>();
            var values = new object[2];
            while (reader.Read())
            {
                reader.GetValues(values);
                dict[(K)values[0]] = (V)values[1];
            }
            return dict;
        }
    }
}
