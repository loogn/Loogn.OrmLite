using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Data.Common;

namespace Loogn.OrmLite
{
    /// <summary>
    /// 写操作API
    /// </summary>
    public static partial class OrmLiteWriteApi
    {
        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <param name="name">存储过程名称</param>
        /// <param name="inParams">输入参数</param>
        /// <param name="excludeDefaults">是否立即执行</param>
        /// <returns></returns>
        public static IDbCommand Proc(this IDbTransaction dbTrans, string name, object inParams = null, bool excludeDefaults = false)
        {
            var cmd = dbTrans.Connection.CreateCommand();
            cmd.Transaction = dbTrans;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = name;
            if (inParams != null)
            {
                var provider = dbTrans.GetCommandDialectProvider();

                var ps = inParams is IDictionary<string, object> ?
                provider.Dictionary2Params(inParams as IDictionary<string, object>) :
                provider.Object2Params(inParams);
                foreach (var p in ps)
                {
                    cmd.Parameters.Add(p);
                }
            }
            if (excludeDefaults)
            {
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            return cmd;
        }

        /// <summary>
        /// 执行命令,返回影响行数
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps">参数列表</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(this IDbTransaction dbTrans, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            return SqlHelper.ExecuteNonQuery(dbTrans, commandType, commandText, ps);
        }

        /// <summary>
        /// 执行命令,返回影响行数
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(this IDbTransaction dbTrans, CommandType commandType, string commandText, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return SqlHelper.ExecuteNonQuery(dbTrans, commandType, commandText, provider.Dictionary2Params(parameters));
        }

        /// <summary>
        /// 执行命令，返回首行首列
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="ps">参数列表</param>
        /// <returns></returns>
        public static object ExecuteScalar(this IDbTransaction dbTrans, CommandType commandType, string commandText, params IDbDataParameter[] ps)
        {
            return SqlHelper.ExecuteScalar(dbTrans, commandType, commandText, ps);
        }

        /// <summary>
        /// 执行命令，返回首行首列
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <param name="commandType">命令类型</param>
        /// <param name="commandText">命令文本</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public static object ExecuteScalar(this IDbTransaction dbTrans, CommandType commandType, string commandText, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return SqlHelper.ExecuteNonQuery(dbTrans, commandType, commandText, provider.Dictionary2Params(parameters));
        }


        /// <summary>
        /// 插入实体，返回影响行数或自增列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTrans"></param>
        /// <param name="obj"></param>
        /// <param name="selectIdentity">是否返回自增列</param>
        /// <returns></returns>
        public static long Insert<T>(this IDbTransaction dbTrans, T obj, bool selectIdentity = false)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Insert<T>(obj, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
                if (identity == null || identity is DBNull)
                {
                    return 0;
                }
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
                return raw;
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <param name="table">表名</param>
        /// <param name="fields">字段字典</param>
        /// <param name="selectIdentity">是否返回自增列</param>
        /// <returns></returns>
        public static long Insert(this IDbTransaction dbTrans, string table, IDictionary<string, object> fields, bool selectIdentity = false)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Insert(table, fields, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
                if (identity == null || identity is DBNull)
                {
                    return 0;
                }
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
                return raw;
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <param name="table">表名</param>
        /// <param name="anonType">字典匿名对象</param>
        /// <param name="selectIdentity">是否返回自增列</param>
        /// <returns></returns>
        public static long Insert(this IDbTransaction dbTrans, string table, object anonType, bool selectIdentity = false)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Insert(table, anonType, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
                if (identity == null || identity is DBNull)
                {
                    return 0;
                }
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
                return raw;
            }
        }


        private static int InsertTrans<T>(this IDbTransaction dbTrans, T obj)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Insert<T>(obj, false);
            var raw = ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
            return raw;
        }
        private static int InsertTrans(this IDbTransaction dbTrans, string table, object anonType)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var l = provider.OpenQuote;
            var r = provider.CloseQuote;
            StringBuilder sbsql = new StringBuilder(50);
            sbsql.AppendFormat("insert into {1}{0}{2} (", table, l, r);
            StringBuilder sbParams = new StringBuilder(") values (", 50);
            var ps = new List<IDbDataParameter>();

