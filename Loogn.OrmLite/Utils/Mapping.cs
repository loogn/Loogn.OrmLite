using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;

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
        public static T ReaderToObject<T>(IDataReader reader)
        {
            if (reader.Read())
            {
                var typeInfo = TypeCachedDict.GetTypeCachedInfo(typeof(T));
                T obj = (T)typeInfo.NewInvoker();
                var length = reader.FieldCount;
                object[] values = new object[length];
                reader.GetValues(values);

                for (int i = 0; i < length; i++)
                {
                    var accessor = typeInfo.GetAccessor(reader.GetName(i));
                    if (accessor != null && accessor.CanInvoker)
                    {
                        accessor.SetterInvoker(obj, values[i]);
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
        /// 取reader的首行首列
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static TValue ReaderToScalar<TValue>(IDataReader reader)
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
        public static List<T> ReaderToObjectList<T>(IDataReader reader)
        {
            var typeInfo = TypeCachedDict.GetTypeCachedInfo(typeof(T));
            List<T> list = new List<T>();
            var first = true;
            int length = reader.FieldCount;
            PropAccessor[] propAccessorArr = new PropAccessor[length];
            object[] values = new object[length];
            while (reader.Read())
            {
                reader.GetValues(values);
                T obj = (T)typeInfo.NewInvoker();
                if (first)
                {
                    for (int i = 0; i < length; i++)
                    {
                        var fieldName = reader.GetName(i);
                        var accessor = typeInfo.GetAccessor(fieldName);
                        propAccessorArr[i] = accessor;
                        accessor.SetterInvoker(obj, values[i]);
                    }
                    first = false;
                }
                else
                {
                    for (var i = 0; i < length; i++)
                    {
                        propAccessorArr[i].SetterInvoker(obj, values[i]);
                    }
                }
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

                if (ReferenceEquals(type, Types.Int32))
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
        public static List<T> ReaderToColumnList<T>(IDataReader reader)
        {
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
        public static HashSet<T> ReaderToColumnSet<T>(IDataReader reader)
        {
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
        public static dynamic ReaderToDynamic(IDataReader reader)
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
        public static List<dynamic> ReaderToDynamicList(IDataReader reader)
        {
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
        public static Dictionary<K, List<V>> ReaderToLookup<K, V>(IDataReader reader)
        {

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
        public static Dictionary<K, V> ReaderToDictionary<K, V>(IDataReader reader)
        {
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
