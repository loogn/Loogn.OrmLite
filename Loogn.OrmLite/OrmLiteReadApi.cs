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
        public static DbDataReader ExecuteReader(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            return SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps);
        }

        public static List<T> SelectOriginal<T>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToObjectList<T>(reader);
            }
        }

        public static List<dynamic> SelectOriginal(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToDynamicList(reader);
            }
        }

        public static T SingleOriginal<T>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToObject<T>(reader);
            }
        }

        public static dynamic SingleOriginal(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToDynamic(reader);
            }
        }

        public static T ScalarOriginal<T>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            var obj = SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
            return ORM.ConvertToType<T>(obj);
        }

        public static List<T> ColumnOriginal<T>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToColumnList<T>(reader);
            }
        }

        public static HashSet<T> ColumnDistinctOriginal<T>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToColumnSet<T>(reader);
            }
        }

        public static Dictionary<K, List<V>> LookupOriginal<K, V>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToLookup<K, V>(reader);
            }
        }

        public static Dictionary<K, V> DictionaryOriginal<K, V>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToDictionary<K, V>(reader);
            }
        }

        public static int CountOriginal(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            var obj = SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
            return ORM.ConvertToType<int>(obj);

        }

        #endregion

        #region Select
        public static List<T> Select<T>(this DbConnection dbConn)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, SqlCmd.Select<T>());
        }

        public static List<T> Select<T>(this DbConnection dbConn, string sql)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Select), null);
        }

        public static List<T> Select<T>(this DbConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Select), ORM.DictionaryToParams(dbConn.GetProviderType(), parameters));
        }

        public static List<T> Select<T>(this DbConnection dbConn, string sql, object parameters)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Select), ORM.AnonTypeToParams(dbConn.GetProviderType(), parameters));
        }

        public static List<dynamic> Select(this DbConnection dbConn, string sql)
        {
            return SelectOriginal(dbConn, CommandType.Text, sql, null);
        }

        public static List<dynamic> Select(this DbConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return SelectOriginal(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(dbConn.GetProviderType(), parameters));
        }

        public static List<dynamic> Select(this DbConnection dbConn, string sql, object parameters)
        {
            return SelectOriginal(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(dbConn.GetProviderType(), parameters));
        }

        public static List<T> SelectWhere<T>(this DbConnection dbConn, string name, object value)
        {
            var tuple = SqlCmd.SelectWhere<T>(dbConn.GetProviderType(), name, value);
            return SelectOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static List<T> SelectWhere<T>(this DbConnection dbConn, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.SelectWhere<T>(dbConn.GetProviderType(), conditions);
            return SelectOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static List<T> SelectWhere<T>(this DbConnection dbConn, object conditions)
        {
            var tuple = SqlCmd.SelectWhere<T>(dbConn.GetProviderType(), conditions);
            return SelectOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static List<T> SelectFmt<T>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static List<dynamic> SelectFmt(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return SelectOriginal(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static List<T> SelectByIds<T>(this DbConnection dbConn, IEnumerable idValues, string idField = OrmLite.KeyName, string selectFields = "*")
        {
            var sql = SqlCmd.SelectByIds<T>(idValues, idField, selectFields);
            if (sql == null) return new List<T>();
            return SelectOriginal<T>(dbConn, CommandType.Text, sql);
        }

        public static List<T> SelectPage<T>(this DbConnection dbConn, OrmLitePageFactor factor, out int totalCount)
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

            var providerType = dbConn.GetProviderType();

            var ps = factor.Params is Dictionary<string, object> ?
                ORM.DictionaryToParams(providerType, factor.Params as Dictionary<string, object>)
                : ORM.AnonTypeToParams(providerType, factor.Params);
            StringBuilder sb = new StringBuilder(200);

            sb.AppendFormat("select count(0) from {0}", factor.TableName);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            totalCount = CountOriginal(dbConn, CommandType.Text, sb.ToString(), ps);

            if (totalCount == 0) //总数为0了，肯定没有数据
            {
                return new List<T>();
            }


            var sql = PageSql(providerType, factor);
            var list = SelectOriginal<T>(dbConn, CommandType.Text, sql, ps);
            return list;
        }

        public static List<dynamic> SelectPage(this DbConnection dbConn, OrmLitePageFactor factor, out int totalCount)
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
            var providerType = dbConn.GetProviderType();
            var ps = factor.Params is Dictionary<string, object> ?
                ORM.DictionaryToParams(providerType, factor.Params as Dictionary<string, object>)
                : ORM.AnonTypeToParams(providerType, factor.Params);
            StringBuilder sb = new StringBuilder(200);

            sb.AppendFormat("select count(0) from {0}", factor.TableName);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            totalCount = CountOriginal(dbConn, CommandType.Text, sb.ToString(), ps);

            if (totalCount == 0) //总数为0了，肯定没有数据
            {
                return new List<dynamic>();
            }
            var sql = PageSql(providerType, factor);

            var list = SelectOriginal<dynamic>(dbConn, CommandType.Text, sql, ps);
            return list;
        }


        private static string PageSql(OrmLiteProviderType providerType, OrmLitePageFactor factor)
        {
            StringBuilder sb = new StringBuilder(100);
            if (providerType == OrmLiteProviderType.SqlServer)
            {
                sb.AppendFormat("select * from (");
                sb.AppendFormat(" select top {0} {1},ROW_NUMBER() over(order by {2}) rowid from {3}", factor.PageIndex * factor.PageSize, factor.Fields, factor.OrderBy, factor.TableName);
                if (!string.IsNullOrEmpty(factor.Conditions))
                {
                    sb.AppendFormat(" where {0}", factor.Conditions);
                }
                sb.AppendFormat(")t where t.rowid>{0}", (factor.PageIndex - 1) * factor.PageSize);
            }
            else if (providerType == OrmLiteProviderType.MySql)
            {
                sb.AppendFormat("select {0} from {1}", factor.Fields, factor.TableName);
                if (!string.IsNullOrEmpty(factor.Conditions))
                {
                    sb.AppendFormat(" where {0}", factor.Conditions);
                }
                sb.AppendFormat(" order by {0} limit {1},{2}", factor.OrderBy, (factor.PageIndex - 1) * factor.PageSize, factor.PageSize);
            }
            return sb.ToString();
        }
        #endregion

        #region Single

        public static T Single<T>(this DbConnection dbConn, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.Single<T>(dbConn.GetProviderType(), conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T Single<T>(this DbConnection dbConn, object conditions)
        {
            var tuple = SqlCmd.Single<T>(dbConn.GetProviderType(), conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T Single<T>(this DbConnection dbConn, string sql)
        {
            return SingleOriginal<T>(dbConn, CommandType.Text, SqlCmd.FullPartSqlSingle<T>(dbConn.GetProviderType(), sql), null);
        }

        public static T Single<T>(this DbConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return SingleOriginal<T>(dbConn, CommandType.Text, SqlCmd.FullPartSqlSingle<T>(dbConn.GetProviderType(), sql), ORM.DictionaryToParams(dbConn.GetProviderType(), parameters));
        }

        public static dynamic Single(this DbConnection dbConn, string sql)
        {
            return SingleOriginal(dbConn, CommandType.Text, sql, null);
        }

        public static dynamic Single(this DbConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return SingleOriginal(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(dbConn.GetProviderType(), parameters));
        }

        public static dynamic Single(this DbConnection dbConn, string sql, object parameters)
        {
            return SingleOriginal(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(dbConn.GetProviderType(), parameters));
        }

        public static T SingleFmt<T>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return SingleOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static dynamic SingleFmt(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return SingleOriginal(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static T SingleById<T>(this DbConnection dbConn, object idValue, string idField = OrmLite.KeyName)
        {
            var tuple = SqlCmd.SingleById<T>(dbConn.GetProviderType(), idValue, idField);
            return SingleOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T SingleWhere<T>(this DbConnection dbConn, string name, object value)
        {
            var tuple = SqlCmd.SingleWhere<T>(dbConn.GetProviderType(), name, value);
            return SingleOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T SingleWhere<T>(this DbConnection dbConn, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.SingleWhere<T>(dbConn.GetProviderType(), conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T SingleWhere<T>(this DbConnection dbConn, object conditions)
        {
            var tuple = SqlCmd.SingleWhere<T>(dbConn.GetProviderType(), conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        #endregion

        #region Scalar

        public static T Scalar<T>(this DbConnection dbConn, string sql)
        {
            return ScalarOriginal<T>(dbConn, CommandType.Text, sql, null);
        }

        public static T Scalar<T>(this DbConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return ScalarOriginal<T>(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(dbConn.GetProviderType(), parameters));
        }

        public static T Scalar<T>(this DbConnection dbConn, string sql, object parameters)
        {
            return ScalarOriginal<T>(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(dbConn.GetProviderType(), parameters));
        }

        public static T ScalarFmt<T>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return ScalarOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static T MaxID<T>(this DbConnection dbConn, string tableName, string field = "id")
        {
            tableName = SqlInjection.Filter(tableName);
            var sql = string.Format("SELECT ISNULL(MAX({0}), 0) FROM {1}", field, tableName);
            return ScalarOriginal<T>(dbConn, CommandType.Text, sql);
        }

        #endregion

        #region Column

        public static List<T> Column<T>(this DbConnection dbConn, string sql)
        {
            return ColumnOriginal<T>(dbConn, CommandType.Text, sql, null);
        }

        public static List<T> Column<T>(this DbConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return ColumnOriginal<T>(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(dbConn.GetProviderType(), parameters));
        }

        public static List<T> Column<T>(this DbConnection dbConn, string sql, object parameters)
        {
            return ColumnOriginal<T>(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(dbConn.GetProviderType(), parameters));
        }

        public static List<T> ColumnFmt<T>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return ColumnOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this DbConnection dbConn, string sql)
        {
            return ColumnDistinctOriginal<T>(dbConn, CommandType.Text, sql);
        }

        public static HashSet<T> ColumnDistinct<T>(this DbConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return ColumnDistinctOriginal<T>(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(dbConn.GetProviderType(), parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this DbConnection dbConn, string sql, object parameters)
        {
            return ColumnDistinctOriginal<T>(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(dbConn.GetProviderType(), parameters));
        }

        public static HashSet<T> ColumnDistinctFmt<T>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return ColumnDistinctOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        #endregion

        #region Lookup Dictionary
        public static Dictionary<K, List<V>> Lookup<K, V>(this DbConnection dbConn, string sql)
        {
            return LookupOriginal<K, V>(dbConn, CommandType.Text, sql, null);
        }
        public static Dictionary<K, List<V>> Lookup<K, V>(this DbConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return LookupOriginal<K, V>(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(dbConn.GetProviderType(), parameters));
        }
        public static Dictionary<K, List<V>> Lookup<K, V>(this DbConnection dbConn, string sql, object parameters)
        {
            return LookupOriginal<K, V>(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(dbConn.GetProviderType(), parameters));
        }

        public static Dictionary<K, List<V>> LookupFmt<K, V>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return LookupOriginal<K, V>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static Dictionary<K, V> Dictionary<K, V>(this DbConnection dbConn, string sql)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, sql, null);
        }
        public static Dictionary<K, V> Dictionary<K, V>(this DbConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(dbConn.GetProviderType(), parameters));
        }
        public static Dictionary<K, V> Dictionary<K, V>(this DbConnection dbConn, string sql, object parameters)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(dbConn.GetProviderType(), parameters));
        }

        public static Dictionary<K, V> DictionaryFmt<K, V>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

        #region Count
        public static int Count<T>(this DbConnection dbConn)
        {
            return CountOriginal(dbConn, CommandType.Text, SqlCmd.Count<T>());
        }

        public static int Count<T>(this DbConnection dbConn, string sql)
        {
            return CountOriginal(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Count), null);
        }
        public static int Count<T>(this DbConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return CountOriginal(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Count), ORM.DictionaryToParams(dbConn.GetProviderType(), parameters));
        }
        public static int Count<T>(this DbConnection dbConn, string sql, object parameters)
        {
            return CountOriginal(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Count), ORM.AnonTypeToParams(dbConn.GetProviderType(), parameters));
        }

        public static int CountWhere<T>(this DbConnection dbConn, string name, object value)
        {
            var tuple = SqlCmd.CountWhere<T>(dbConn.GetProviderType(), name, value);
            return CountOriginal(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static int CountWhere<T>(this DbConnection dbConn, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.CountWhere<T>(dbConn.GetProviderType(), conditions);
            return CountOriginal(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static int CountWhere<T>(this DbConnection dbConn, object conditions)
        {
            var tuple = SqlCmd.CountWhere<T>(dbConn.GetProviderType(), conditions);
            return CountOriginal(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }


        public static int CountFmt(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return CountOriginal(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

    }
}
