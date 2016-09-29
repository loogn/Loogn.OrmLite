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
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return Mapping.ReaderToObjectList<T>(reader);
            }
        }

        public static List<dynamic> SelectOriginal(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return Mapping.ReaderToDynamicList(reader);
            }
        }

        public static T SingleOriginal<T>(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return Mapping.ReaderToObject<T>(reader);
            }
        }

        public static dynamic SingleOriginal(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return Mapping.ReaderToDynamic(reader);
            }
        }

        public static T ScalarOriginal<T>(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            var obj = SqlHelper.ExecuteScalar(dbTrans, commandType, commandText, ps);
            return Mapping.ConvertToPrimitiveType<T>(obj);
        }

        public static List<T> ColumnOriginal<T>(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return Mapping.ReaderToColumnList<T>(reader);
            }
        }

        public static HashSet<T> ColumnDistinctOriginal<T>(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return Mapping.ReaderToColumnSet<T>(reader);
            }
        }

        public static Dictionary<K, List<V>> LookupOriginal<K, V>(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return Mapping.ReaderToLookup<K, V>(reader);
            }
        }

        public static Dictionary<K, V> DictionaryOriginal<K, V>(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbTrans, commandType, commandText, ps))
            {
                return Mapping.ReaderToDictionary<K, V>(reader);
            }
        }

        public static int CountOriginal(this DbTransaction dbTrans, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            var obj = SqlHelper.ExecuteScalar(dbTrans, commandType, commandText, ps);
            return Mapping.ConvertToPrimitiveType<int>(obj);
        }

        #endregion

        #region Select
        public static List<T> Select<T>(this DbTransaction dbTrans)
        {
            return SelectOriginal<T>(dbTrans, CommandType.Text, BaseCmd.GetCmd(dbTrans.GetProviderType()).Select<T>());
        }

        public static List<T> Select<T>(this DbTransaction dbTrans, string sql)
        {
            return SelectOriginal<T>(dbTrans, CommandType.Text, BaseCmd.GetCmd(dbTrans.GetProviderType()).FullPartSql<T>(sql, PartSqlType.Select), null);
        }

        public static List<T> Select<T>(this DbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbTrans.GetProviderType());
            return SelectOriginal<T>(dbTrans, CommandType.Text, theCmd.FullPartSql<T>(sql, PartSqlType.Select), theCmd.DictionaryToParams(parameters));
        }

        public static List<T> Select<T>(this DbTransaction dbTrans, string sql, object parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbTrans.GetProviderType());
            return SelectOriginal<T>(dbTrans, CommandType.Text, theCmd.FullPartSqlSingle<T>(sql), theCmd.AnonTypeToParams(parameters));
        }

        public static List<dynamic> Select(this DbTransaction dbTrans, string sql)
        {
            return SelectOriginal(dbTrans, CommandType.Text, sql, null);
        }

        public static List<dynamic> Select(this DbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbTrans.GetProviderType());
            return SelectOriginal(dbTrans, CommandType.Text, sql, theCmd.DictionaryToParams(parameters));
        }

        public static List<dynamic> Select(this DbTransaction dbTrans, string sql, object parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbTrans.GetProviderType());
            return SelectOriginal(dbTrans, CommandType.Text, sql, theCmd.AnonTypeToParams(parameters));
        }

        public static List<T> SelectWhere<T>(this DbTransaction dbTrans, string name, object value)
        {
            var cmd = BaseCmd.GetCmd(dbTrans.GetProviderType()).SelectWhere<T>(name, value);
            return SelectOriginal<T>(dbTrans, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static List<T> SelectWhere<T>(this DbTransaction dbTrans, IDictionary<string, object> conditions)
        {
            var cmd = BaseCmd.GetCmd(dbTrans.GetProviderType()).SelectWhere<T>(conditions);
            return SelectOriginal<T>(dbTrans, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static List<T> SelectWhere<T>(this DbTransaction dbTrans, object conditions)
        {
            var cmd = BaseCmd.GetCmd(dbTrans.GetProviderType()).SelectWhere<T>(conditions);
            return SelectOriginal<T>(dbTrans, CommandType.Text, cmd.CmdText, cmd.Params);
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
            var sql = BaseCmd.GetCmd(dbTrans.GetProviderType()).SelectByIds<T>(idValues, idField, selectFields);
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
                factor.TableName = ReflectionHelper.GetInfo<T>().TableName;
            }
            if (string.IsNullOrEmpty(factor.Fields))
            {
                factor.Fields = "*";
            }
            var theCmd = BaseCmd.GetCmd(dbTrans.GetProviderType());

            var l = theCmd.L();
            var r = theCmd.R();

            var ps = factor.Params is IDictionary<string, object> ?
                theCmd.DictionaryToParams(factor.Params as IDictionary<string, object>)
                : theCmd.AnonTypeToParams(factor.Params);
            StringBuilder sb = new StringBuilder(200);

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
            var sql = theCmd.PageSql(factor);
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

            var theCmd = BaseCmd.GetCmd(dbTrans.GetProviderType());

            var l = theCmd.L();
            var r = theCmd.R();

            var ps = factor.Params is IDictionary<string, object> ?
                theCmd.DictionaryToParams(factor.Params as IDictionary<string, object>)
                : theCmd.AnonTypeToParams(factor.Params);
            StringBuilder sb = new StringBuilder(200);

            sb.AppendFormat("select count(0) from {1}{0}{2}", factor.TableName, l, r);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            totalCount = CountOriginal(dbTrans, CommandType.Text, sb.ToString(), ps);

            if (totalCount == 0) //总数为0了，肯定没有数据
            {
                return new List<dynamic>();
            }

            var sql = theCmd.PageSql(factor);
            var list = SelectOriginal(dbTrans, CommandType.Text, sql, ps);
            return list;
        }

        public static OrmLitePageResult<T> SelectPage<T>(this DbTransaction dbTrans, OrmLitePageFactor factor)
        {
            int totalCount;
            OrmLitePageResult<T> pageInfo = new OrmLitePageResult<T>();
            pageInfo.List = dbTrans.SelectPage<T>(factor, out totalCount);
            pageInfo.PageIndex = factor.PageIndex;
            pageInfo.PageSize = factor.PageSize;
            pageInfo.TotalCount = totalCount;
            return pageInfo;
        }

        public static OrmLitePageResult<dynamic> SelectPage(this DbTransaction dbTrans, OrmLitePageFactor factor)
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

        public static T Single<T>(this DbTransaction dbTrans, IDictionary<string, object> conditions)
        {
            var cmd = BaseCmd.GetCmd(dbTrans.GetProviderType()).Single<T>(conditions);
            return SingleOriginal<T>(dbTrans, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static T Single<T>(this DbTransaction dbTrans, object conditions)
        {
            var cmd = BaseCmd.GetCmd(dbTrans.GetProviderType()).Single<T>(conditions);
            return SingleOriginal<T>(dbTrans, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static T Single<T>(this DbTransaction dbTrans, string sql)
        {
            return SingleOriginal<T>(dbTrans, CommandType.Text, BaseCmd.GetCmd(dbTrans.GetProviderType()).FullPartSqlSingle<T>(sql), null);
        }

        public static T Single<T>(this DbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbTrans.GetProviderType());
            return SingleOriginal<T>(dbTrans, CommandType.Text, theCmd.FullPartSqlSingle<T>(sql), theCmd.DictionaryToParams(parameters));
        }

        public static dynamic Single(this DbTransaction dbTrans, string sql)
        {
            return SingleOriginal(dbTrans, CommandType.Text, sql, null);
        }

        public static dynamic Single(this DbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbTrans.GetProviderType());
            return SingleOriginal(dbTrans, CommandType.Text, sql, theCmd.DictionaryToParams(parameters));
        }

        public static dynamic Single(this DbTransaction dbTrans, string sql, object parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbTrans.GetProviderType());
            return SingleOriginal(dbTrans, CommandType.Text, sql, theCmd.AnonTypeToParams(parameters));
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
            var cmd = BaseCmd.GetCmd(dbTrans.GetProviderType()).SingleById<T>(idValue, idField);
            return SingleOriginal<T>(dbTrans, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static T SingleWhere<T>(this DbTransaction dbTrans, string name, object value)
        {
            var cmd = BaseCmd.GetCmd(dbTrans.GetProviderType()).SingleWhere<T>(name, value);
            return SingleOriginal<T>(dbTrans, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static T SingleWhere<T>(this DbTransaction dbTrans, IDictionary<string, object> conditions)
        {
            var cmd = BaseCmd.GetCmd(dbTrans.GetProviderType()).SingleWhere<T>(conditions);
            return SingleOriginal<T>(dbTrans, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static T SingleWhere<T>(this DbTransaction dbTrans, object conditions)
        {
            var cmd = BaseCmd.GetCmd(dbTrans.GetProviderType()).SingleWhere<T>(conditions);
            return SingleOriginal<T>(dbTrans, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        #endregion

        #region Scalar

        public static T Scalar<T>(this DbTransaction dbTrans, string sql)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql, null);
        }

        public static T Scalar<T>(this DbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql, BaseCmd.GetCmd(dbTrans.GetProviderType()).DictionaryToParams(parameters));
        }

        public static T Scalar<T>(this DbTransaction dbTrans, string sql, object parameters)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql, BaseCmd.GetCmd(dbTrans.GetProviderType()).AnonTypeToParams(parameters));
        }

        public static T ScalarFmt<T>(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return ScalarOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static T MaxID<T>(this DbTransaction dbTrans, string tableName, string field = "id")
        {
            tableName = SqlInjection.Filter(tableName);
            var theCmd = BaseCmd.GetCmd(dbTrans.GetProviderType());

            var sql = string.Format("SELECT {2}(MAX({3}{0}{4}), 0) FROM {3}{1}{4}", field, tableName, theCmd.IFNULL(), theCmd.L(), theCmd.R());
            return ScalarOriginal<T>(dbTrans, CommandType.Text, sql);
        }
        #endregion

        #region Column

        public static List<T> Column<T>(this DbTransaction dbTrans, string sql)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, sql, null);
        }

        public static List<T> Column<T>(this DbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, sql, BaseCmd.GetCmd(dbTrans.GetProviderType()).DictionaryToParams(parameters));
        }

        public static List<T> Column<T>(this DbTransaction dbTrans, string sql, object parameters)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, sql, BaseCmd.GetCmd(dbTrans.GetProviderType()).AnonTypeToParams(parameters));
        }

        public static List<T> ColumnFmt<T>(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return ColumnOriginal<T>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this DbTransaction dbTrans, string sql)
        {
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, sql);
        }

        public static HashSet<T> ColumnDistinct<T>(this DbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, sql, BaseCmd.GetCmd(dbTrans.GetProviderType()).DictionaryToParams(parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this DbTransaction dbTrans, string sql, object parameters)
        {
            return ColumnDistinctOriginal<T>(dbTrans, CommandType.Text, sql, BaseCmd.GetCmd(dbTrans.GetProviderType()).AnonTypeToParams(parameters));
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
        public static Dictionary<K, List<V>> Lookup<K, V>(this DbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, sql, BaseCmd.GetCmd(dbTrans.GetProviderType()).DictionaryToParams(parameters));
        }
        public static Dictionary<K, List<V>> Lookup<K, V>(this DbTransaction dbTrans, string sql, object parameters)
        {
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, sql, BaseCmd.GetCmd(dbTrans.GetProviderType()).AnonTypeToParams(parameters));
        }

        public static Dictionary<K, List<V>> LookupFmt<K, V>(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return LookupOriginal<K, V>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static Dictionary<K, V> Dictionary<K, V>(this DbTransaction dbTrans, string sql)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, sql, null);
        }
        public static Dictionary<K, V> Dictionary<K, V>(this DbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, sql, BaseCmd.GetCmd(dbTrans.GetProviderType()).DictionaryToParams(parameters));
        }
        public static Dictionary<K, V> Dictionary<K, V>(this DbTransaction dbTrans, string sql, object parameters)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, sql, BaseCmd.GetCmd(dbTrans.GetProviderType()).AnonTypeToParams(parameters));
        }

        public static Dictionary<K, V> DictionaryFmt<K, V>(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return DictionaryOriginal<K, V>(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

        #region Count
        public static int Count<T>(this DbTransaction dbTrans)
        {
            return CountOriginal(dbTrans, CommandType.Text, BaseCmd.GetCmd(dbTrans.GetProviderType()).Count<T>());
        }

        public static int Count<T>(this DbTransaction dbTrans, string sql)
        {
            return CountOriginal(dbTrans, CommandType.Text, BaseCmd.GetCmd(dbTrans.GetProviderType()).FullPartSql<T>(sql, PartSqlType.Count), null);
        }
        public static int Count<T>(this DbTransaction dbTrans, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbTrans.GetProviderType());
            return CountOriginal(dbTrans, CommandType.Text, theCmd.FullPartSql<T>(sql, PartSqlType.Count), theCmd.DictionaryToParams(parameters));
        }
        public static int Count<T>(this DbTransaction dbTrans, string sql, object parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbTrans.GetProviderType());
            return CountOriginal(dbTrans, CommandType.Text, theCmd.FullPartSql<T>(sql, PartSqlType.Count), theCmd.AnonTypeToParams(parameters));
        }

        public static int CountWhere<T>(this DbTransaction dbTrans, string name, object value)
        {
            var cmd = BaseCmd.GetCmd(dbTrans.GetProviderType()).CountWhere<T>(name, value);
            return CountOriginal(dbTrans, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static int CountWhere<T>(this DbTransaction dbTrans, IDictionary<string, object> conditions)
        {
            var cmd = BaseCmd.GetCmd(dbTrans.GetProviderType()).CountWhere<T>(conditions);
            return CountOriginal(dbTrans, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static int CountWhere<T>(this DbTransaction dbTrans, object conditions)
        {
            var cmd = BaseCmd.GetCmd(dbTrans.GetProviderType()).CountWhere<T>(conditions);
            return CountOriginal(dbTrans, CommandType.Text, cmd.CmdText, cmd.Params);
        }


        public static int CountFmt(this DbTransaction dbTrans, string sqlFormat, params object[] parameters)
        {
            return CountOriginal(dbTrans, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

    }
}
