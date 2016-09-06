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
            var theCmd = BaseCmd.GetCmd(dbConn.GetProviderType());

            if (inParams != null)
            {
                var ps = inParams is IDictionary<string, object> ?
                theCmd.DictionaryToParams(inParams as IDictionary<string, object>)
                : theCmd.AnonTypeToParams(inParams);
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
           
            return SqlHelper.ExecuteNonQuery(dbConn, commandType, commandText, ps);
        }
        public static int ExecuteNonQuery(this DbConnection dbConn, CommandType commandType, string commandText, IDictionary<string, object> parameters)
        {
           
            return SqlHelper.ExecuteNonQuery(dbConn, commandType, commandText, BaseCmd.GetCmd(dbConn.GetProviderType()).DictionaryToParams(parameters));
        }

        public static object ExecuteScalar(this DbConnection dbConn, CommandType commandType, string commandText, params DbParameter[] ps)
        {
           
            return SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
        }
        public static object ExecuteScalar(this DbConnection dbConn, CommandType commandType, string commandText, IDictionary<string, object> parameters)
        {
           
            return SqlHelper.ExecuteNonQuery(dbConn, commandType, commandText, BaseCmd.GetCmd(dbConn.GetProviderType()).DictionaryToParams(parameters));
        }

        public static int Insert<T>(this DbConnection dbConn, T obj, bool selectIdentity = false)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).Insert<T>(obj, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
                if (identity == null || identity is DBNull)
                {
                    return 0;
                }
                return Convert.ToInt32(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
                return raw;
            }
        }

        public static int Insert(this DbConnection dbConn, string table, IDictionary<string, object> fields, bool selectIdentity = false)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).Insert(table, fields, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
                if (identity == null || identity is DBNull)
                {
                    return 0;
                }
                return Convert.ToInt32(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
                return raw;
            }
        }

        public static int Insert(this DbConnection dbConn, string table, object anonType, bool selectIdentity = false)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).Insert(table, anonType, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
                if (identity == null || identity is DBNull)
                {
                    return 0;
                }
                return Convert.ToInt32(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
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
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).Update<T>(obj, updateFields);
            int c = ExecuteNonQuery(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
            return c;
        }

        public static int UpdateAnonymous(this DbConnection dbConn, string tableName, object anonymous)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).Update(tableName, anonymous);
            int c = ExecuteNonQuery(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
            return c;
        }
        public static int UpdateAnonymous<T>(this DbConnection dbConn, object anonymous)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).Update(ReflectionHelper.GetInfo<T>().TableName, anonymous);
            int c = ExecuteNonQuery(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
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

        public static int Update<T>(this DbConnection dbConn, IDictionary<string, object> updateFields, string conditions, IDictionary<string, object> parameters)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).Update(ReflectionHelper.GetInfo<T>().TableName, updateFields, conditions, parameters);

            int c = ExecuteNonQuery(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
            return c;
        }

        public static int Update(this DbConnection dbConn, string tableName, IDictionary<string, object> updateFields, string conditions, IDictionary<string, object> parameters)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).Update(tableName, updateFields, conditions, parameters);

            int c = ExecuteNonQuery(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
            return c;
        }




        public static int UpdateById(this DbConnection dbConn, string tableName, IDictionary<string, object> updateFields, object id, string idname = OrmLite.KeyName)
        {
            return Update(dbConn, tableName, updateFields, idname + "=@id", DictBuilder.Assign("id", id));
        }

        public static int UpdateById<T>(this DbConnection dbConn, IDictionary<string, object> updateFields, object id, string idname = OrmLite.KeyName)
        {
            return Update<T>(dbConn, updateFields, idname + "=@id", DictBuilder.Assign("id", id));
        }

        public static int UpdateFieldById<T>(this DbConnection dbConn, string fieldName, object fieldValue, object id, string idname = OrmLite.KeyName)
        {
            return Update<T>(dbConn, DictBuilder.Assign(fieldName, fieldValue), idname + "=@id", DictBuilder.Assign("id", id));
        }

        public static int Delete(this DbConnection dbConn, string sql, IDictionary<string, object> parameters = null)
        {
            return ExecuteNonQuery(dbConn, CommandType.Text, sql, BaseCmd.GetCmd(dbConn.GetProviderType()).DictionaryToParams(parameters));
        }

        public static int Delete<T>(this DbConnection dbConn, IDictionary<string, object> conditions)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).Delete<T>(conditions);
            return ExecuteNonQuery(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static int DeleteById<T>(this DbConnection dbConn, object id, string idField = OrmLite.KeyName)
        {
            var cmd = BaseCmd.GetCmd(dbConn.GetProviderType()).DeleteById<T>(id, idField);
            return ExecuteNonQuery(dbConn, CommandType.Text, cmd.CmdText, cmd.Params);
        }

        public static int DeleteByIds<T>(this DbConnection dbConn, IEnumerable idValues, string idFields = OrmLite.KeyName)
        {
            var sql = BaseCmd.GetCmd(dbConn.GetProviderType()).DeleteByIds<T>(idValues, idFields);
            if (sql == null || sql.Length == 0) return 0;
            return ExecuteNonQuery(dbConn, CommandType.Text, sql);
        }

        public static int Delete<T>(this DbConnection dbConn)
        {
            var sql = BaseCmd.GetCmd(dbConn.GetProviderType()).Delete<T>();
            return ExecuteNonQuery(dbConn, CommandType.Text, sql);
        }
    }
}
