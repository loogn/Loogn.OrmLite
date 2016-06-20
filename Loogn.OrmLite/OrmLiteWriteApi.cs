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
        public static void EnsureOpen(this DbConnection dbConn)
        {
            if (dbConn.State != ConnectionState.Open)
            {
                dbConn.Open();
            }
        }

        public static DbCommand Proc(this DbConnection dbConn, string name, object inParams = null, bool execute = false)
        {
            var cmd = dbConn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = name;
            var providerType = dbConn.GetProviderType();

            if (inParams != null)
            {
                var ps = inParams is Dictionary<string, object> ?
                ORM.DictionaryToParams(providerType, inParams as Dictionary<string, object>)
                : ORM.AnonTypeToParams(providerType, inParams);
                cmd.Parameters.AddRange(ps);
            }
            dbConn.Open();
            if (execute)
            {
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            return cmd;
        }

        public static int ExecuteNonQuery(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            return SqlHelper.ExecuteNonQuery(dbConn, commandType, commandText, ps);
        }
        public static int ExecuteNonQuery(this DbConnection dbConn, CommandType commandType, string commandText, Dictionary<string, object> parameters)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            return SqlHelper.ExecuteNonQuery(dbConn, commandType, commandText, ORM.DictionaryToParams(dbConn.GetProviderType(), parameters));
        }

        public static object ExecuteScalar(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            return SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
        }
        public static object ExecuteScalar(this DbConnection dbConn, CommandType commandType, string commandText, Dictionary<string, object> parameters)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            return SqlHelper.ExecuteNonQuery(dbConn, commandType, commandText, ORM.DictionaryToParams(dbConn.GetProviderType(), parameters));
        }

        public static int Insert<T>(this DbConnection dbConn, T obj, bool selectIdentity = false)
        {
            var tuple = SqlCmd.Insert<T>(dbConn.GetProviderType(), obj, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
                if (identity == null || identity is DBNull)
                {
                    return 0;
                }
                return Convert.ToInt32(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
                return raw;
            }
        }

        public static int Insert(this DbConnection dbConn, string table, Dictionary<string, object> fields, bool selectIdentity = false)
        {
            var tuple = SqlCmd.Insert(dbConn.GetProviderType(), table, fields, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
                if (identity == null || identity is DBNull)
                {
                    return 0;
                }
                return Convert.ToInt32(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
                return raw;
            }
        }

        public static int Insert(this DbConnection dbConn, string table, object anonType, bool selectIdentity = false)
        {
            var tuple = SqlCmd.Insert(dbConn.GetProviderType(), table, anonType, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
                if (identity == null || identity is DBNull)
                {
                    return 0;
                }
                return Convert.ToInt32(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
                return raw;
            }
        }


        public static bool Insert(this DbConnection dbConn, string table, params object[] objs)
        {
            return InsertAll(dbConn, table, objs);
        }
        public static bool InsertAll(this DbConnection dbConn, string table, IEnumerable objs)
        {
            if (objs != null)
            {
                dbConn.EnsureOpen();
                var trans = dbConn.BeginTransaction();
                try
                {
                    var providerType = dbConn.GetProviderType();
                    foreach (var obj in objs)
                    {
                        var rowCount = InsertTrans(trans, providerType, table, obj);
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

        public static bool Insert<T>(this DbConnection dbConn, params T[] objs)
        {
            return InsertAll<T>(dbConn, objs);
        }

        public static bool InsertAll<T>(this DbConnection dbConn, IEnumerable<T> objs)
        {
            if (objs != null)
            {
                dbConn.EnsureOpen();
                var trans = dbConn.BeginTransaction();
                try
                {
                    var providerType = dbConn.GetProviderType();
                    foreach (var obj in objs)
                    {
                        var rowCount = InsertTrans<T>(trans, providerType, obj);
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

        public static int Update<T>(this DbConnection dbConn, T obj, params string[] updateFields)
        {
            var tuple = SqlCmd.Update<T>(dbConn.GetProviderType(), obj, updateFields);
            int c = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
            return c;
        }

        public static int UpdateAnonymous(this DbConnection dbConn, string tableName, object anonymous)
        {
            var tuple = SqlCmd.Update(dbConn.GetProviderType(), tableName, anonymous);
            int c = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
            return c;
        }
        public static int UpdateAnonymous<T>(this DbConnection dbConn, object anonymous)
        {
            var tuple = SqlCmd.Update(dbConn.GetProviderType(), typeof(T).GetCachedTableName(), anonymous);
            int c = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
            return c;
        }

        public static int UpdateAnonymous(this DbConnection dbConn, object model, object anonymous)
        {
            var tuple = SqlCmd.Update(dbConn.GetProviderType(), model.GetType().GetCachedTableName(), anonymous);
            int c = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
            return c;
        }

        public static int Update<T>(this DbConnection dbConn, params T[] objs)
        {
            return UpdateAll<T>(dbConn, objs);
        }

        public static int UpdateAll<T>(this DbConnection dbConn, IEnumerable<T> objs)
        {
            int row = 0;
            if (objs != null)
            {
                dbConn.EnsureOpen();
                var trans = dbConn.BeginTransaction();
                try
                {
                    var providerType = dbConn.GetProviderType();
                    foreach (var obj in objs)
                    {
                        var rowCount = UpdateTrans<T>(trans, providerType, obj);
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

        public static int Update<T>(this DbConnection dbConn, Dictionary<string, object> updateFields, string conditions, Dictionary<string, object> parameters)
        {
            var tuple = SqlCmd.Update(dbConn.GetProviderType(), typeof(T).GetCachedTableName(), updateFields, conditions, parameters);

            int c = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
            return c;
        }

        public static int Update(this DbConnection dbConn, string tableName, Dictionary<string, object> updateFields, string conditions, Dictionary<string, object> parameters)
        {
            var tuple = SqlCmd.Update(dbConn.GetProviderType(), tableName, updateFields, conditions, parameters);

            int c = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
            return c;
        }




        public static int UpdateById(this DbConnection dbConn, string tableName, Dictionary<string, object> updateFields, object id, string idname = OrmLite.KeyName)
        {
            return Update(dbConn, tableName, updateFields, idname + "=@id", DictBuilder.Assign("id", id));
        }

        public static int UpdateById<T>(this DbConnection dbConn, Dictionary<string, object> updateFields, object id, string idname = OrmLite.KeyName)
        {
            return Update<T>(dbConn, updateFields, idname + "=@id", DictBuilder.Assign("id", id));
        }

        public static int UpdateFieldById<T>(this DbConnection dbConn, string fieldName, object fieldValue, object id, string idname = OrmLite.KeyName)
        {
            return Update<T>(dbConn, DictBuilder.Assign(fieldName, fieldValue), idname + "=@id", DictBuilder.Assign("id", id));
        }

        public static int Delete(this DbConnection dbConn, string sql, Dictionary<string, object> parameters = null)
        {
            return ExecuteNonQuery(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(dbConn.GetProviderType(), parameters));
        }

        public static int Delete<T>(this DbConnection dbConn, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.Delete<T>(dbConn.GetProviderType(), conditions);
            return ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static int DeleteById<T>(this DbConnection dbConn, object id, string idField = OrmLite.KeyName)
        {
            var tuple = SqlCmd.DeleteById<T>(dbConn.GetProviderType(), id, idField);
            return ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static int DeleteByIds<T>(this DbConnection dbConn, IEnumerable idValues, string idFields = OrmLite.KeyName)
        {
            var sql = SqlCmd.DeleteByIds<T>(dbConn.GetProviderType(), idValues, idFields);
            if (sql == null || sql.Length == 0) return 0;
            return ExecuteNonQuery(dbConn, CommandType.Text, sql);
        }

        public static int Delete<T>(this DbConnection dbConn)
        {
            var sql = SqlCmd.Delete<T>(dbConn.GetProviderType());
            return ExecuteNonQuery(dbConn, CommandType.Text, sql);
        }
    }
}
