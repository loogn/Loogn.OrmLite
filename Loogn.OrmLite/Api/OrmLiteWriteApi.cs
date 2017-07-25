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
    public static partial class OrmLiteWriteApi
    {
        /// <summary>
        /// 确保打开数据库连接
        /// </summary>
        /// <param name="dbConn"></param>
        public static void EnsureOpen(this IDbConnection dbConn)
        {
            if (dbConn.State != ConnectionState.Open)
            {
                dbConn.Open();
            }
        }

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="name">存储过程名称</param>
        /// <param name="inParams">输入参数</param>
        /// <param name="execute">是否立即执行</param>
        /// <returns></returns>
        public static IDbCommand Proc(this IDbConnection dbConn, string name, object inParams = null, bool execute = false)
        {
            var cmd = dbConn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = name;

            if (inParams != null)
            {
                var provider = dbConn.GetCommandDialectProvider();
                var ps = inParams is IDictionary<string, object> ?
                provider.Dictionary2Params(inParams as IDictionary<string, object>) :
                provider.Object2Params(inParams);
                foreach (var p in ps)
                {
                    cmd.Parameters.Add(p);
                }
            }
            dbConn.Open();
            if (execute)
            {
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            return cmd;
        }

        /// <summary>
        /// 执行命令,返回影响行数
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps">参数列表</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(this IDbConnection dbConn, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            return SqlHelper.ExecuteNonQuery(dbConn, commandType, commandText, ps);
        }

        /// <summary>
        /// 执行命令,返回影响行数
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(this IDbConnection dbConn, CommandType commandType, string commandText, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return SqlHelper.ExecuteNonQuery(dbConn, commandType, commandText, provider.Dictionary2Params(parameters));
        }

        /// <summary>
        /// 执行命令，返回首行首列
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps">参数列表</param>
        /// <returns></returns>
        public static object ExecuteScalar(this IDbConnection dbConn, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            return SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
        }

        /// <summary>
        /// 执行命令，返回首行首列
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public static object ExecuteScalar(this IDbConnection dbConn, CommandType commandType, string commandText, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return SqlHelper.ExecuteNonQuery(dbConn, commandType, commandText, provider.Dictionary2Params(parameters));
        }

        /// <summary>
        /// 插入实体，返回影响行数或自增列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="obj"></param>
        /// <param name="selectIdentity">是否返回自增列</param>
        /// <returns></returns>
        public static long Insert<T>(this IDbConnection dbConn, T obj, bool selectIdentity = false)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.Insert<T>(obj, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbConn,cmd.CommandType, cmd.CommandText, cmd.Params);
                if (identity == null || identity is DBNull)
                {
                    return 0;
                }
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn,cmd.CommandType, cmd.CommandText, cmd.Params);
                return raw;
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="table">表名</param>
        /// <param name="fields">字段字典</param>
        /// <param name="selectIdentity">是否返回自增列</param>
        /// <returns></returns>
        public static long Insert(this IDbConnection dbConn, string table, IDictionary<string, object> fields, bool selectIdentity = false)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.Insert(table, fields, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbConn,cmd.CommandType, cmd.CommandText, cmd.Params);
                if (identity == null || identity is DBNull)
                {
                    return 0;
                }
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn,cmd.CommandType, cmd.CommandText, cmd.Params);
                return raw;
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="table">表名</param>
        /// <param name="anonType">字典匿名对象</param>
        /// <param name="selectIdentity">是否返回自增列</param>
        /// <returns></returns>
        public static long Insert(this IDbConnection dbConn, string table, object anonType, bool selectIdentity = false)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.Insert(table, anonType, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbConn,cmd.CommandType, cmd.CommandText, cmd.Params);
                if (identity == null || identity is DBNull)
                {
                    return 0;
                }
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn,cmd.CommandType, cmd.CommandText, cmd.Params);
                return raw;
            }
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="table">表名</param>
        /// <param name="objs">匿名对象列表</param>
        /// <returns></returns>
        public static bool Insert(this IDbConnection dbConn, string table, params object[] objs)
        {
            return InsertAll(dbConn, table, objs);
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="table"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static bool InsertAll(this IDbConnection dbConn, string table, IEnumerable objs)
        {
            if (objs != null)
            {
                dbConn.EnsureOpen();
                var trans = dbConn.BeginTransaction();
                try
                {
                    foreach (var obj in objs)
                    {
                        var rowCount = InsertTrans(trans, table, obj);
                        if (rowCount == 0)
                        {
                            trans.Rollback();
                            return false;
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception exp)
                {
                    trans.Rollback();
                    throw exp;
                }
                finally
                {
                    trans.Dispose();
                }
            }
            return true;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static bool Insert<T>(this IDbConnection dbConn, params T[] objs)
        {
            return InsertAll<T>(dbConn, objs);
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static bool InsertAll<T>(this IDbConnection dbConn, IEnumerable<T> objs)
        {
            if (objs != null)
            {
                dbConn.EnsureOpen();
                var trans = dbConn.BeginTransaction();
                try
                {
                    foreach (var obj in objs)
                    {
                        var rowCount = InsertTrans<T>(trans, obj);
                        if (rowCount == 0)
                        {
                            trans.Rollback();
                            return false;
                        }
                    }
                    trans.Commit();
                    return true;
                }
                catch (Exception exp)
                {
                    trans.Rollback();
                    throw exp;
                }
                finally
                {
                    trans.Dispose();
                }
            }
            return true;
        }

        /// <summary>
        /// 根据主键修改字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="obj"></param>
        /// <param name="updateFields">要修改的字段</param>
        /// <returns></returns>
        public static int Update<T>(this IDbConnection dbConn, T obj, params string[] updateFields)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.Update<T>(obj, updateFields);
            int c = ExecuteNonQuery(dbConn,cmd.CommandType, cmd.CommandText, cmd.Params);
            return c;
        }

        /// <summary>
        /// 根据ID修改匿名对象
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="tableName">表名</param>
        /// <param name="anonymous">匿名对象</param>
        /// <returns></returns>
        public static int UpdateAnonymous(this IDbConnection dbConn, string tableName, object anonymous)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.Update(tableName, anonymous);
            int c = ExecuteNonQuery(dbConn,cmd.CommandType, cmd.CommandText, cmd.Params);
            return c;
        }

        /// <summary>
        /// 根据主键修改匿名对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="anonymous">匿名对象</param>
        /// <returns></returns>
        public static int UpdateAnonymous<T>(this IDbConnection dbConn, object anonymous)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.Update(TypeCachedDict.GetTypeCachedInfo<T>().TableName, anonymous);
            int c = ExecuteNonQuery(dbConn,cmd.CommandType, cmd.CommandText, cmd.Params);
            return c;
        }

        /// <summary>
        /// 根据主键批量修改数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="objs">数据集合</param>
        /// <returns></returns>
        public static int Update<T>(this IDbConnection dbConn, params T[] objs)
        {
            return UpdateAll<T>(dbConn, objs);
        }

        /// <summary>
        /// 根据主键批量修改数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="objs">数据集合</param>
        /// <returns></returns>
        public static int UpdateAll<T>(this IDbConnection dbConn, IEnumerable<T> objs)
        {
            int row = 0;
            if (objs != null)
            {
                dbConn.EnsureOpen();
                var trans = dbConn.BeginTransaction();
                try
                {
                    foreach (var obj in objs)
                    {
                        var rowCount = UpdateTrans<T>(trans, obj);
                        if (rowCount == 0)
                        {
                            trans.Rollback();
                            row = 0;
                            break;
                        }
                        row++;
                    }
                    trans.Commit();
                }
                catch (Exception exp)
                {
                    trans.Rollback();
                    throw exp;
                }
                finally
                {
                    trans.Dispose();
                }
            }
            return row;
        }

        /// <summary>
        /// 根据条件修改数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="updateFields">被修改的字段字典</param>
        /// <param name="conditions">条件语句</param>
        /// <param name="parameters">参数字段</param>
        /// <returns></returns>
        public static int Update<T>(this IDbConnection dbConn, IDictionary<string, object> updateFields, string conditions, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.Update(TypeCachedDict.GetTypeCachedInfo<T>().TableName, updateFields, conditions, parameters);

            int c = ExecuteNonQuery(dbConn,cmd.CommandType, cmd.CommandText, cmd.Params);
            return c;
        }

        /// <summary>
        /// 根据条件修改数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="tableName">表名</param>
        /// <param name="updateFields">被修改的字段字典</param>
        /// <param name="conditions">条件语句</param>
        /// <param name="parameters">参数字段</param>
        /// <returns></returns>
        public static int Update(this IDbConnection dbConn, string tableName, IDictionary<string, object> updateFields, string conditions, IDictionary<string, object> parameters)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.Update(tableName, updateFields, conditions, parameters);
            int c = ExecuteNonQuery(dbConn,cmd.CommandType, cmd.CommandText, cmd.Params);
            return c;
        }

        /// <summary>
        /// 根据单个字段修数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="tableName">表名</param>
        /// <param name="updateFields">被修改的字段字典</param>
        /// <param name="id">条件字段值</param>
        /// <param name="idname">条件字段名，默认是ID</param>
        /// <returns></returns>
        public static int UpdateById(this IDbConnection dbConn, string tableName, IDictionary<string, object> updateFields, object id, string idname = OrmLite.KeyName)
        {
            return Update(dbConn, tableName, updateFields, idname + "=@id", DictBuilder.Assign("id", id));
        }

        /// <summary>
        /// 根据单个字段修改数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="updateFields">被修改的字段字典</param>
        /// <param name="id">条件字段值</param>
        /// <param name="idname">条件字段名，默认是ID</param>
        /// <returns></returns>
        public static int UpdateById<T>(this IDbConnection dbConn, IDictionary<string, object> updateFields, object id, string idname = OrmLite.KeyName)
        {
            return Update<T>(dbConn, updateFields, idname + "=@id", DictBuilder.Assign("id", id));
        }

        /// <summary>
        /// 根据一个字段修改另一个字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="fieldName">被修改的字段名</param>
        /// <param name="fieldValue">被修改的字段值</param>
        /// <param name="id">条件字段值</param>
        /// <param name="idname">条件字段名，默认是ID</param>
        /// <returns></returns>
        public static int UpdateFieldById<T>(this IDbConnection dbConn, string fieldName, object fieldValue, object id, string idname = OrmLite.KeyName)
        {
            return Update<T>(dbConn, DictBuilder.Assign(fieldName, fieldValue), idname + "=@id", DictBuilder.Assign("id", id));
        }

        /// <summary>
        /// 根据sql语句删除数据
        /// </summary>
        /// <param name="dbConn"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public static int Delete(this IDbConnection dbConn, string sql, IDictionary<string, object> parameters = null)
        {
            var provider = dbConn.GetCommandDialectProvider();
            return ExecuteNonQuery(dbConn,CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }

        /// <summary>
        /// 根据字段删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="id">字段值</param>
        /// <param name="idField">字段名</param>
        /// <returns></returns>
        public static int DeleteById<T>(this IDbConnection dbConn, object id, string idField = OrmLite.KeyName)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.DeleteById<T>(id, idField);
            return ExecuteNonQuery(dbConn,cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据字段集合删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <param name="idValues">字段值集合</param>
        /// <param name="idFields">字段名，默认是ID</param>
        /// <returns></returns>
        public static int DeleteByIds<T>(this IDbConnection dbConn, IEnumerable idValues, string idFields = OrmLite.KeyName)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.DeleteByIds<T>(idValues, idFields);
            if (string.IsNullOrEmpty(cmd.CommandText)) return 0;
            return ExecuteNonQuery(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 删除所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbConn"></param>
        /// <returns></returns>
        public static int Delete<T>(this IDbConnection dbConn)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.Delete<T>();
            return ExecuteNonQuery(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static int DeleteWhere<T>(this IDbConnection dbConn, string name, object value)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.DeleteWhere<T>(name, value);
            return ExecuteNonQuery(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static int DeleteWhere<T>(this IDbConnection dbConn, object conditions)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.DeleteWhere<T>(conditions);
            return ExecuteNonQuery(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static int DeleteWhere<T>(this IDbConnection dbConn, IDictionary<string, object> conditions)
        {
            var provider = dbConn.GetCommandDialectProvider();
            var cmd = provider.DeleteWhere<T>(conditions);
            return ExecuteNonQuery(dbConn, cmd.CommandType, cmd.CommandText, cmd.Params);
        }
    }
}
