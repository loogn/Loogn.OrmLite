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
        public static IDataReader ExecuteReader(this IDbTransaction dbTrans, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            return SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps);
        }

        public static List<T> SelectOriginal<T>(this IDbTransaction dbTrans, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToObjectList<T>(reader);
            }
        }

        public static List<dynamic> SelectOriginal(this IDbTransaction dbTrans, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToDynamicList(reader);
            }
        }

        public static T SingleOriginal<T>(this IDbTransaction dbTrans, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToObject<T>(reader);
            }
        }

        public static dynamic SingleOriginal(this IDbTransaction dbTrans, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToDynamic(reader);
            }
        }

        public static T ScalarOriginal<T>(this IDbTransaction dbTrans, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            var obj = SqlHelper.ExecuteScalar(dbTrans, commandType, commandText, ps);
            return TransformForDataReader.ConvertToPrimitiveType<T>(obj);
        }

        public static List<T> ColumnOriginal<T>(this IDbTransaction dbTrans, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToColumnList<T>(reader);
            }
        }

        public static HashSet<T> ColumnDistinctOriginal<T>(this IDbTransaction dbTrans, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToColumnSet<T>(reader);
            }
        }

        public static Dictionary<K, List<V>> LookupOriginal<K, V>(this IDbTransaction dbTrans, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToLookup<K, V>(reader);
            }
        }

        public static Dictionary<K, V> DictionaryOriginal<K, V>(this IDbTransaction dbTrans, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToDictionary<K, V>(reader);
            }
        }

        public static int CountOriginal(this IDbTransaction dbTrans, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            var obj = SqlHelper.ExecuteScalar(dbTrans, commandType, commandText, ps);
            return TransformForDataReader.ConvertToPrimitiveType<int>(obj);
        }

        #endregion

        #region Select
        public static List<T> Select<T>(this IDbTransaction dbTrans)
        {
            var cmd = dbTrans.GetCommandDialectProvider().Select<T>();
            return SelectOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static List<T> Select<T>(this IDbTransaction dbTrans, string sql)
        {
            var cmd = dbTrans.GetCommandDialectProvider().FullSelect<T>(sql);
            return SelectOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static List<T> Select<T>(this IDbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.FullSelect<T>(sql);
            return SelectOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, provider.Dictionary2Params(parameters));
        }

        public static List<T> Select<T>(this IDbTransaction dbTrans, string sql, object parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.FullSelect<T>(sql);
            return SelectOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, provider.Object2Params(parameters));
        }

        public static List<dynamic> Select(this IDbTransaction dbTrans, string sql)
        {
            return SelectOriginal(dbTrans, CommandType.Text, sql, null);
        }

        public static List<dynamic> Select(this IDbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return SelectOriginal(dbTrans, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }

        public static List<dynamic> Select(this IDbTransaction dbTrans, string sql, object parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return SelectOriginal(dbTrans, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        public static List<T> SelectWhere<T>(this IDbTransaction dbTrans, string name, object value)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.SelectWhere<T>(name, value);
            return SelectOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static List<T> SelectWhere<T>(this IDbTransaction dbTrans, IDictionary<string, object> conditions)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.SelectWhere<T>(conditions);
            return SelectOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static List<T> SelectWhere<T>(this IDbTransaction dbTrans, object conditions)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.SelectWhere<T>(conditions);
            return SelectOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static List<T> SelectFmt<T>(this IDbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return SelectOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static List<dynamic> SelectFmt(this IDbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return SelectOriginal(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static List<T> SelectByIds<T>(this IDbTransaction dbTrans, IEnumerable idValues, string idField = OrmLite.KeyName, string selectFields = "*")
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.SelectByIds<T>(idValues, idField, selectFields);
            if (string.IsNullOrEmpty(cmd.CommandText)) return new List<T>();
            return SelectOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static List<T> SelectPage<T>(this IDbTransaction dbTrans, OrmLitePageFactor factor, out int totalCount)
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
                factor.TableName = TypeCachedDict.GetTypeCachedInfo(typeof(T)).TableName;
            }
            if (string.IsNullOrEmpty(factor.Fields))
            {
                factor.Fields = "*";
            }

            var provider = dbTrans.GetCommandDialectProvider();

            var ps = factor.Params is IDictionary<string, object> ?
                provider.Dictionary2Params(factor.Params as IDictionary<string, object>) :
                provider.Object2Params(factor.Params);
            StringBuilder sb = new StringBuilder(200);

            var l = provider.OpenQuote;
            var r = provider.CloseQuote;

            if (factor.TableName.IndexOf(" ") > 0)
            {
                l = "";
                r = "";
            }

            sb.AppendFormat("select count(0) from {1}{0}{2}", factor.TableName, l, r);

            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            totalCount = CountOriginal(dbTrans, CommandType.Text, sb.ToString(), ps);

            if (totalCount == 0) //总数为0了，肯定没有数据
            {
                return new List<T>();
            }
            var cmd = provider.Paged(factor);
            var list = SelectOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, ps);
            return list;
        }

        public static List<dynamic> SelectPage(this IDbTransaction dbTrans, OrmLitePageFactor factor, out int totalCount)
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

            var provider = dbTrans.GetCommandDialectProvider();

            var ps = factor.Params is IDictionary<string, object> ?
                provider.Dictionary2Params(factor.Params as IDictionary<string, object>)
                : provider.Object2Params(factor.Params);
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
            var cmd = provider.Paged(factor);
            var list = SelectOriginal(dbTrans, cmd.CommandType, cmd.CommandText, ps);
            return list;
        }

        public static OrmLitePageResult<T> SelectPage<T>(this IDbTransaction dbTrans, OrmLitePageFactor factor)
        {
            int totalCount;
            OrmLitePageResult<T> pageInfo = new OrmLitePageResult<T>();
            pageInfo.List = dbTrans.SelectPage<T>(factor, out totalCount);
            pageInfo.PageIndex = factor.PageIndex;
            pageInfo.PageSize = factor.PageSize;
            pageInfo.TotalCount = totalCount;
            return pageInfo;
        }

        public static OrmLitePageResult<dynamic> SelectPage(this IDbTransaction dbTrans, OrmLitePageFactor factor)
        {
            int totalCount;
            OrmLitePageResult<dynamic> pageInfo = new OrmLitePageResult<dynamic>();
            pageInfo.List = dbTrans.SelectPage(factor, out totalCount);
            pageInfo.PageIndex = factor.PageIndex;
            pageInfo.PageSize = factor.PageSize;
            pageInfo.TotalCount = totalCount;
            return pageInfo;
        }

        #endregion

        #region Single

        public static T Single<T>(this IDbTransaction dbTrans, string sql)
        {
            var cmd = dbTrans.GetCommandDialectProvider().FullSingle<T>(sql);
            return SingleOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static T Single<T>(this IDbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = dbTrans.GetCommandDialectProvider().FullSingle<T>(sql);
            return SingleOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, provider.Dictionary2Params(parameters));
        }

        public static dynamic Single(this IDbTransaction dbTrans, string sql)
        {
            return SingleOriginal(dbTrans, CommandType.Text, sql, null);
        }

        public static dynamic Single(this IDbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return SingleOriginal(dbTrans, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }

        public static dynamic Single(this IDbTransaction dbTrans, string sql, object parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return SingleOriginal(dbTrans, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        public static T SingleFmt<T>(this IDbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return SingleOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static dynamic SingleFmt(this IDbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return SingleOriginal(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static T SingleById<T>(this IDbTransaction dbTrans, object idValue, string idField = OrmLite.KeyName)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.SingleById<T>(idValue, idField);
            return SingleOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static T SingleWhere<T>(this IDbTransaction dbTrans, string name, object value)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.SingleWhere<T>(name, value);
            return SingleOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static T SingleWhere<T>(this IDbTransaction dbTrans, IDictionary<string, object> conditions)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.SingleWhere<T>(conditions);
            return SingleOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static T SingleWhere<T>(this IDbTransaction dbTrans, object conditions)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.SingleWhere<T>(conditions);
            return SingleOriginal<T>(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        #endregion

        #region Scalar

        public static T Scalar<T>(this IDbTransaction dbTrans, string sql)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql, null);
        }

        public static T Scalar<T>(this IDbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }

        public static T Scalar<T>(this IDbTransaction dbTrans, string sql, object parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        public static T ScalarFmt<T>(this IDbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static T MaxID<T>(this IDbTransaction dbTrans, string tableName, string field = "id")
        {
            tableName = SqlInjection.Filter(tableName);
            var provider = dbTrans.GetCommandDialectProvider();

            var sql = string.Format("SELECT {2}(MAX({3}{0}{4}), 0) FROM {3}{1}{4}", field, tableName, provider.IsNullFunc, provider.OpenQuote, provider.CloseQuote);
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql);
        }
        #endregion

        #region Column

        public static List<T> Column<T>(this IDbTransaction dbTrans, string sql)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, sql, null);
        }

        public static List<T> Column<T>(this IDbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return ColumnOriginal<T>(dbTrans, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }

        public static List<T> Column<T>(this IDbTransaction dbTrans, string sql, object parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return ColumnOriginal<T>(dbTrans, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        public static List<T> ColumnFmt<T>(this IDbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this IDbTransaction dbTrans, string sql)
        {
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, sql);
        }

        public static HashSet<T> ColumnDistinct<T>(this IDbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this IDbTransaction dbTrans, string sql, object parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        public static HashSet<T> ColumnDistinctFmt<T>(this IDbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        #endregion

        #region Lookup Dictionary
        public static Dictionary<K, List<V>> Lookup<K, V>(this IDbTransaction dbTrans, string sql)
        {
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, sql, null);
        }
        public static Dictionary<K, List<V>> Lookup<K, V>(this IDbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }
        public static Dictionary<K, List<V>> Lookup<K, V>(this IDbTransaction dbTrans, string sql, object parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        public static Dictionary<K, List<V>> LookupFmt<K, V>(this IDbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static Dictionary<K, V> Dictionary<K, V>(this IDbTransaction dbTrans, string sql)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, sql, null);
        }
        public static Dictionary<K, V> Dictionary<K, V>(this IDbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }
        public static Dictionary<K, V> Dictionary<K, V>(this IDbTransaction dbTrans, string sql, object parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        public static Dictionary<K, V> DictionaryFmt<K, V>(this IDbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

        #region Count
        public static int Count<T>(this IDbTransaction dbTrans)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Count<T>();
            return CountOriginal(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static int Count<T>(this IDbTransaction dbTrans, string sql)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.FullCount<T>(sql);
            return CountOriginal(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }
        public static int Count<T>(this IDbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.FullCount<T>(sql);
            return CountOriginal(dbTrans, cmd.CommandType, cmd.CommandText, provider.Dictionary2Params(parameters));
        }
        public static int Count<T>(this IDbTransaction dbTrans, string sql, object parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.FullCount<T>(sql);
            return CountOriginal(dbTrans, cmd.CommandType, cmd.CommandText, provider.Object2Params(parameters));
        }

        public static int CountWhere<T>(this IDbTransaction dbTrans, string name, object value)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.CountWhere<T>(name, value);

            return CountOriginal(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static int CountWhere<T>(this IDbTransaction dbTrans, IDictionary<string, object> conditions)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.CountWhere<T>(conditions);
            return CountOriginal(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static int CountWhere<T>(this IDbTransaction dbTrans, object conditions)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.CountWhere<T>(conditions);
            return CountOriginal(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }


        public static int CountFmt(this IDbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return CountOriginal(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

    }
}
