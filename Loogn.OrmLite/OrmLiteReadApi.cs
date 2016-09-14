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

            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToObjectList<T>(reader);
            }
        }

        public static List<dynamic> SelectOriginal(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {

            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToDynamicList(reader);
            }
        }

        public static T SingleOriginal<T>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {

            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToObject<T>(reader);
            }
        }

        public static dynamic SingleOriginal(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {

            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToDynamic(reader);
            }
        }

        public static T ScalarOriginal<T>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {

            var obj = SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
            return Mapping.ConvertToType<T>(obj);
        }

        public static List<T> ColumnOriginal<T>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {

            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToColumnList<T>(reader);
            }
        }

        public static HashSet<T> ColumnDistinctOriginal<T>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {

            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToColumnSet<T>(reader);
            }
        }

        public static Dictionary<K, List<V>> LookupOriginal<K, V>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {

            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToLookup<K, V>(reader);
            }
        }

        public static Dictionary<K, V> DictionaryOriginal<K, V>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {

            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToDictionary<K, V>(reader);
            }
        }

        public static int CountOriginal(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            var obj = SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
            return Mapping.ConvertToType<int>(obj);
        }

        #endregion

        #region Select
        public static List<T> Select<T>(this DbConnection dbConn)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, BaseCmd.GetCmd(dbConn.GetProviderType()).Select<T>());
        }

        public static List<T> Select<T>(this DbConnection dbConn, string sql)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, BaseCmd.GetCmd(dbConn.GetProviderType()).FullPartSql<T>(sql, PartSqlType.Select), null);
        }

        public static List<T> Select<T>(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SelectOriginal<T>(dbConn, CommandType.Text, theCmd.FullPartSql<T>(sql, PartSqlType.Select), theCmd.DictionaryToParams(parameters));
        }

        public static List<T> Select<T>(this DbConnection dbConn, string sql, object parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SelectOriginal<T>(dbConn, CommandType.Text, theCmd.FullPartSql<T>(sql, PartSqlType.Select), theCmd.AnonTypeToParams(parameters));
        }

        public static List<dynamic> Select(this DbConnection dbConn, string sql)
        {
            return SelectOriginal(dbConn, CommandType.Text, sql, null);
        }

        public static List<dynamic> Select(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SelectOriginal(dbConn, CommandType.Text, sql, theCmd.DictionaryToParams(parameters));
        }

        public static List<dynamic> Select(this DbConnection dbConn, string sql, object parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SelectOriginal(dbConn, CommandType.Text, sql, theCmd.AnonTypeToParams(parameters));
        }

        public static List<T> SelectWhere<T>(this DbConnection dbConn, string name, object value)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SelectWhere<T>(name, value);
            return SelectOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static List<T> SelectWhere<T>(this DbConnection dbConn, IDictionary<string, object> conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SelectWhere<T>(conditions);
            return SelectOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static List<T> SelectWhere<T>(this DbConnection dbConn, object conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SelectWhere<T>(conditions);
            return SelectOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
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
            var sql = BaseCmd.GetCmd(dbConn.GetProviderType()).SelectByIds<T>(idValues, idField, selectFields);
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
                factor.TableName = ReflectionHelper.GetInfo<T>().TableName;
            }
            if (string.IsNullOrEmpty(factor.Fields))
            {
                factor.Fields = "*";
            }

            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());


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
            totalCount = CountOriginal(dbConn, CommandType.Text, sb.ToString(), ps);

            if (totalCount == 0) //总数为0了，肯定没有数据
            {
                return new List<T>();
            }
            var sql = theCmd.PageSql(factor);

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
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());

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
            totalCount = CountOriginal(dbConn, CommandType.Text, sb.ToString(), ps);

            if (totalCount == 0) //总数为0了，肯定没有数据
            {
                return new List<dynamic>();
            }
            var sql = theCmd.PageSql(factor);

            var list = SelectOriginal<dynamic>(dbConn, CommandType.Text, sql, ps);
            return list;
        }
        
        #endregion

        #region Single

        public static T Single<T>(this DbConnection dbConn, IDictionary<string, object> conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).Single<T>(conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static T Single<T>(this DbConnection dbConn, object conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).Single<T>(conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static T Single<T>(this DbConnection dbConn, string sql)
        {
            return SingleOriginal<T>(dbConn, CommandType.Text, BaseCmd.GetCmd(dbConn.GetProviderType()).FullPartSqlSingle<T>(sql), null);
        }

        public static T Single<T>(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SingleOriginal<T>(dbConn, CommandType.Text, theCmd.FullPartSqlSingle<T>(sql), theCmd.DictionaryToParams(parameters));
        }

        public static dynamic Single(this DbConnection dbConn, string sql)
        {
            return SingleOriginal(dbConn, CommandType.Text, sql, null);
        }

        public static dynamic Single(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SingleOriginal(dbConn, CommandType.Text, sql, theCmd.DictionaryToParams(parameters));
        }

        public static dynamic Single(this DbConnection dbConn, string sql, object parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SingleOriginal(dbConn, CommandType.Text, sql, theCmd.AnonTypeToParams(parameters));
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
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SingleById<T>(idValue, idField);
            return SingleOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static T SingleWhere<T>(this DbConnection dbConn, string name, object value)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SingleWhere<T>(name, value);
            return SingleOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static T SingleWhere<T>(this DbConnection dbConn, IDictionary<string, object> conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SingleWhere<T>(conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static T SingleWhere<T>(this DbConnection dbConn, object conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SingleWhere<T>(conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        #endregion

        #region Scalar

        public static T Scalar<T>(this DbConnection dbConn, string sql)
        {
            return ScalarOriginal<T>(dbConn, CommandType.Text, sql, null);
        }

        public static T Scalar<T>(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return ScalarOriginal<T>(dbConn, CommandType.Text, sql, theCmd.DictionaryToParams(parameters));
        }

        public static T Scalar<T>(this DbConnection dbConn, string sql, object parameters)
        {
            return ScalarOriginal<T>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).AnonTypeToParams(parameters));
        }

        public static T ScalarFmt<T>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return ScalarOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static T MaxID<T>(this DbConnection dbConn, string tableName, string field = "id")
        {
            tableName = SqlInjection.Filter(tableName);
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());

            var sql = string.Format("SELECT {2}(MAX({3}{0}{4}), 0) FROM {3}{1}{4}", field, tableName, theCmd.IFNULL(), theCmd.L(), theCmd.R());
            return ScalarOriginal<T>(dbConn, CommandType.Text, sql);
        }

        #endregion

        #region Column

        public static List<T> Column<T>(this DbConnection dbConn, string sql)
        {
            return ColumnOriginal<T>(dbConn, CommandType.Text, sql, null);
        }

        public static List<T> Column<T>(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            return ColumnOriginal<T>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).DictionaryToParams(parameters));
        }

        public static List<T> Column<T>(this DbConnection dbConn, string sql, object parameters)
        {
            return ColumnOriginal<T>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).AnonTypeToParams(parameters));
        }

        public static List<T> ColumnFmt<T>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return ColumnOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this DbConnection dbConn, string sql)
        {
            return ColumnDistinctOriginal<T>(dbConn, CommandType.Text, sql);
        }

        public static HashSet<T> ColumnDistinct<T>(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            return ColumnDistinctOriginal<T>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).DictionaryToParams(parameters));
        }

        public static HashSet<T> ColumnDistinct<T>(this DbConnection dbConn, string sql, object parameters)
        {
            return ColumnDistinctOriginal<T>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).AnonTypeToParams(parameters));
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
        public static Dictionary<K, List<V>> Lookup<K, V>(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            return LookupOriginal<K, V>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).DictionaryToParams(parameters));
        }
        public static Dictionary<K, List<V>> Lookup<K, V>(this DbConnection dbConn, string sql, object parameters)
        {
            return LookupOriginal<K, V>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).AnonTypeToParams(parameters));
        }

        public static Dictionary<K, List<V>> LookupFmt<K, V>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return LookupOriginal<K, V>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        public static Dictionary<K, V> Dictionary<K, V>(this DbConnection dbConn, string sql)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, sql, null);
        }
        public static Dictionary<K, V> Dictionary<K, V>(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).DictionaryToParams(parameters));
        }
        public static Dictionary<K, V> Dictionary<K, V>(this DbConnection dbConn, string sql, object parameters)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).AnonTypeToParams(parameters));
        }

        public static Dictionary<K, V> DictionaryFmt<K, V>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

        #region Count
        public static int Count<T>(this DbConnection dbConn)
        {
            return CountOriginal(dbConn, CommandType.Text, BaseCmd.GetCmd(dbConn.GetProviderType()).Count<T>());
        }

        public static int Count<T>(this DbConnection dbConn, string sql)
        {
            return CountOriginal(dbConn, CommandType.Text, BaseCmd.GetCmd(dbConn.GetProviderType()).FullPartSql<T>(sql, PartSqlType.Count), null);
        }
        public static int Count<T>(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return CountOriginal(dbConn, CommandType.Text, theCmd.FullPartSql<T>(sql, PartSqlType.Count), theCmd.DictionaryToParams(parameters));
        }
        public static int Count<T>(this DbConnection dbConn, string sql, object parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return CountOriginal(dbConn, CommandType.Text, theCmd.FullPartSql<T>(sql, PartSqlType.Count), theCmd.AnonTypeToParams(parameters));
        }

        public static int CountWhere<T>(this DbConnection dbConn, string name, object value)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).CountWhere<T>(name, value);
            return CountOriginal(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static int CountWhere<T>(this DbConnection dbConn, IDictionary<string, object> conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).CountWhere<T>(conditions);
            return CountOriginal(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static int CountWhere<T>(this DbConnection dbConn, object conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).CountWhere<T>(conditions);
            return CountOriginal(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }


        public static int CountFmt(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return CountOriginal(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

    }
}
