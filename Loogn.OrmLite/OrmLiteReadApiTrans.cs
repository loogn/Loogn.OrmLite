using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace Loogn.OrmLite
{
    public static partial class OrmLiteReadApi
    {
        #region Original Function
        public static DbDataReader ExecuteReader(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            return SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps);
        }

        public static List<T> SelectOriginal<T>(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToObjectList<T>(reader);
            }
        }

        public static List<dynamic> SelectOriginal(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {

            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToDynamicList(reader);
            }
        }

        public static T SingleOriginal<T>(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToObject<T>(reader);
            }
        }

        public static dynamic SingleOriginal(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToDynamic(reader);
            }
        }

        public static T ScalarOriginal<T>(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            var obj = SqlHelper.ExecuteScalar(dbTrans, commandType, commandText, ps);
            return ORM.ConvertToType<T>(obj);
        }

        public static List<T> ColumnOriginal<T>(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToColumnList<T>(reader);
            }
        }

        public static HashSet<T> ColumnDistinctOriginal<T>(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToColumnSet<T>(reader);
            }
        }

        public static Dictionary<K, List<V>> LookupOriginal<K, V>(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToLookup<K, V>(reader);
            }
        }

        public static Dictionary<K, V> DictionaryOriginal<K, V>(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToDictionary<K, V>(reader);
            }
        }

        public static int CountOriginal(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            var obj = SqlHelper.ExecuteScalar(dbTrans, commandType, commandText, ps);
            return ORM.ConvertToType<int>(obj);
        }

        #endregion

        #region Select
        public static List<T> Select<T>(this DbTransaction dbTrans)
        {
            return SelectOriginal<T>(dbTrans, CommandType.Text, SqlCmd.Select<T>(dbTrans.GetProviderType()));
        }

        public static List<T> Select<T>(this DbTransaction dbTrans, string sql)
        {
            return SelectOriginal<T>(dbTrans, CommandType.Text, SqlCmd.FullPartSql<T>(dbTrans.GetProviderType(), sql, PartSqlType.Select), null);
        }

        public static List<T> Select<T>(this DbTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return SelectOriginal<T>(dbTrans, CommandType.Text, SqlCmd.FullPartSql<T>(dbTrans.GetProviderType(), sql, PartSqlType.Select), ORM.DictionaryToParams(dbTrans.GetProviderType(), parameters));
        }

        public static List<T> Select<T>(this DbTransaction dbTrans, string sql, object parameters)
        {
            return SelectOriginal<T>(dbTrans, CommandType.Text, SqlCmd.FullPartSqlSingle<T>(dbTrans.GetProviderType(), sql), ORM.AnonTypeToParams(dbTrans.GetProviderType(), parameters));
        }

        public static List<dynamic> Select(this DbTransaction dbTrans, string sql)
        {
            return SelectOriginal(dbTrans, CommandType.Text, sql, null);
        }

        public static List<dynamic> Select(this DbTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return SelectOriginal(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(dbTrans.GetProviderType(), parameters));
        }

        public static List<dynamic> Select(this DbTransaction dbTrans, string sql, object parameters)
        {
            return SelectOriginal(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(dbTrans.GetProviderType(), parameters));
        }

        public static List<T> SelectWhere<T>(this DbTransaction dbTrans, string name, object value)
        {
            var tuple = SqlCmd.SelectWhere<T>(dbTrans.GetProviderType(), name, value);
            return SelectOriginal<T>(dbTrans, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static List<T> SelectWhere<T>(this DbTransaction dbTrans, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.SelectWhere<T>(dbTrans.GetProviderType(), conditions);
            return SelectOriginal<T>(dbTrans, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static List<T> SelectWhere<T>(this DbTransaction dbTrans, object conditions)
        {
            var tuple = SqlCmd.SelectWhere<T>(dbTrans.GetProviderType(), conditions);
            return SelectOriginal<T>(dbTrans, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static List<T> SelectFmt<T>(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return SelectOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static List<dynamic> SelectFmt(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return SelectOriginal(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static List<T> SelectByIds<T>(this DbTransaction dbTrans, IEnumerable idValues, string idField = OrmLite.KeyName, string selectFields = "*")
        {
            var sql = SqlCmd.SelectByIds<T>(dbTrans.GetProviderType(), idValues, idField, selectFields);
            if (sql == null) return new List<T>();
            return SelectOriginal<T>(dbTrans, CommandType.Text, sql);
        }

        public static List<T> SelectPage<T>(this DbTransaction dbTrans, OrmLitePageFactor factor, out int totalCount)
        {
            if (factor.PageIndex < 1)
            {
                throw new ArgumentException("pageIndex参数应>1");
            }
            if (string.IsNullOrEmpty(factor.OrderBy))
            {
                throw new ArgumentException("orderby参数不能为空或null");
            }
            if (factor.TableName == null || factor.TableName.Length == 0)
            {
                factor.TableName = typeof(T).GetCachedTableName();
            }
            if (string.IsNullOrEmpty(factor.Fields))
            {
                factor.Fields = "*";
            }
            var providerType = dbTrans.GetProviderType();

            var ps = factor.Params is Dictionary<string, object> ?
                ORM.DictionaryToParams(providerType, factor.Params as Dictionary<string, object>)
                : ORM.AnonTypeToParams(providerType, factor.Params);
            StringBuilder sb = new StringBuilder(200);

            sb.AppendFormat("select count(0) from {0}", factor.TableName);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            totalCount = CountOriginal(dbTrans, CommandType.Text, sb.ToString(), ps);

            if (totalCount == 0) //总数为0了，肯定没有数据
            {
                return new List<T>();
            }

            var sql = PageSql(providerType, factor);
            var list = SelectOriginal<T>(dbTrans, CommandType.Text, sql, ps);
            return list;
        }

        public static List<dynamic> SelectPage(this DbTransaction dbTrans, OrmLitePageFactor factor, out int totalCount)
        {
            if (factor.PageIndex < 1)
            {
                throw new ArgumentException("pageIndex参数应>1");
            }
            if (string.IsNullOrEmpty(factor.OrderBy))
            {
                throw new ArgumentException("orderby参数不能为空或null");
            }
            if (factor.TableName == null || factor.TableName.Length == 0)
            {
                throw new ArgumentException("tableName参数不能为空或null");
            }
            if (string.IsNullOrEmpty(factor.Fields))
            {
                factor.Fields = "*";
            }
            var providerType = dbTrans.GetProviderType();
            var ps = factor.Params is Dictionary<string, object> ?
                ORM.DictionaryToParams(providerType, factor.Params as Dictionary<string, object>)
                : ORM.AnonTypeToParams(providerType, factor.Params);
            StringBuilder sb = new StringBuilder(200);

            sb.AppendFormat("select count(0) from {0}", factor.TableName);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            totalCount = CountOriginal(dbTrans, CommandType.Text, sb.ToString(), ps);

            if (totalCount == 0) //总数为0了，肯定没有数据
            {
                return new List<dynamic>();
            }

            var sql = PageSql(providerType, factor);
            var list = SelectOriginal(dbTrans, CommandType.Text, sql, ps);
            return list;
        }
        #endregion

        #region Single

        public static T Single<T>(this DbTransaction dbTrans, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.Single<T>(dbTrans.GetProviderType(), conditions);
            return SingleOriginal<T>(dbTrans, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T Single<T>(this DbTransaction dbTrans, object conditions)
        {
            var tuple = SqlCmd.Single<T>(dbTrans.GetProviderType(), conditions);
            return SingleOriginal<T>(dbTrans, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T Single<T>(this DbTransaction dbTrans, string sql)
        {
            return SingleOriginal<T>(dbTrans, CommandType.Text, SqlCmd.FullPartSqlSingle<T>(dbTrans.GetProviderType(), sql), null);
        }

        public static T Single<T>(this DbTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return SingleOriginal<T>(dbTrans, CommandType.Text, SqlCmd.FullPartSqlSingle<T>(dbTrans.GetProviderType(), sql), ORM.DictionaryToParams(dbTrans.GetProviderType(), parameters));
        }

        public static dynamic Single(this DbTransaction dbTrans, string sql)
        {
            return SingleOriginal(dbTrans, CommandType.Text, sql, null);
        }

        public static dynamic Single(this DbTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return SingleOriginal(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(dbTrans.GetProviderType(), parameters));
        }

        public static dynamic Single(this DbTransaction dbTrans, string sql, object parameters)
        {
            return SingleOriginal(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(dbTrans.GetProviderType(), parameters));
        }

        public static T SingleFmt<T>(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return SingleOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static dynamic SingleFmt(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return SingleOriginal(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static T SingleById<T>(this DbTransaction dbTrans, object idValue, string idField = OrmLite.KeyName)
        {
            var tuple = SqlCmd.SingleById<T>(dbTrans.GetProviderType(), idValue, idField);
            return SingleOriginal<T>(dbTrans, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T SingleWhere<T>(this DbTransaction dbTrans, string name, object value)
        {
            var tuple = SqlCmd.SingleWhere<T>(dbTrans.GetProviderType(), name, value);
            return SingleOriginal<T>(dbTrans, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T SingleWhere<T>(this DbTransaction dbTrans, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.SingleWhere<T>(dbTrans.GetProviderType(), conditions);
            return SingleOriginal<T>(dbTrans, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T SingleWhere<T>(this DbTransaction dbTrans, object conditions)
        {
            var tuple = SqlCmd.SingleWhere<T>(dbTrans.GetProviderType(), conditions);
            return SingleOriginal<T>(dbTrans, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        #endregion

        #region Scalar

        public static T Scalar<T>(this DbTransaction dbTrans, string sql)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql, null);
        }

        public static T Scalar<T>(this DbTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(dbTrans.GetProviderType(), parameters));
        }

        public static T Scalar<T>(this DbTransaction dbTrans, string sql, object parameters)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(dbTrans.GetProviderType(), parameters));
        }

        public static T ScalarFmt<T>(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static T MaxID<T>(this DbTransaction dbTrans, string tableName, string field = "id")
        {
            tableName = SqlInjection.Filter(tableName);
            var sql = string.Format("SELECT ISNULL(MAX({0}), 0) FROM {1}", field, tableName);
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql);
        }
        #endregion

        #region Column

        public static List<T> Column<T>(this DbTransaction dbTrans, string sql)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, sql, null);
        }

        public static List<T> Column<T>(this DbTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(dbTrans.GetProviderType(), parameters));
        }

        public static List<T> Column<T>(this DbTransaction dbTrans, string sql, object parameters)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(dbTrans.GetProviderType(), parameters));
        }

        public static List<T> ColumnFmt<T>(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this DbTransaction dbTrans, string sql)
        {
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, sql);
        }

        public static HashSet<T> ColumnDistinct<T>(this DbTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(dbTrans.GetProviderType(), parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this DbTransaction dbTrans, string sql, object parameters)
        {
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(dbTrans.GetProviderType(), parameters));
        }

        public static HashSet<T> ColumnDistinctFmt<T>(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        #endregion

        #region Lookup Dictionary
        public static Dictionary<K, List<V>> Lookup<K, V>(this DbTransaction dbTrans, string sql)
        {
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, sql, null);
        }
        public static Dictionary<K, List<V>> Lookup<K, V>(this DbTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(dbTrans.GetProviderType(), parameters));
        }
        public static Dictionary<K, List<V>> Lookup<K, V>(this DbTransaction dbTrans, string sql, object parameters)
        {
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(dbTrans.GetProviderType(), parameters));
        }

        public static Dictionary<K, List<V>> LookupFmt<K, V>(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static Dictionary<K, V> Dictionary<K, V>(this DbTransaction dbTrans, string sql)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, sql, null);
        }
        public static Dictionary<K, V> Dictionary<K, V>(this DbTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(dbTrans.GetProviderType(), parameters));
        }
        public static Dictionary<K, V> Dictionary<K, V>(this DbTransaction dbTrans, string sql, object parameters)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(dbTrans.GetProviderType(), parameters));
        }

        public static Dictionary<K, V> DictionaryFmt<K, V>(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

        #region Count
        public static int Count<T>(this DbTransaction dbTrans)
        {
            return CountOriginal(dbTrans, CommandType.Text, SqlCmd.Count<T>(dbTrans.GetProviderType()));
        }

        public static int Count<T>(this DbTransaction dbTrans, string sql)
        {
            return CountOriginal(dbTrans, CommandType.Text, SqlCmd.FullPartSql<T>(dbTrans.GetProviderType(), sql, PartSqlType.Count), null);
        }
        public static int Count<T>(this DbTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return CountOriginal(dbTrans, CommandType.Text, SqlCmd.FullPartSql<T>(dbTrans.GetProviderType(), sql, PartSqlType.Count), ORM.DictionaryToParams(dbTrans.GetProviderType(), parameters));
        }
        public static int Count<T>(this DbTransaction dbTrans, string sql, object parameters)
        {
            return CountOriginal(dbTrans, CommandType.Text, SqlCmd.FullPartSql<T>(dbTrans.GetProviderType(), sql, PartSqlType.Count), ORM.AnonTypeToParams(dbTrans.GetProviderType(), parameters));
        }

        public static int CountWhere<T>(this DbTransaction dbTrans, string name, object value)
        {
            var tuple = SqlCmd.CountWhere<T>(dbTrans.GetProviderType(), name, value);
            return CountOriginal(dbTrans, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static int CountWhere<T>(this DbTransaction dbTrans, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.CountWhere<T>(dbTrans.GetProviderType(), conditions);
            return CountOriginal(dbTrans, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static int CountWhere<T>(this DbTransaction dbTrans, object conditions)
        {
            var tuple = SqlCmd.CountWhere<T>(dbTrans.GetProviderType(), conditions);
            return CountOriginal(dbTrans, CommandType.Text, tuple.Item1, tuple.Item2);
        }


        public static int CountFmt(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return CountOriginal(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

    }
}