            if (anonType is IDictionary<string, object>)
            {
                foreach (var kv in anonType as IDictionary<string, object>)
                {
                    var fieldName = kv.Key;
                    var val = kv.Value;
                    sbsql.AppendFormat("{1}{0}{2},", fieldName, l, r);
                    sbParams.AppendFormat("@{0},", fieldName);
                    var p = provider.CreateParameter();
                    p.ParameterName = "@" + fieldName;
                    p.Value = val ?? DBNull.Value;
                    ps.Add(p);
                }
            }
            else
            {
                var typeInfo = TypeCachedDict.GetTypeCachedInfo(anonType.GetType());
                foreach (var kv in typeInfo.PropInvokerDict)
                {
                    var fieldName = kv.Key;
                    var val = kv.Value.Get(anonType);
                    sbsql.AppendFormat("{1}{0}{2},", fieldName, l, r);
                    sbParams.AppendFormat("@{0},", fieldName);

                    var p = provider.CreateParameter();
                    p.ParameterName = "@" + fieldName;
                    p.Value = val ?? DBNull.Value;
                    ps.Add(p);
                }
            }

            if (ps.Count == 0)
            {
                throw new ArgumentException("model里没有字段，无法插入");
            }
            sbsql.Remove(sbsql.Length - 1, 1);
            sbParams.Remove(sbParams.Length - 1, 1);
            sbsql.Append(sbParams.ToString());
            sbsql.Append(")");
            var raw = ExecuteNonQuery(dbTrans, CommandType.Text, sbsql.ToString(), ps.ToArray());
            return raw;
        }


        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <param name="table"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static bool InsertAll(this IDbTransaction dbTrans, string table, IEnumerable objs)
        {
            if (objs != null)
            {
                foreach (var obj in objs)
                {
                    var rowCount = InsertTrans(dbTrans, table, obj);
                    if (rowCount == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTrans"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        public static bool InsertAll<T>(this IDbTransaction dbTrans, IEnumerable<T> objs)
        {
            if (objs != null)
            {
                foreach (var obj in objs)
                {
                    var rowCount = InsertTrans<T>(dbTrans, obj);
                    if (rowCount == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 根据主键修改字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTrans"></param>
        /// <param name="obj"></param>
        /// <param name="updateFields">要修改的字段</param>
        /// <returns></returns>
        public static int Update<T>(this IDbTransaction dbTrans, T obj, params string[] updateFields)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Update<T>(obj, updateFields);
            int c = ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
            return c;
        }

        /// <summary>
        /// 根据ID修改匿名对象
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <param name="tableName">表名</param>
        /// <param name="anonymous">匿名对象</param>
        /// <returns></returns>
        public static int UpdateAnonymous(this IDbTransaction dbTrans, string tableName, object anonymous)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Update(tableName, anonymous);
            int c = ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
            return c;
        }

        /// <summary>
        /// 根据主键修改匿名对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTrans"></param>
        /// <param name="anonymous">匿名对象</param>
        /// <returns></returns>
        public static int UpdateAnonymous<T>(this IDbTransaction dbTrans, object anonymous)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Update(TypeCachedDict.GetTypeCachedInfo<T>().TableName, anonymous);
            int c = ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
            return c;
        }

        private static int UpdateTrans<T>(this IDbTransaction dbTrans, T obj)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Update<T>(obj);
            int c = ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
            return c;
        }

        /// <summary>
        /// 根据主键批量修改数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTrans"></param>
        /// <param name="objs">数据集合</param>
        /// <returns></returns>
        public static int UpdateAll<T>(this IDbTransaction dbTrans, IEnumerable<T> objs)
        {
            int row = 0;
            foreach (var obj in objs)
            {
                var rowCount = UpdateTrans<T>(dbTrans, obj);
                row++;
            }
            return row;
        }

        /// <summary>
        /// 根据条件修改数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTrans"></param>
        /// <param name="updateFields">被修改的字段字典</param>
        /// <param name="conditions">条件语句</param>
        /// <param name="parameters">参数字段</param>
        /// <returns></returns>
        public static int Update<T>(this IDbTransaction dbTrans, IDictionary<string, object> updateFields, string conditions, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Update(TypeCachedDict.GetTypeCachedInfo<T>().TableName, updateFields, conditions, parameters);
            int c = ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
            return c;
        }

        public static int UpdateWhere<T>(this IDbTransaction dbTrans, IDictionary<string, object> updateFields, IDictionary<string, object> conditions)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.UpdateWhere(TypeCachedDict.GetTypeCachedInfo<T>().TableName, updateFields, conditions);
            int c = ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
            return c;
        }

        public static int UpdateWhere(this IDbTransaction dbTrans, string tableName, IDictionary<string, object> updateFields, IDictionary<string, object> conditions)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.UpdateWhere(tableName, updateFields, conditions);
            int c = ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
            return c;
        }

        /// <summary>
        /// 根据条件修改数据
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <param name="tableName">表名</param>
        /// <param name="updateFields">被修改的字段字典</param>
        /// <param name="conditions">条件语句</param>
        /// <param name="parameters">参数字段</param>
        /// <returns></returns>
        public static int Update(this IDbTransaction dbTrans, string tableName, IDictionary<string, object> updateFields, string conditions, IDictionary<string, object> parameters)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Update(tableName, updateFields, conditions, parameters);
            int c = ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
            return c;
        }



        /// <summary>
        /// 根据单个字段修数据
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <param name="tableName">表名</param>
        /// <param name="updateFields">被修改的字段字典</param>
        /// <param name="id">条件字段值</param>
        /// <param name="idname">条件字段名，默认是ID</param>
        /// <returns></returns>
        public static int UpdateById(this IDbTransaction dbTrans, string tableName, IDictionary<string, object> updateFields, object id, string idname = OrmLite.KeyName)
        {
            return Update(dbTrans, tableName, updateFields, idname + "=@id", DictBuilder.Assign("id", id));
        }

        /// <summary>
        /// 根据单个字段修改数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTrans"></param>
        /// <param name="updateFields">被修改的字段字典</param>
        /// <param name="id">条件字段值</param>
        /// <param name="idname">条件字段名，默认是ID</param>
        /// <returns></returns>
        public static int UpdateById<T>(this IDbTransaction dbTrans, IDictionary<string, object> updateFields, object id, string idname = OrmLite.KeyName)
        {
            return Update<T>(dbTrans, updateFields, idname + "=@id", DictBuilder.Assign("id", id));
        }

        /// <summary>
        /// 根据一个字段修改另一个字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTrans"></param>
        /// <param name="fieldName">被修改的字段名</param>
        /// <param name="fieldValue">被修改的字段值</param>
        /// <param name="id">条件字段值</param>
        /// <param name="idname">条件字段名，默认是ID</param>
        /// <returns></returns>
        public static int UpdateFieldById<T>(this IDbTransaction dbTrans, string fieldName, object fieldValue, object id, string idname = OrmLite.KeyName)
        {
            return Update<T>(dbTrans, DictBuilder.Assign(fieldName, fieldValue), idname + "=@id", DictBuilder.Assign("id", id));
        }

        /// <summary>
        /// 根据sql语句删除数据
        /// </summary>
        /// <param name="dbTrans"></param>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public static int Delete(this IDbTransaction dbTrans, string sql, IDictionary<string, object> parameters = null)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            return ExecuteNonQuery(dbTrans, CommandType.Text, sql, provider.Dictionary2Params(parameters));
        }


        /// <summary>
        /// 根据字段删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTrans"></param>
        /// <param name="id">字段值</param>
        /// <param name="idField">字段名</param>
        /// <returns></returns>
        public static int DeleteById<T>(this IDbTransaction dbTrans, object id, string idField = OrmLite.KeyName)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.DeleteById<T>(id, idField);
            return ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 根据字段集合删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTrans"></param>
        /// <param name="idValues">字段值集合</param>
        /// <param name="idFields">字段名，默认是ID</param>
        /// <returns></returns>
        public static int DeleteByIds<T>(this IDbTransaction dbTrans, IEnumerable idValues, string idFields = OrmLite.KeyName)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.DeleteByIds<T>(idValues, idFields);
            if (string.IsNullOrEmpty(cmd.CommandText)) return 0;
            return ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        /// <summary>
        /// 删除所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbTrans"></param>
        /// <returns></returns>
        public static int Delete<T>(this IDbTransaction dbTrans)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Delete<T>();
            return ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static int DeleteWhere<T>(this IDbTransaction dbTrans, string name, object value)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.DeleteWhere<T>(name, value);
            return ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static int DeleteWhere<T>(this IDbTransaction dbTrans, object conditions)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.DeleteWhere<T>(conditions);
            return ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static int DeleteWhere<T>(this IDbTransaction dbTrans, IDictionary<string, object> conditions)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.DeleteWhere<T>(conditions);
            return ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static int Truncate<T>(this IDbTransaction dbTrans)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Truncate<T>();
            return ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

        public static int Truncate(this IDbTransaction dbTrans, string tableName)
        {
            var provider = dbTrans.GetCommandDialectProvider();
            var cmd = provider.Truncate(tableName);
            return ExecuteNonQuery(dbTrans, cmd.CommandType, cmd.CommandText, cmd.Params);
        }

    }
}
