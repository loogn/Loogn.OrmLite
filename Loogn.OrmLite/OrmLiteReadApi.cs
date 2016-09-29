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
    /// <summary>
    /// 读操作API
    /// </summary>
    public static partial class OrmLiteReadApi
    {
        #region Original Function
        /// <summary>
        /// 执行命令，返回DataReader
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps">参数列表</param>
        /// <returns></returns>
        public static DbDataReader ExecuteReader(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            return SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps);
        }

        /// <summary>
        /// 执行命令，返回实体列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps">参数列表</param>
        /// <returns></returns>
        public static List<T> SelectOriginal<T>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToObjectList<T>(reader);
            }
        }

        /// <summary>
        /// 执行命令，返回动态列表
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps">参数列表</param>
        /// <returns></returns>
        public static List<dynamic> SelectOriginal(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToDynamicList(reader);
            }
        }

        /// <summary>
        /// 执行命令，返回实体对象，查不到返回null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps">参数列表</param>
        /// <returns></returns>
        public static T SingleOriginal<T>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToObject<T>(reader);
            }
        }

        /// <summary>
        /// 执行命令，返回动态对象，查不到返回null
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps">参数列表</param>
        /// <returns></returns>
        public static dynamic SingleOriginal(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToDynamic(reader);
            }
        }

        /// <summary>
        /// 执行命令，返回首行首列数据
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps">参数列表</param>
        /// <returns></returns>
        public static TValue ScalarOriginal<TValue>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            var obj = SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
            return Mapping.ConvertToPrimitiveType<TValue>(obj);
        }

        /// <summary>
        /// 执行命令，返回首列数据
        /// </summary>
        /// <typeparam name="TField">首列数据类型</typeparam>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static List<TField> ColumnOriginal<TField>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToColumnList<TField>(reader);
            }
        }

        /// <summary>
        /// 执行命令，返回首列数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static HashSet<T> ColumnDistinctOriginal<T>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToColumnSet<T>(reader);
            }
        }

        /// <summary>
        /// 执行命令，返回由第一列聚合第二列的字典
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static Dictionary<K, List<V>> LookupOriginal<K, V>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToLookup<K, V>(reader);
            }
        }

        /// <summary>
        /// 执行命令，返回由第一列映射第二列的字典
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static Dictionary<K, V> DictionaryOriginal<K, V>(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return Mapping.ReaderToDictionary<K, V>(reader);
            }
        }

        /// <summary>
        /// 执行命令，返回首行首列的数字
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps"></param>
        /// <returns></returns>
        public static int CountOriginal(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            var obj = SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
            return Mapping.ConvertToPrimitiveType<int>(obj);
        }

        #endregion

        #region Select

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <returns></returns>
        public static List<T> Select<T>(this DbConnection dbConn)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, BaseCmd.GetCmd(dbConn.GetProviderType()).Select<T>());
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static List<T> Select<T>(this DbConnection dbConn, string sql)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, BaseCmd.GetCmd(dbConn.GetProviderType()).FullPartSql<T>(sql, PartSqlType.Select), null);
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static List<T> Select<T>(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SelectOriginal<T>(dbConn, CommandType.Text, theCmd.FullPartSql<T>(sql, PartSqlType.Select), theCmd.DictionaryToParams(parameters));
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static List<T> Select<T>(this DbConnection dbConn, string sql, object parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SelectOriginal<T>(dbConn, CommandType.Text, theCmd.FullPartSql<T>(sql, PartSqlType.Select), theCmd.AnonTypeToParams(parameters));
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static List<dynamic> Select(this DbConnection dbConn, string sql)
        {
            return SelectOriginal(dbConn, CommandType.Text, sql, null);
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static List<dynamic> Select(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SelectOriginal(dbConn, CommandType.Text, sql, theCmd.DictionaryToParams(parameters));
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static List<dynamic> Select(this DbConnection dbConn, string sql, object parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SelectOriginal(dbConn, CommandType.Text, sql, theCmd.AnonTypeToParams(parameters));
        }

        /// <summary>
        /// 根据指定字段查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="name">条件字段名</param>
        /// <param name="value">条件字段值</param>
        /// <returns></returns>
        public static List<T> SelectWhere<T>(this DbConnection dbConn, string name, object value)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SelectWhere<T>(name, value);
            return SelectOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        /// <summary>
        /// 根据指定字段查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="conditions">字段字典</param>
        /// <returns></returns>
        public static List<T> SelectWhere<T>(this DbConnection dbConn, IDictionary<string, object> conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SelectWhere<T>(conditions);
            return SelectOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        /// <summary>
        /// 根据条件匿名对象查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="conditions">条件匿名字段</param>
        /// <returns></returns>
        public static List<T> SelectWhere<T>(this DbConnection dbConn, object conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SelectWhere<T>(conditions);
            return SelectOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        /// <summary>
        /// 根据格式化sql语句查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat">格式化sql语句</param>
        /// <param name="parameters">格式化参数</param>
        /// <returns></returns>
        public static List<T> SelectFmt<T>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return SelectOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        /// <summary>
        /// 根据格式化sql语句查询数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat">格式化sql语句</param>
        /// <param name="parameters">格式化参数</param>
        /// <returns></returns>
        public static List<dynamic> SelectFmt(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return SelectOriginal(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        /// <summary>
        /// 根据字段集合查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="idValues">字段值集合</param>
        /// <param name="idField">字段名称，默认是ID</param>
        /// <param name="selectFields">要查询的字段</param>
        /// <returns></returns>
        public static List<T> SelectByIds<T>(this DbConnection dbConn, IEnumerable idValues, string idField = OrmLite.KeyName, string selectFields = "*")
        {
            var sql = BaseCmd.GetCmd(dbConn.GetProviderType()).SelectByIds<T>(idValues, idField, selectFields);
            if (sql == null) return new List<T>();
            return SelectOriginal<T>(dbConn, CommandType.Text, sql);
        }

        /// <summary>
        /// 查询分页数据，返回数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="factor">分页因子,TableName可选</param>
        /// <param name="totalCount">输出参数，总条数</param>
        /// <returns></returns>
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

        /// <summary>
        /// 查询分页数据，返回数据列表
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="factor">分页因子，TableName必填</param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 查询分页数据，返回分页结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">分页因子，TableName可选</param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static OrmLitePageResult<T> SelectPage<T>(this DbConnection dbConn, OrmLitePageFactor factor)
        {
            int totalCount;
            OrmLitePageResult<T> pageInfo = new OrmLitePageResult<T>();
            pageInfo.List = dbConn.SelectPage<T>(factor, out totalCount);
            pageInfo.PageIndex = factor.PageIndex;
            pageInfo.PageSize = factor.PageSize;
            pageInfo.TotalCount = totalCount;
            return pageInfo;
        }

        /// <summary>
        /// 查询分页数据，返回分页结果
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="factor">分页因子，TableName必填</param>
        /// <returns></returns>
        public static OrmLitePageResult<dynamic> SelectPage(this DbConnection dbConn, OrmLitePageFactor factor)
        {
            int totalCount;
            OrmLitePageResult<dynamic> pageInfo = new OrmLitePageResult<dynamic>();
            pageInfo.List = dbConn.SelectPage(factor, out totalCount);
            pageInfo.PageIndex = factor.PageIndex;
            pageInfo.PageSize = factor.PageSize;
            pageInfo.TotalCount = totalCount;
            return pageInfo;
        }
        #endregion

        #region Single
        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="conditions">条件字段</param>
        /// <returns></returns>
        public static T Single<T>(this DbConnection dbConn, IDictionary<string, object> conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).Single<T>(conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        /// <summary>
        /// 查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="conditions">条件匿名对象</param>
        /// <returns></returns>
        public static T Single<T>(this DbConnection dbConn, object conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).Single<T>(conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        /// <summary>
        /// 根据sql语句查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static T Single<T>(this DbConnection dbConn, string sql)
        {
            return SingleOriginal<T>(dbConn, CommandType.Text, BaseCmd.GetCmd(dbConn.GetProviderType()).FullPartSqlSingle<T>(sql), null);
        }

        /// <summary>
        /// 根据sql语句查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static T Single<T>(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SingleOriginal<T>(dbConn, CommandType.Text, theCmd.FullPartSqlSingle<T>(sql), theCmd.DictionaryToParams(parameters));
        }

        /// <summary>
        /// 根据sql语句查询单条数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static dynamic Single(this DbConnection dbConn, string sql)
        {
            return SingleOriginal(dbConn, CommandType.Text, sql, null);
        }

        /// <summary>
        /// 根据sql语句查询单条数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static dynamic Single(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SingleOriginal(dbConn, CommandType.Text, sql, theCmd.DictionaryToParams(parameters));
        }

        /// <summary>
        /// 根据sql语句查询单条数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static dynamic Single(this DbConnection dbConn, string sql, object parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return SingleOriginal(dbConn, CommandType.Text, sql, theCmd.AnonTypeToParams(parameters));
        }

        /// <summary>
        /// 根据格式化sql语句查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static T SingleFmt<T>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return SingleOriginal<T>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        /// <summary>
        /// 根据格式化sql语句查询单条数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static dynamic SingleFmt(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return SingleOriginal(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        /// <summary>
        /// 根据指定字段查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="idValue">字段值</param>
        /// <param name="idField">字段名</param>
        /// <returns></returns>
        public static T SingleById<T>(this DbConnection dbConn, object idValue, string idField = OrmLite.KeyName)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SingleById<T>(idValue, idField);
            return SingleOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        /// <summary>
        /// 根据指定字段查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="name">字段名</param>
        /// <param name="value">字段值</param>
        /// <returns></returns>
        public static T SingleWhere<T>(this DbConnection dbConn, string name, object value)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SingleWhere<T>(name, value);
            return SingleOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        /// <summary>
        /// 根据指定字段查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="conditions">字段字典</param>
        /// <returns></returns>
        public static T SingleWhere<T>(this DbConnection dbConn, IDictionary<string, object> conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SingleWhere<T>(conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        /// <summary>
        /// 根据指定字段查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="conditions">字段匿名对象</param>
        /// <returns></returns>
        public static T SingleWhere<T>(this DbConnection dbConn, object conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).SingleWhere<T>(conditions);
            return SingleOriginal<T>(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        #endregion

        #region Scalar

        /// <summary>
        /// 根据sql语句查询首行首列
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static TValue Scalar<TValue>(this DbConnection dbConn, string sql)
        {
            return ScalarOriginal<TValue>(dbConn, CommandType.Text, sql, null);
        }

        /// <summary>
        /// 根据sql语句查询首行首列
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static TValue Scalar<TValue>(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());
            return ScalarOriginal<TValue>(dbConn, CommandType.Text, sql, theCmd.DictionaryToParams(parameters));
        }

        /// <summary>
        /// 根据sql语句查询首行首列
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static TValue Scalar<TValue>(this DbConnection dbConn, string sql, object parameters)
        {
            return ScalarOriginal<TValue>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).AnonTypeToParams(parameters));
        }

        /// <summary>
        /// 根据sql语句查询首行首列
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat">格式化sql语句</param>
        /// <param name="parameters">格式化参数</param>
        /// <returns></returns>
        public static TValue ScalarFmt<TValue>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return ScalarOriginal<TValue>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }


        /// <summary>
        /// 得到表中某列的最大值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="tableName">表名</param>
        /// <param name="field">列名</param>
        /// <returns></returns>
        public static TValue MaxID<TValue>(this DbConnection dbConn, string tableName, string field = OrmLite.KeyName)
        {
            tableName = SqlInjection.Filter(tableName);
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());

            var sql = string.Format("SELECT {2}(MAX({3}{0}{4}), 0) FROM {3}{1}{4}", field, tableName, theCmd.IFNULL(), theCmd.L(), theCmd.R());
            return ScalarOriginal<TValue>(dbConn, CommandType.Text, sql);
        }

        #endregion

        #region Column

        /// <summary>
        /// 根据sql语句查询一列数据
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static List<TField> Column<TField>(this DbConnection dbConn, string sql)
        {
            return ColumnOriginal<TField>(dbConn, CommandType.Text, sql, null);
        }

        /// <summary>
        /// 根据sql语句查询一列数据
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static List<TField> Column<TField>(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            return ColumnOriginal<TField>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).DictionaryToParams(parameters));
        }

        /// <summary>
        /// 根据sql语句查询一列数据
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<TField> Column<TField>(this DbConnection dbConn, string sql, object parameters)
        {
            return ColumnOriginal<TField>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).AnonTypeToParams(parameters));
        }

        /// <summary>
        /// 根据格式化sql语句查询一列数据
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat">格式化sql语句</param>
        /// <param name="parameters">格式化参数</param>
        /// <returns></returns>
        public static List<TField> ColumnFmt<TField>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return ColumnOriginal<TField>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        /// <summary>
        /// 根据sql语句查询一列去重数据
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static HashSet<TField> ColumnDistinct<TField>(this DbConnection dbConn, string sql)
        {
            return ColumnDistinctOriginal<TField>(dbConn, CommandType.Text, sql);
        }

        /// <summary>
        /// 根据sql语句查询一列去重数据
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static HashSet<TField> ColumnDistinct<TField>(this DbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            return ColumnDistinctOriginal<TField>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).DictionaryToParams(parameters));
        }

        /// <summary>
        /// 根据sql语句查询一列去重数据
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static HashSet<TField> ColumnDistinct<TField>(this DbConnection dbConn, string sql, object parameters)
        {
            return ColumnDistinctOriginal<TField>(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).AnonTypeToParams(parameters));
        }

        /// <summary>
        /// 根据sql语句查询一列去重数据
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static HashSet<TField> ColumnDistinctFmt<TField>(this DbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return ColumnDistinctOriginal<TField>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
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
