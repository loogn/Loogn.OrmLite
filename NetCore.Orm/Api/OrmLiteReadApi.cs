using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Loogn.OrmLite.MetaData;

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
        public static IDataReader ExecuteReader(this IDbConnection dbConn, CommandType commandType, string commandText, params IDbDataParameter[] ps)
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
        public static List<T> SelectOriginal<T>(this IDbConnection dbConn, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToObjectList<T>(reader);
            }
        }

        /// <summary>
        /// 执行命令，返回多个结果集，返回对象用using释放，Fetch结果的时候最好不要做其他逻辑
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="cmds"></param>
        /// <returns></returns>
        public static MutipleResult SelectMutipleResult(this IDbConnection dbConn, params MutipleCmd[] cmds)
        {
            if (cmds == null || cmds.Length == 0) return new MutipleResult();
            var provider = dbConn.GetCommandDialectProvider();

            List<string> sqls = new List<string>();
            List<IDbDataParameter> ps = new List<IDbDataParameter>();
            for (int i = 0; i < cmds.Length; i++)
            {
                var cmd = cmds[i];
                sqls.Add(cmd.GetMatchedCmdText(i));
                ps.AddRange(TransformToParameters.DictionaryToParams(provider, cmd.GetUniqueParams(i)));
            }
            var sql = string.Join(";", sqls);
            var reader = SqlHelper.ExecuteReader(dbConn, CommandType.Text, sql, ps.ToArray());
            return new MutipleResult(reader);
        }

        /// <summary>
        /// 执行命令，返回动态列表
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps">参数列表</param>
        /// <returns></returns>
        public static List<dynamic> SelectOriginal(this IDbConnection dbConn, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToDynamicList(reader);
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
        public static T SingleOriginal<T>(this IDbConnection dbConn, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToObject<T>(reader);
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
        public static dynamic SingleOriginal(this IDbConnection dbConn, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToDynamic(reader);
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
        public static TValue ScalarOriginal<TValue>(this IDbConnection dbConn, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            var obj = SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
            return TransformForDataReader.ConvertToPrimitiveType<TValue>(obj);
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
        public static List<TField> ColumnOriginal<TField>(this IDbConnection dbConn, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToColumnList<TField>(reader);
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
        public static HashSet<T> ColumnDistinctOriginal<T>(this IDbConnection dbConn, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToColumnSet<T>(reader);
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
        public static Dictionary<K, List<V>> LookupOriginal<K, V>(this IDbConnection dbConn, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToLookup<K, V>(reader);
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
        public static Dictionary<K, V> DictionaryOriginal<K, V>(this IDbConnection dbConn, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            using (var reader = SqlHelper.ExecuteReader(dbConn, commandType, commandText, ps))
            {
                return TransformForDataReader.ReaderToDictionary<K, V>(reader);
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
        public static long CountOriginal(this IDbConnection dbConn, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            var obj = SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
            return TransformForDataReader.ConvertToPrimitiveType<long>(obj);
        }

        #endregion

        #region Select

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <returns></returns>
        public static List<T> Select<T>(this IDbConnection dbConn)
        {
            var cmd = dbConn.GetCommandDialectProvider().Select<T>();
            return SelectOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static List<T> Select<T>(this IDbConnection dbConn, string sql)
        {
            var cmd = dbConn.GetCommandDialectProvider().FullSelect<T>(sql);
            return SelectOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static List<T> Select<T>(this IDbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.FullSelect<T>(sql);
            return SelectOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, provider.Dictionary2Params(parameters));
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static List<T> Select<T>(this IDbConnection dbConn, string sql, object parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.FullSelect<T>(sql);
            return SelectOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, provider.Object2Params(parameters));
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static List<dynamic> Select(this IDbConnection dbConn, string sql)
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
        public static List<dynamic> Select(this IDbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return SelectOriginal(dbConn, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static List<dynamic> Select(this IDbConnection dbConn, string sql, object parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return SelectOriginal(dbConn, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        /// <summary>
        /// 根据指定字段查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="name">条件字段名</param>
        /// <param name="value">条件字段值</param>
        /// <returns></returns>
        public static List<T> SelectWhere<T>(this IDbConnection dbConn, string name, object value)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.SelectWhere<T>(name, value);
            return SelectOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据指定字段查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="conditions">字段字典</param>
        /// <returns></returns>
        public static List<T> SelectWhere<T>(this IDbConnection dbConn, IDictionary<string, object> conditions)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.SelectWhere<T>(conditions);
            return SelectOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据条件匿名对象查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="conditions">条件匿名字段</param>
        /// <returns></returns>
        public static List<T> SelectWhere<T>(this IDbConnection dbConn, object conditions)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.SelectWhere<T>(conditions);
            return SelectOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据格式化sql语句查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat">格式化sql语句</param>
        /// <param name="parameters">格式化参数</param>
        /// <returns></returns>
        public static List<T> SelectFmt<T>(this IDbConnection dbConn, string sqlFormat, params object[] parameters)
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
        public static List<dynamic> SelectFmt(this IDbConnection dbConn, string sqlFormat, params object[] parameters)
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
        public static List<T> SelectByIds<T>(this IDbConnection dbConn, IEnumerable idValues, string idField = OrmLite.KeyName, string selectFields = "*")
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.SelectByIds<T>(idValues, idField, selectFields);
            if (string.IsNullOrEmpty(cmd.CommandText)) return new List<T>();

            return SelectOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 查询分页数据，返回数据列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="factor">分页因子,TableName可选</param>
        /// <param name="totalCount">输出参数，总条数</param>
        /// <returns></returns>
        public static List<T> SelectPage<T>(this IDbConnection dbConn, OrmLitePageFactor factor, out long totalCount)
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
                factor.TableName = TypeCachedDict.GetTypeCachedInfo<T>().TableName;
            }
            if (string.IsNullOrEmpty(factor.Fields))
            {
                factor.Fields = "*";
            }
            var provider = dbConn.GetCommandDialectProvider();

            var ps = factor.Params is IDictionary<string, object> ?
                provider.Dictionary2Params(factor.Params as IDictionary<string, object>) :
                provider.Object2Params(factor.Params);

            var l = provider.OpenQuote;
            var r = provider.CloseQuote;
            //多表连接查询
            if (factor.TableName.IndexOf(" ") > 0)
            {
                l = "";
                r = "";
            }
            //修改表名
            factor.TableName = l + factor.TableName + r;

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
            var cmd = provider.Paged(factor);

            var list = SelectOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, ps);
            return list;
        }

        /// <summary>
        /// 查询分页数据，返回数据列表
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="factor">分页因子，TableName必填</param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public static List<dynamic> SelectPage(this IDbConnection dbConn, OrmLitePageFactor factor, out long totalCount)
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
            var provider = dbConn.GetCommandDialectProvider();

            var l = provider.OpenQuote;
            var r = provider.CloseQuote;
            //多表连接查询
            if (factor.TableName.IndexOf(" ") > 0)
            {
                l = "";
                r = "";
            }
            //修改表名
            factor.TableName = l + factor.TableName + r;

            var ps = factor.Params is IDictionary<string, object> ?
              provider.Dictionary2Params(factor.Params as IDictionary<string, object>) :
              provider.Object2Params(factor.Params);

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
            var cmd = provider.Paged(factor);

            var list = SelectOriginal<dynamic>(dbConn, cmd.CommandType, cmd.CommandText, ps);
            return list;
        }

        /// <summary>
        /// 查询分页数据，返回分页结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn">分页因子，TableName可选</param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static OrmLitePageResult<T> SelectPage<T>(this IDbConnection dbConn, OrmLitePageFactor factor)
        {
            long totalCount;
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
        public static OrmLitePageResult<dynamic> SelectPage(this IDbConnection dbConn, OrmLitePageFactor factor)
        {
            long totalCount;
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
        /// 根据sql语句查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static T Single<T>(this IDbConnection dbConn, string sql)
        {
            var cmd = dbConn.GetCommandDialectProvider().FullSingle<T>(sql);
            return SingleOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据sql语句查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static T Single<T>(this IDbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.FullSingle<T>(sql);
            return SingleOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, provider.Dictionary2Params(parameters));
        }

        /// <summary>
        /// 根据sql语句查询单条数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public static dynamic Single(this IDbConnection dbConn, string sql)
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
        public static dynamic Single(this IDbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return SingleOriginal(dbConn, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }

        /// <summary>
        /// 根据sql语句查询单条数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static dynamic Single(this IDbConnection dbConn, string sql, object parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return SingleOriginal(dbConn, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        /// <summary>
        /// 根据格式化sql语句查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static T SingleFmt<T>(this IDbConnection dbConn, string sqlFormat, params object[] parameters)
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
        public static dynamic SingleFmt(this IDbConnection dbConn, string sqlFormat, params object[] parameters)
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
        public static T SingleById<T>(this IDbConnection dbConn, object idValue, string idField = OrmLite.KeyName)
        {
            var cmd = dbConn.GetCommandDialectProvider().SingleById<T>(idValue, idField);
            return SingleOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据指定字段查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="name">字段名</param>
        /// <param name="value">字段值</param>
        /// <returns></returns>
        public static T SingleWhere<T>(this IDbConnection dbConn, string name, object value)
        {
            var cmd = dbConn.GetCommandDialectProvider().SingleWhere<T>(name, value);
            return SingleOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据指定字段查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="conditions">字段字典</param>
        /// <returns></returns>
        public static T SingleWhere<T>(this IDbConnection dbConn, IDictionary<string, object> conditions)
        {
            var cmd = dbConn.GetCommandDialectProvider().SingleWhere<T>(conditions);
            return SingleOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据指定字段查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="conditions">字段匿名对象</param>
        /// <returns></returns>
        public static T SingleWhere<T>(this IDbConnection dbConn, object conditions)
        {
            var cmd = dbConn.GetCommandDialectProvider().SingleWhere<T>(conditions);
            return SingleOriginal<T>(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
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
        public static TValue Scalar<TValue>(this IDbConnection dbConn, string sql)
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
        public static TValue Scalar<TValue>(this IDbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return ScalarOriginal<TValue>(dbConn, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }

        /// <summary>
        /// 根据sql语句查询首行首列
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static TValue Scalar<TValue>(this IDbConnection dbConn, string sql, object parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return ScalarOriginal<TValue>(dbConn, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        /// <summary>
        /// 根据sql语句查询首行首列
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat">格式化sql语句</param>
        /// <param name="parameters">格式化参数</param>
        /// <returns></returns>
        public static TValue ScalarFmt<TValue>(this IDbConnection dbConn, string sqlFormat, params object[] parameters)
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
        public static TValue MaxID<TValue>(this IDbConnection dbConn, string tableName, string field = OrmLite.KeyName)
        {
            tableName = SqlInjection.Filter(tableName);
            var provider = dbConn.GetCommandDialectProvider();

            var sql = string.Format("SELECT {2}(MAX({3}{0}{4}), 0) FROM {3}{1}{4}", field, tableName, provider.IsNullFunc, provider.OpenQuote, provider.CloseQuote);
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
        public static List<TField> Column<TField>(this IDbConnection dbConn, string sql)
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
        public static List<TField> Column<TField>(this IDbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return ColumnOriginal<TField>(dbConn, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }

        /// <summary>
        /// 根据sql语句查询一列数据
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<TField> Column<TField>(this IDbConnection dbConn, string sql, object parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return ColumnOriginal<TField>(dbConn, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        /// <summary>
        /// 根据格式化sql语句查询一列数据
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat">格式化sql语句</param>
        /// <param name="parameters">格式化参数</param>
        /// <returns></returns>
        public static List<TField> ColumnFmt<TField>(this IDbConnection dbConn, string sqlFormat, params object[] parameters)
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
        public static HashSet<TField> ColumnDistinct<TField>(this IDbConnection dbConn, string sql)
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
        public static HashSet<TField> ColumnDistinct<TField>(this IDbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return ColumnDistinctOriginal<TField>(dbConn, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }

        /// <summary>
        /// 根据sql语句查询一列去重数据
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static HashSet<TField> ColumnDistinct<TField>(this IDbConnection dbConn, string sql, object parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return ColumnDistinctOriginal<TField>(dbConn, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        /// <summary>
        /// 根据sql语句查询一列去重数据
        /// </summary>
        /// <typeparam name="TField"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static HashSet<TField> ColumnDistinctFmt<TField>(this IDbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return ColumnDistinctOriginal<TField>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        #endregion

        #region Lookup Dictionary

        /// <summary>
        /// 查询两列数据，第一列作为Key，聚合第二列作为Value
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static Dictionary<K, List<V>> Lookup<K, V>(this IDbConnection dbConn, string sql)
        {
            return LookupOriginal<K, V>(dbConn, CommandType.Text, sql, null);
        }
        /// <summary>
        /// 查询两列数据，第一列作为Key，聚合第二列作为Value
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Dictionary<K, List<V>> Lookup<K, V>(this IDbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return LookupOriginal<K, V>(dbConn, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }
        /// <summary>
        /// 查询两列数据，第一列作为Key，聚合第二列作为Value
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Dictionary<K, List<V>> Lookup<K, V>(this IDbConnection dbConn, string sql, object parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return LookupOriginal<K, V>(dbConn, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        /// <summary>
        /// 查询两列数据，第一列作为Key，聚合第二列作为Value
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Dictionary<K, List<V>> LookupFmt<K, V>(this IDbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return LookupOriginal<K, V>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }

        /// <summary>
        /// 查询两列数据，第一列作为Key，第二列作为Value
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static Dictionary<K, V> Dictionary<K, V>(this IDbConnection dbConn, string sql)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, sql, null);
        }
        /// <summary>
        /// 查询两列数据，第一列作为Key，第二列作为Value
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Dictionary<K, V> Dictionary<K, V>(this IDbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }

        /// <summary>
        /// 查询两列数据，第一列作为Key，第二列作为Value
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Dictionary<K, V> Dictionary<K, V>(this IDbConnection dbConn, string sql, object parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, sql, provider.Object2Params(parameters));
        }

        /// <summary>
        /// 查询两列数据，第一列作为Key，第二列作为Value
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static Dictionary<K, V> DictionaryFmt<K, V>(this IDbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return DictionaryOriginal<K, V>(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion

        #region Count
        /// <summary>
        /// 查询总条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <returns></returns>
        public static long Count<T>(this IDbConnection dbConn)
        {
            var cmd = dbConn.GetCommandDialectProvider().Count<T>();
            return CountOriginal(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据sql语句查询条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static long Count<T>(this IDbConnection dbConn, string sql)
        {
            var cmd = dbConn.GetCommandDialectProvider().FullCount<T>(sql);
            return CountOriginal(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据sql语句查询条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static long Count<T>(this IDbConnection dbConn, string sql, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.FullCount<T>(sql);
            return CountOriginal(dbConn, cmd.CommandType, cmd.CommandText, provider.Dictionary2Params(parameters));
        }

        /// <summary>
        /// 根据sql语句查询条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static long Count<T>(this IDbConnection dbConn, string sql, object parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.FullCount<T>(sql);
            return CountOriginal(dbConn, cmd.CommandType, cmd.CommandText, provider.Object2Params(parameters));
        }

        /// <summary>
        /// 根据单个字段作为条件查询条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long CountWhere<T>(this IDbConnection dbConn, string name, object value)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.CountWhere<T>(name, value);
            return CountOriginal(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据字段作为条件查询条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static long CountWhere<T>(this IDbConnection dbConn, IDictionary<string, object> conditions)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.CountWhere<T>(conditions);
            return CountOriginal(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据字段作为条件查询条数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static long CountWhere<T>(this IDbConnection dbConn, object conditions)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.CountWhere<T>(conditions);
            return CountOriginal(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据sql语句查询条数
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sqlFormat"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static long CountFmt(this IDbConnection dbConn, string sqlFormat, params object[] parameters)
        {
            return CountOriginal(dbConn, CommandType.Text, string.Format(sqlFormat, parameters));
        }
        #endregion


        #region MetaData

        /// <summary>
        /// 查询指定数据库表的元数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="includeColumns"></param>
        /// <returns></returns>
        public static List<TableMetaData> SelectTableMetaData(this IDbConnection dbConn, bool includeColumns = false)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.TableMetaData(dbConn.Database);
            var tableList = SelectOriginal<TableMetaData>(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
            if (includeColumns)
            {
                foreach (var table in tableList)
                {
                    table.Columns = SelectColumnMetaData(dbConn, table.Name);
                }
            }
            return tableList;
        }

        /// <summary>
        /// 查询指定表的列元数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static List<ColumnMetaData> SelectColumnMetaData(this IDbConnection dbConn, string tableName)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.ColumnMetaData(dbConn.Database, tableName);
            return SelectOriginal<ColumnMetaData>(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        #endregion

    }
}
