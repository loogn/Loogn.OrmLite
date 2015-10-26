using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    public static partial class OrmLiteReadApi
    {
        #region Original Function
        public static SqlDataReader ExecuteReader(this SqlConnection dbConn, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            return SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps);
        }

        public static List<T> SelectOriginal<T>(this SqlConnection dbConn, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToObjectList<T>(reader);
            }
        }

        public static List<dynamic> SelectOriginal(this SqlConnection dbConn, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToDynamicList(reader);
            }
        }

        public static T SingleOriginal<T>(this SqlConnection dbConn, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToObject<T>(reader);
            }
        }

        public static dynamic SingleOriginal(this SqlConnection dbConn, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToDynamic(reader);
            }
        }

        public static T ScalarOriginal<T>(this SqlConnection dbConn, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            var obj = SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
            if (obj == null || obj is DBNull)
            {
                return default(T);
            }
            else
            {
                return (T)obj;
            }
        }

        public static List<T> ColumnOriginal<T>(this SqlConnection dbConn, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToColumnList<T>(reader);
            }
        }

        public static HashSet<T> ColumnDistinctOriginal<T>(this SqlConnection dbConn, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToColumnSet<T>(reader);
            }
        }

        public static Dictionary<K, List<V>> LookupOriginal<K, V>(this SqlConnection dbConn, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToLookup<K, V>(reader);
            }
        }

        public static Dictionary<K, V> DictionaryOriginal<K, V>(this SqlConnection dbConn, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return ORM.ReaderToDictionary<K, V>(reader);
            }
        }

        public static int CountOriginal(this SqlConnection dbConn, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            var obj = SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
            if(obj==null || obj is DBNull)
            {
                return 0;
            }
            return Convert.ToInt32(obj);
        }

        #endregion

        #region Select
        public static List<T> Select<T>(this SqlConnection dbConn)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, SqlCmd.Select<T>());
        }

        public static List<T> Select<T>(this SqlConnection dbConn, string sql)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Select), null);
        }

        public static List<T> Select<T>(this SqlConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Select), ORM.DictionaryToParams(parameters));
        }

        public static List<T> Select<T>(this SqlConnection dbConn, string sql, object parameters)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Select), ORM.AnonTypeToParams(parameters));
        }

        public static List<dynamic> Select(this SqlConnection dbConn, string sql)
        {
            return SelectOriginal(dbConn, CommandType.Text, sql, null);
        }

        public static List<dynamic> Select(this SqlConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return SelectOriginal(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }

        public static List<dynamic> Select(this SqlConnection dbConn, string sql, object parameters)
        {
            return SelectOriginal(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static List<T> SelectWhere<T>(this SqlConnection dbConn, string name, object value)
        {
            var tuple = SqlCmd.SelectWhere<T>(name, value);
            return SelectOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static List<T> SelectWhere<T>(this SqlConnection dbConn, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.SelectWhere<T>(conditions);
            return SelectOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static List<T> SelectWhere<T>(this SqlConnection dbConn, object conditions)
        {
            var tuple = SqlCmd.SelectWhere<T>(conditions);
            return SelectOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static List<T> SelectFmt<T>(this SqlConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static List<dynamic> SelectFmt(this SqlConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return SelectOriginal(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static List<T> SelectByIds<T>(this SqlConnection dbConn, IEnumerable idValues, string idField = OrmLite.KeyName)
        {
            var sql = SqlCmd.SelectByIds<T>(idValues, idField);
            if (sql == null) return new List<T>();
            return SelectOriginal<T>(dbConn, CommandType.Text, sql);
        }

        public static List<T> SelectPage<T>(this SqlConnection dbConn, OrmLitePageFactor factor, out int totalCount)
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

            var ps = factor.Params is Dictionary<string, object> ?
                ORM.DictionaryToParams(factor.Params as Dictionary<string, object>)
                : ORM.AnonTypeToParams(factor.Params);
            StringBuilder sb = new StringBuilder(200);

            sb.AppendFormat("select count(0) from [{0}]", factor.TableName);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            totalCount = CountOriginal(dbConn, CommandType.Text, sb.ToString(), ps);

            if (totalCount == 0) //总数为0了，肯定没有数据
            {
                return new List<T>();
            }
            sb.Clear();

            sb.AppendFormat("select * from (");
            sb.AppendFormat(" select top {0} {1},ROW_NUMBER() over(order by {2}) rowid from [{3}]", factor.PageIndex * factor.PageSize, factor.Fields, factor.OrderBy, factor.TableName);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            sb.AppendFormat(")t where t.rowid>{0}", (factor.PageIndex - 1) * factor.PageSize);

            var list = SelectOriginal<T>(dbConn, CommandType.Text, sb.ToString(), ps);
            return list;
        }

        public static List<dynamic> SelectPage(this SqlConnection dbConn, OrmLitePageFactor factor, out int totalCount)
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

            var ps = factor.Params is Dictionary<string, object> ?
                ORM.DictionaryToParams(factor.Params as Dictionary<string, object>)
                : ORM.AnonTypeToParams(factor.Params);
            StringBuilder sb = new StringBuilder(200);

            sb.AppendFormat("select count(0) from [{0}]", factor.TableName);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            totalCount = CountOriginal(dbConn, CommandType.Text, sb.ToString(), ps);

            if (totalCount == 0) //总数为0了，肯定没有数据
            {
                return new List<dynamic>();
            }
            sb.Clear();

            sb.AppendFormat("select * from (");
            sb.AppendFormat(" select top {0} {1},ROW_NUMBER() over(order by {2}) rowid from [{3}]", factor.PageIndex * factor.PageSize, factor.Fields, factor.OrderBy, factor.TableName);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            sb.AppendFormat(")t where t.rowid>{0}", (factor.PageIndex - 1) * factor.PageSize);

            var list = SelectOriginal(dbConn, CommandType.Text, sb.ToString(), ps);
            return list;
        }
        #endregion

        #region Single

        public static T Single<T>(this SqlConnection dbConn, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.Single<T>(conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T Single<T>(this SqlConnection dbConn, object conditions)
        {
            var tuple = SqlCmd.Single<T>(conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T Single<T>(this SqlConnection dbConn, string sql)
        {
            return SingleOriginal<T>(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Single), null);
        }

        public static T Single<T>(this SqlConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return SingleOriginal<T>(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Single), ORM.DictionaryToParams(parameters));
        }

        public static dynamic Single(this SqlConnection dbConn, string sql)
        {
            return SingleOriginal(dbConn, CommandType.Text, sql, null);
        }

        public static dynamic Single(this SqlConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return SingleOriginal(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }

        public static dynamic Single(this SqlConnection dbConn, string sql, object parameters)
        {
            return SingleOriginal(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static T SingleFmt<T>(this SqlConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return SingleOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static dynamic SingleFmt(this SqlConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return SingleOriginal(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static T SingleById<T>(this SqlConnection dbConn, object idValue, string idField = OrmLite.KeyName)
        {
            var tuple = SqlCmd.SingleById<T>(idValue, idField);
            return SingleOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T SingleWhere<T>(this SqlConnection dbConn, string name, object value)
        {
            var tuple = SqlCmd.SingleWhere<T>(name, value);
            return SingleOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T SingleWhere<T>(this SqlConnection dbConn, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.SingleWhere<T>(conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static T SingleWhere<T>(this SqlConnection dbConn, object conditions)
        {
            var tuple = SqlCmd.SingleWhere<T>(conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        #endregion

        #region Scalar

        public static T Scalar<T>(this SqlConnection dbConn, string sql)
        {
            return ScalarOriginal<T>(dbConn, CommandType.Text, sql, null);
        }

        public static T Scalar<T>(this SqlConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return ScalarOriginal<T>(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }

        public static T Scalar<T>(this SqlConnection dbConn, string sql, object parameters)
        {
            return ScalarOriginal<T>(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static T ScalarFmt<T>(this SqlConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return ScalarOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        #endregion

        #region Column

        public static List<T> Column<T>(this SqlConnection dbConn, string sql)
        {
            return ColumnOriginal<T>(dbConn, CommandType.Text, sql, null);
        }

        public static List<T> Column<T>(this SqlConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return ColumnOriginal<T>(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }

        public static List<T> Column<T>(this SqlConnection dbConn, string sql, object parameters)
        {
            return ColumnOriginal<T>(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static List<T> ColumnFmt<T>(this SqlConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return ColumnOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this SqlConnection dbConn, string sql)
        {
            return ColumnDistinctOriginal<T>(dbConn, CommandType.Text, sql);
        }

        public static HashSet<T> ColumnDistinct<T>(this SqlConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return ColumnDistinctOriginal<T>(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this SqlConnection dbConn, string sql, object parameters)
        {
            return ColumnDistinctOriginal<T>(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static HashSet<T> ColumnDistinctFmt<T>(this SqlConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return ColumnDistinctOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        #endregion

        #region Lookup Dictionary
        public static Dictionary<K, List<V>> Lookup<K, V>(this SqlConnection dbConn, string sql)
        {
            return LookupOriginal<K, V>(dbConn, CommandType.Text, sql, null);
        }
        public static Dictionary<K, List<V>> Lookup<K, V>(this SqlConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return LookupOriginal<K, V>(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }
        public static Dictionary<K, List<V>> Lookup<K, V>(this SqlConnection dbConn, string sql, object parameters)
        {
            return LookupOriginal<K, V>(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static Dictionary<K, List<V>> LookupFmt<K, V>(this SqlConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return LookupOriginal<K, V>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static Dictionary<K, V> Dictionary<K, V>(this SqlConnection dbConn, string sql)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, sql, null);
        }
        public static Dictionary<K, V> Dictionary<K, V>(this SqlConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }
        public static Dictionary<K, V> Dictionary<K, V>(this SqlConnection dbConn, string sql, object parameters)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static Dictionary<K, V> DictionaryFmt<K, V>(this SqlConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

        #region Count
        public static int Count<T>(this SqlConnection dbConn)
        {
            return CountOriginal(dbConn, CommandType.Text, SqlCmd.Count<T>());
        }

        public static int Count<T>(this SqlConnection dbConn, string sql)
        {
            return CountOriginal(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Count), null);
        }
        public static int Count<T>(this SqlConnection dbConn, string sql, Dictionary<string, object> parameters)
        {
            return CountOriginal(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Count), ORM.DictionaryToParams(parameters));
        }
        public static int Count<T>(this SqlConnection dbConn, string sql, object parameters)
        {
            return CountOriginal(dbConn, CommandType.Text, SqlCmd.FullPartSql<T>(sql, PartSqlType.Count), ORM.AnonTypeToParams(parameters));
        }

        public static int CountWhere<T>(this SqlConnection dbConn, string name, object value)
        {
            var tuple = SqlCmd.CountWhere<T>(name, value);
            return CountOriginal(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static int CountWhere<T>(this SqlConnection dbConn, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.CountWhere<T>(conditions);
            return CountOriginal(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static int CountWhere<T>(this SqlConnection dbConn, object conditions)
        {
            var tuple = SqlCmd.CountWhere<T>(conditions);
            return CountOriginal(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }


        public static int CountFmt(this SqlConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return CountOriginal(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

    }
}
