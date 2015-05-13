using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    public static partial class OrmLiteWriteApi
    {
        public static void EnsureOpen(this SqlConnection dbConn)
        {
            if (dbConn.State != ConnectionState.Open)
            {
                dbConn.Open();
            }
        }

        public static SqlCommand Proc(this SqlConnection dbConn, string name, object inParams = null, bool excludeDefaults = false)
        {
            var cmd = dbConn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = name;
            if (inParams != null)
            {
                var ps = ORM.AnonTypeToParams(inParams);
                cmd.Parameters.AddRange(ps);
            }
            dbConn.Open();
            if (excludeDefaults)
            {
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            return cmd;
        }

        public static int ExecuteNonQuery(this SqlConnection dbConn, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            return SqlHelper.ExecuteNonQuery(dbConn, commandType, commandText, ps);
        }
        public static int ExecuteNonQuery(this SqlConnection dbConn, CommandType commandType, string commandText, Dictionary<string, object> parameters)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            return SqlHelper.ExecuteNonQuery(dbConn, commandType, commandText, ORM.DictionaryToParams(parameters));
        }

        public static object ExecuteScalar(this SqlConnection dbConn, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            return SqlHelper.ExecuteScalar(dbConn, commandType, commandText, ps);
        }
        public static object ExecuteScalar(this SqlConnection dbConn, CommandType commandType, string commandText, Dictionary<string, object> parameters)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            return SqlHelper.ExecuteNonQuery(dbConn, commandType, commandText, ORM.DictionaryToParams(parameters));
        }

        public static long Insert<T>(this SqlConnection dbConn, T obj, bool selectIdentity = false)
        {
            var tuple = SqlCmd.Insert<T>(obj, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
                return raw;
            }
        }

        public static long Insert(this SqlConnection dbConn, string table, Dictionary<string, object> fields, bool selectIdentity = false)
        {
            var tuple = SqlCmd.Insert(table, fields, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
                return raw;
            }
        }

        public static long Insert(this SqlConnection dbConn, string table, object anonType, bool selectIdentity = false)
        {
            var tuple = SqlCmd.Insert(table, anonType, selectIdentity);
            if (selectIdentity)
            {
                var identity = ExecuteScalar(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
                return raw;
            }
        }


        public static void Insert(this SqlConnection dbConn, string table, params object[] objs)
        {
            InsertAll(dbConn, table, objs);
        }
        public static void InsertAll(this SqlConnection dbConn, string table,  IEnumerable objs)
        {
            if (objs != null)
            {
                if (dbConn.State != ConnectionState.Open) dbConn.Open();
                var trans = dbConn.BeginTransaction();
                try
                {
                    foreach (var obj in objs)
                    {
                        var rowCount = InsertTrans(trans, table, obj);
                        if (rowCount == 0)
                        {
                            trans.Rollback();
                            break;
                        }
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
        }

        public static void Insert<T>(this SqlConnection dbConn, params T[] objs)
        {
            InsertAll<T>(dbConn, objs);
        }

        public static void InsertAll<T>(this SqlConnection dbConn, IEnumerable<T> objs)
        {
            if (objs != null)
            {
                if (dbConn.State != ConnectionState.Open) dbConn.Open();
                var trans = dbConn.BeginTransaction();
                try
                {
                    foreach (var obj in objs)
                    {
                        var rowCount = InsertTrans<T>(trans, obj);
                        if (rowCount == 0)
                        {
                            trans.Rollback();
                            break;
                        }
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
        }

        public static int Update<T>(this SqlConnection dbConn, T obj, bool includeDefaults = false)
        {
            var tuple = SqlCmd.Update<T>(obj, includeDefaults);
            int c = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
            return c;
        }

        public static int Update<T>(this SqlConnection dbConn, params T[] objs)
        {
            return UpdateAll<T>(dbConn, objs);
        }

        public static int UpdateAll<T>(this SqlConnection dbConn, IEnumerable<T> objs)
        {
            int row = 0;
            if (objs != null)
            {
                dbConn.Open();
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

        public static int Update<T>(this SqlConnection dbConn, Dictionary<string, object> updateFields, string conditions, Dictionary<string, object> parameters)
        {
            var tuple = SqlCmd.Update<T>(updateFields, conditions, parameters);

            int c = ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
            return c;
        }

        public static int Delete(this SqlConnection dbConn, string sql, Dictionary<string, object> parameters = null)
        {
            return ExecuteNonQuery(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }

        public static int Delete<T>(this SqlConnection dbConn, Dictionary<string, object> conditions)
        {
            var tuple = SqlCmd.Delete<T>(conditions);
            return ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static int DeleteById<T>(this SqlConnection dbConn, object id, string idField = OrmLite.KeyName)
        {
            var tuple = SqlCmd.DeleteById<T>(id, idField);
            return ExecuteNonQuery(dbConn, CommandType.Text, tuple.Item1, tuple.Item2);
        }

        public static int DeleteByIds<T>(this SqlConnection dbConn, IEnumerable idValues, string idFields = OrmLite.KeyName)
        {
            var sql = SqlCmd.DeleteByIds<T>(idValues, idFields);
            if (sql == null || sql.Length == 0) return 0;
            return ExecuteNonQuery(dbConn, CommandType.Text, sql);
        }

        public static int Delete<T>(this SqlConnection dbConn)
        {
            var sql = SqlCmd.Delete<T>();
            return ExecuteNonQuery(dbConn, CommandType.Text, sql);
        }
    }
}
