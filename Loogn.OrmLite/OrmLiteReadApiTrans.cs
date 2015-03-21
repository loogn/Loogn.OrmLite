using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//All Copy OrmLiteReaderApi.cs

//SqlConnection---->SqlTransaction
//dbConn ---->  dbTrans
namespace Loogn.OrmLite
{
    public static partial class OrmLiteReadApi
    {
        #region Original Function

        public static List<T> SelectOriginal<T>(this SqlTransaction dbTrans, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToObjectList<T>(reader);
            }
        }

        public static List<dynamic> SelectOriginal(this SqlTransaction dbTrans, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToDynamicList(reader);
            }
        }

        public static T SingleOriginal<T>(this SqlTransaction dbTrans, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToObject<T>(reader);
            }
        }

        public static dynamic SingleOriginal(this SqlTransaction dbTrans, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToDynamic(reader);
            }
        }

        public static T ScalarOriginal<T>(this SqlTransaction dbTrans, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            var obj = SqlHelper.ExecuteScalar(dbTrans, commandType, commandText, ps);
            return (T)obj;
        }

        public static List<T> ColumnOriginal<T>(this SqlTransaction dbTrans, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToColumnList<T>(reader);
            }
        }

        public static HashSet<T> ColumnDistinctOriginal<T>(this SqlTransaction dbTrans, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToColumnSet<T>(reader);
            }
        }

        public static Dictionary<K, List<V>> LookupOriginal<K, V>(this SqlTransaction dbTrans, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToLookup<K, V>(reader);
            }
        }

        public static Dictionary<K, V> DictionaryOriginal<K, V>(this SqlTransaction dbTrans, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return ORM.ReaderToDictionary<K, V>(reader);
            }
        }

        public static long CountOriginal(this SqlTransaction dbTrans, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            var obj = SqlHelper.ExecuteScalar(dbTrans, commandType, commandText, ps);
            return Convert.ToInt64(obj);
        }

        #endregion

        #region Select
        public static List<T> Select<T>(this SqlTransaction dbTrans)
        {
            var table = typeof(T).GetCachedTableName();
            return SelectOriginal<T>(dbTrans, CommandType.Text, "SELECT * FROM [" + table + "]");
        }

        public static List<T> Select<T>(this SqlTransaction dbTrans, string sql)
        {
            return SelectOriginal<T>(dbTrans, CommandType.Text, ORM.FullPartSql<T>(sql, PartSqlType.Select), null);
        }

        public static List<T> Select<T>(this SqlTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return SelectOriginal<T>(dbTrans, CommandType.Text, ORM.FullPartSql<T>(sql, PartSqlType.Select), ORM.DictionaryToParams(parameters));
        }

        public static List<T> Select<T>(this SqlTransaction dbTrans, string sql, object parameters)
        {
            return SelectOriginal<T>(dbTrans, CommandType.Text, ORM.FullPartSql<T>(sql, PartSqlType.Select), ORM.AnonTypeToParams(parameters));
        }

        public static List<dynamic> Select(this SqlTransaction dbTrans, string sql)
        {
            return SelectOriginal(dbTrans, CommandType.Text, sql, null);
        }

        public static List<dynamic> Select(this SqlTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return SelectOriginal(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }

        public static List<dynamic> Select(this SqlTransaction dbTrans, string sql, object parameters)
        {
            return SelectOriginal(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static List<T> SelectWhere<T>(this SqlTransaction dbTrans, string name, object value)
        {
            var table = typeof(T).GetCachedTableName();
            SqlParameter p = new SqlParameter("@" + name, value);
            return SelectOriginal<T>(dbTrans, CommandType.Text, string.Format("Select * from [{0}] where [{1}]=@{1}", table, name), p);
        }

        public static List<T> SelectWhere<T>(this SqlTransaction dbTrans, Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT * FROM [{0}]", tableName);
            var ps = ORM.DictionaryToParams(conditions, sqlbuilder);
            return SelectOriginal<T>(dbTrans, CommandType.Text, sqlbuilder.ToString(), ps);
        }

        public static List<T> SelectWhere<T>(this SqlTransaction dbTrans, object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT * FROM [{0}]", tableName);
            var ps = ORM.AnonTypeToParams(conditions, sqlbuilder);
            return SelectOriginal<T>(dbTrans, CommandType.Text, sqlbuilder.ToString(), ps);
        }

        public static List<T> SelectFmt<T>(this SqlTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return SelectOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static List<dynamic> SelectFmt(this SqlTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return SelectOriginal(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static List<T> SelectByIds<T>(this SqlTransaction dbTrans, IEnumerable idValues, string idField = "ID")
        {
            if (idValues == null) return new List<T>();
            bool any = false;
            var needQuot = false;
            StringBuilder sql = null;
            var enumerator = idValues.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!any)
                {
                    any = true;
                    var idType = enumerator.Current.GetType();
                    if (idType == typeof(string) || idType == typeof(DateTime))
                    {
                        needQuot = true;
                    }
                    var table = typeof(T).GetCachedTableName();
                    sql = new StringBuilder(50);
                    sql.AppendFormat("Select * from [{0}] where [{1}] in (", table, idField);
                }
                if (needQuot)
                {
                    sql.AppendFormat("'{0}',", enumerator.Current);
                }
                else
                {
                    sql.AppendFormat("{0},", enumerator.Current);
                }
            }
            if (!any)
            {
                return new List<T>();
            }
            else
            {
                sql.Replace(',', ')', sql.Length - 1, 1);
                return SelectOriginal<T>(dbTrans, CommandType.Text, sql.ToString());
            }
        }

        public static List<T> SelectPage<T>(this SqlTransaction dbTrans, OrmLitePageFactor factor, out long totalCount)
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

            var ps = ORM.AnonTypeToParams(factor.Params);
            StringBuilder sb = new StringBuilder(200);

            sb.AppendFormat("select count(0) from [{0}]", factor.TableName);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            totalCount = CountOriginal(dbTrans, CommandType.Text, sb.ToString(), ps);

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

            var list = SelectOriginal<T>(dbTrans, CommandType.Text, sb.ToString(), ps);
            return list;
        }

        #endregion

        #region Single

        public static T Single<T>(this SqlTransaction dbTrans, Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT  TOP 1 * FROM [{0}]", tableName);
            var ps = ORM.DictionaryToParams(conditions, sqlbuilder);
            return SingleOriginal<T>(dbTrans, CommandType.Text, sqlbuilder.ToString(), ps);
        }

        public static T Single<T>(this SqlTransaction dbTrans, object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT  TOP 1 * FROM [{0}]", tableName);
            var ps = ORM.AnonTypeToParams(conditions, sqlbuilder);
            return SingleOriginal<T>(dbTrans, CommandType.Text, sqlbuilder.ToString(), ps);
        }

        public static T Single<T>(this SqlTransaction dbTrans, string sql)
        {
            return SingleOriginal<T>(dbTrans, CommandType.Text, ORM.FullPartSql<T>(sql, PartSqlType.Single), null);
        }

        public static T Single<T>(this SqlTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return SingleOriginal<T>(dbTrans, CommandType.Text, ORM.FullPartSql<T>(sql, PartSqlType.Single), ORM.DictionaryToParams(parameters));
        }

        public static dynamic Single(this SqlTransaction dbTrans, string sql)
        {
            return SingleOriginal(dbTrans, CommandType.Text, sql, null);
        }

        public static dynamic Single(this SqlTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return SingleOriginal(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }

        public static dynamic Single(this SqlTransaction dbTrans, string sql, object parameters)
        {
            return SingleOriginal(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static T SingleFmt<T>(this SqlTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return SingleOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static dynamic SingleFmt(this SqlTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return SingleOriginal(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static T SingleById<T>(this SqlTransaction dbTrans, object idValue, string idField = "ID")
        {
            SqlParameter sp = new SqlParameter("@" + idField, idValue);
            return SingleOriginal<T>(dbTrans, CommandType.Text, string.Format("SELECT TOP 1 * FROM [{0}] WHERE [{1}]=@{1}", typeof(T).GetCachedTableName(), idField), sp);
        }

        public static T SingleWhere<T>(this SqlTransaction dbTrans, string name, object value)
        {
            var table = typeof(T).GetCachedTableName();
            SqlParameter p = new SqlParameter("@" + name, value);
            return SingleOriginal<T>(dbTrans, CommandType.Text, string.Format("SELECT TOP 1 * FROM [{0}] WHERE [{1}]=@{1}", table, name), p);
        }

        public static T SingleWhere<T>(this SqlTransaction dbTrans, Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT TOP 1 * FROM [{0}]", tableName);
            var ps = ORM.DictionaryToParams(conditions, sqlbuilder);
            return SingleOriginal<T>(dbTrans, CommandType.Text, sqlbuilder.ToString(), ps);
        }

        public static T SingleWhere<T>(this SqlTransaction dbTrans, object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT TOP 1 * FROM [{0}]", tableName);
            var ps = ORM.AnonTypeToParams(conditions, sqlbuilder);
            return SingleOriginal<T>(dbTrans, CommandType.Text, sqlbuilder.ToString(), ps);
        }

        #endregion

        #region Scalar

        public static T Scalar<T>(this SqlTransaction dbTrans, string sql)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql, null);
        }

        public static T Scalar<T>(this SqlTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }

        public static T Scalar<T>(this SqlTransaction dbTrans, string sql, object parameters)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static T ScalarFmt<T>(this SqlTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        #endregion

        #region Column

        public static List<T> Column<T>(this SqlTransaction dbTrans, string sql)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, sql, null);
        }

        public static List<T> Column<T>(this SqlTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }

        public static List<T> Column<T>(this SqlTransaction dbTrans, string sql, object parameters)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static List<T> ColumnFmt<T>(this SqlTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this SqlTransaction dbTrans, string sql)
        {
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, sql);
        }

        public static HashSet<T> ColumnDistinct<T>(this SqlTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this SqlTransaction dbTrans, string sql, object parameters)
        {
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static HashSet<T> ColumnDistinctFmt<T>(this SqlTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        #endregion

        #region Lookup Dictionary
        public static Dictionary<K, List<V>> Lookup<K, V>(this SqlTransaction dbTrans, string sql)
        {
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, sql, null);
        }
        public static Dictionary<K, List<V>> Lookup<K, V>(this SqlTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }
        public static Dictionary<K, List<V>> Lookup<K, V>(this SqlTransaction dbTrans, string sql, object parameters)
        {
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static Dictionary<K, List<V>> LookupFmt<K, V>(this SqlTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static Dictionary<K, V> Dictionary<K, V>(this SqlTransaction dbTrans, string sql)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, sql, null);
        }
        public static Dictionary<K, V> Dictionary<K, V>(this SqlTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }
        public static Dictionary<K, V> Dictionary<K, V>(this SqlTransaction dbTrans, string sql, object parameters)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, sql, ORM.AnonTypeToParams(parameters));
        }

        public static Dictionary<K, V> DictionaryFmt<K, V>(this SqlTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

        #region Count
        public static long Count<T>(this SqlTransaction dbTrans)
        {
            var table = typeof(T).GetCachedTableName();
            return CountOriginal(dbTrans, CommandType.Text, "SELECT COUNT(0) FROM [" + table + "]");
        }

        public static long Count<T>(this SqlTransaction dbTrans, string sql)
        {
            return CountOriginal(dbTrans, CommandType.Text, ORM.FullPartSql<T>(sql, PartSqlType.Count), null);
        }
        public static long Count<T>(this SqlTransaction dbTrans, string sql, Dictionary<string, object> parameters)
        {
            return CountOriginal(dbTrans, CommandType.Text, ORM.FullPartSql<T>(sql, PartSqlType.Count), ORM.DictionaryToParams(parameters));
        }
        public static long Count<T>(this SqlTransaction dbTrans, string sql, object parameters)
        {
            return CountOriginal(dbTrans, CommandType.Text, ORM.FullPartSql<T>(sql, PartSqlType.Count), ORM.AnonTypeToParams(parameters));
        }

        public static long CountWhere<T>(this SqlTransaction dbTrans, string name, object value)
        {
            var table = typeof(T).GetCachedTableName();
            SqlParameter p = new SqlParameter("@" + name, value);
            return CountOriginal(dbTrans, CommandType.Text, string.Format("SELECT COUNT(0) FROM [{0}] WHERE [{1}]=@{1}", table, name), p);
        }

        public static long CountWhere<T>(this SqlTransaction dbTrans, Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT COUNT(0) FROM [{0}]", tableName);
            var ps = ORM.DictionaryToParams(conditions, sqlbuilder);
            return CountOriginal(dbTrans, CommandType.Text, sqlbuilder.ToString(), ps);
        }

        public static long CountWhere<T>(this SqlTransaction dbTrans, object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT COUNT(0) FROM [{0}]", tableName);
            var ps = ORM.AnonTypeToParams(conditions, sqlbuilder);
            return CountOriginal(dbTrans, CommandType.Text, sqlbuilder.ToString(), ps);
        }


        public static long CountFmt(this SqlTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return CountOriginal(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

    }
}
