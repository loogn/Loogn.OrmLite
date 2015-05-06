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
            if (excludeDefaults)
            {
                dbConn.Open();
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            dbConn.Open();
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
            var type = typeof(T);
            var table = type.GetCachedTableName();
            var propertys = type.GetCachedProperties();

            StringBuilder sbsql = new StringBuilder(100);
            sbsql.AppendFormat("insert into [{0}] (", table);
            StringBuilder sbParams = new StringBuilder(") values (");
            var ps = new List<SqlParameter>();
            foreach (var property in propertys)
            {
                var fieldAttr = (OrmLiteFieldAttribute)property.GetCachedCustomAttributes(typeof(OrmLiteFieldAttribute)).FirstOrDefault();
                if (property.Name.Equals("ID", StringComparison.OrdinalIgnoreCase))
                {
                    if (fieldAttr == null || (!fieldAttr.InsertRequire))
                    {
                        continue;
                    }
                }

                if (fieldAttr == null || (!fieldAttr.InsertIgnore && !fieldAttr.Ignore))
                {
                    var fieldName = property.Name;

                    if (fieldAttr != null && fieldAttr.Name != null && fieldAttr.Name.Length > 0)
                    {
                        fieldName = fieldAttr.Name;
                    }
                    var val = property.GetValue(obj, null);
                    if (val == null)
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            val = string.Empty;
                        }
                    }
                    else
                    {
                        if (property.PropertyType == typeof(DateTime) && (DateTime)val == DateTime.MinValue)
                        {
                            val = DateTime.Now;
                        }
                    }
                    sbsql.AppendFormat("[{0}],", fieldName);
                    sbParams.AppendFormat("@{0},", fieldName);
                    ps.Add(new SqlParameter("@" + fieldName, val ?? DBNull.Value));
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

            if (selectIdentity)
            {
                sbsql.Append(";SELECT ISNULL(SCOPE_IDENTITY(),@@rowcount)");
                var identity = ExecuteScalar(dbConn, CommandType.Text, sbsql.ToString(), ps.ToArray());
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn, CommandType.Text, sbsql.ToString(), ps.ToArray());
                return raw;
            }
        }

        public static long Insert(this SqlConnection dbConn, string table, object anonType, bool selectIdentity = false)
        {
            var type = anonType.GetType();
            var propertys = type.GetCachedProperties();

            StringBuilder sbsql = new StringBuilder(100);
            sbsql.AppendFormat("insert into [{0}] (", table);
            StringBuilder sbParams = new StringBuilder(") values (");
            var ps = new List<SqlParameter>();
            foreach (var property in propertys)
            {
                var fieldName = property.Name;
                var val = property.GetValue(anonType, null);
                sbsql.AppendFormat("[{0}],", fieldName);
                sbParams.AppendFormat("@{0},", fieldName);
                ps.Add(new SqlParameter("@" + fieldName, val ?? DBNull.Value));
            }
            if (ps.Count == 0)
            {
                throw new ArgumentException("model里没有字段，无法插入");
            }
            sbsql.Remove(sbsql.Length - 1, 1);
            sbParams.Remove(sbParams.Length - 1, 1);
            sbsql.Append(sbParams.ToString());
            sbsql.Append(")");

            if (selectIdentity)
            {
                sbsql.Append(";SELECT ISNULL(SCOPE_IDENTITY(),@@rowcount)");
                var identity = ExecuteScalar(dbConn, CommandType.Text, sbsql.ToString(), ps.ToArray());
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbConn, CommandType.Text, sbsql.ToString(), ps.ToArray());
                return raw;
            }
        }

        private static int InsertTrans<T>(this SqlConnection dbConn, T obj, SqlTransaction trans)
        {
            var type = typeof(T);
            var table = type.GetCachedTableName();
            var propertys = type.GetCachedProperties();

            StringBuilder sbsql = new StringBuilder(100);
            sbsql.AppendFormat("insert into [{0}] (", table);
            StringBuilder sbParams = new StringBuilder(") values (");
            var ps = new List<SqlParameter>();
            foreach (var property in propertys)
            {
                var fieldAttr = (OrmLiteFieldAttribute)property.GetCachedCustomAttributes(typeof(OrmLiteFieldAttribute)).FirstOrDefault();
                if (property.Name.Equals("ID", StringComparison.OrdinalIgnoreCase))
                {
                    if (fieldAttr == null || (!fieldAttr.InsertRequire))
                    {
                        continue;
                    }
                }
                if (fieldAttr == null || (!fieldAttr.InsertIgnore && !fieldAttr.Ignore))
                {
                    var fieldName = property.Name;

                    if (fieldAttr != null && fieldAttr.Name != null && fieldAttr.Name.Length > 0)
                    {
                        fieldName = fieldAttr.Name;
                    }
                    var val = property.GetValue(obj, null);
                    if (val == null)
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            val = string.Empty;
                        }
                    }
                    else
                    {
                        if (property.PropertyType == typeof(DateTime) && (DateTime)val == DateTime.MinValue)
                        {
                            //val = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                            val = DateTime.Now;
                        }
                    }
                    sbsql.AppendFormat("[{0}],", fieldName);
                    sbParams.AppendFormat("@{0},", fieldName);
                    ps.Add(new SqlParameter("@" + fieldName, val ?? DBNull.Value));
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
            var raw = ExecuteNonQuery(trans, CommandType.Text, sbsql.ToString(), ps.ToArray());
            return raw;
        }

        private static int InsertTrans(this SqlConnection dbConn, string table, object anonType, SqlTransaction trans)
        {
            var type = anonType.GetType();
            var propertys = type.GetCachedProperties();
            StringBuilder sbsql = new StringBuilder(100);
            sbsql.AppendFormat("insert into [{0}] (", table);
            StringBuilder sbParams = new StringBuilder(") values (");
            var ps = new List<SqlParameter>();
            foreach (var property in propertys)
            {
                var fieldName = property.Name;
                var val = property.GetValue(anonType, null);
                sbsql.AppendFormat("[{0}],", fieldName);
                sbParams.AppendFormat("@{0},", fieldName);
                ps.Add(new SqlParameter("@" + fieldName, val ?? DBNull.Value));
            }
            if (ps.Count == 0)
            {
                throw new ArgumentException("model里没有字段，无法插入");
            }
            sbsql.Remove(sbsql.Length - 1, 1);
            sbParams.Remove(sbParams.Length - 1, 1);
            sbsql.Append(sbParams.ToString());
            sbsql.Append(")");
            var raw = ExecuteNonQuery(trans, CommandType.Text, sbsql.ToString(), ps.ToArray());
            return raw;
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
                        var rowCount = InsertTrans(dbConn, table, obj, trans);
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
                }
                finally
                {
                    trans.Dispose();
                }
            }
        }

        public static void Insert(this SqlConnection dbConn,string table, params object[] objs)
        {
            InsertAll(dbConn, table, objs);
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
                        var rowCount = InsertTrans<T>(dbConn, obj, trans);
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

        public static int Update<T>(this SqlConnection dbConn, T obj, bool includeDefaults = false)
        {
            var type = typeof(T);
            var table = type.GetCachedTableName();
            var propertys = type.GetCachedProperties();
            StringBuilder sbsql = new StringBuilder(100);
            sbsql.AppendFormat("update [{0}] set ", table);
            string condition = null;
            var ps = new List<SqlParameter>();
            foreach (var property in propertys)
            {
                var fieldAttr = (OrmLiteFieldAttribute)property.GetCachedCustomAttributes(typeof(OrmLiteFieldAttribute)).FirstOrDefault();
                if (fieldAttr == null || (!fieldAttr.UpdateIgnore && !fieldAttr.Ignore))
                {
                    var val = property.GetValue(obj, null);
                    if (includeDefaults) //需要初始化
                    {
                        if (val == null)
                        {
                            if (property.PropertyType == typeof(string))
                            {
                                val = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        if (val == null)
                        {
                            //string nullable
                            continue;   
                        }
                        else
                        {
                            if (property.PropertyType == typeof(byte) ||
                                property.PropertyType == typeof(int) ||
                                property.PropertyType == typeof(long) ||
                                property.PropertyType == typeof(float) ||
                                property.PropertyType == typeof(double) ||
                                property.PropertyType == typeof(Decimal))
                            {
                                if (Convert.ToInt32(val) == 0)
                                {
                                    continue;
                                }
                            }
                            else if (property.PropertyType == typeof(DateTime) && (DateTime)val == DateTime.MinValue)
                            {
                                continue;
                            }
                        }
                    }
                    var fieldName = property.Name;
                    if (fieldAttr != null && fieldAttr.Name != null && fieldAttr.Name.Length > 0)
                    {
                        fieldName = fieldAttr.Name;
                    }
                    if (fieldName.Equals("ID", StringComparison.OrdinalIgnoreCase) || (fieldAttr != null && fieldAttr.IsPrimaryKey))
                    {
                        condition = string.Format("[{0}] = @{0}", fieldName);
                    }
                    else
                    {
                        sbsql.AppendFormat("[{0}] = @{0},", fieldName);
                    }
                    ps.Add(new SqlParameter("@" + fieldName, val ?? DBNull.Value));
                }
            }
            if (ps.Count == 0)
            {
                throw new ArgumentException("model里没有字段，无法修改");
            }
            sbsql.Remove(sbsql.Length - 1, 1);
            sbsql.AppendFormat(" where ");
            sbsql.Append(condition);
            int c = ExecuteNonQuery(dbConn, CommandType.Text, sbsql.ToString(), ps.ToArray());
            return c;
        }

        private static int UpdateTrans<T>(this SqlConnection dbConn, T obj, SqlTransaction trans)
        {
            var type = typeof(T);
            var table = type.GetCachedTableName();
            var propertys = type.GetCachedProperties();
            StringBuilder sbsql = new StringBuilder(100);
            sbsql.AppendFormat("update [{0}] set ", table);
            string condition = null;
            var ps = new List<SqlParameter>();
            foreach (var property in propertys)
            {
                var fieldAttr = (OrmLiteFieldAttribute)property.GetCachedCustomAttributes(typeof(OrmLiteFieldAttribute)).FirstOrDefault();
                if (fieldAttr == null || (!fieldAttr.UpdateIgnore && !fieldAttr.Ignore))
                {
                    var fieldName = property.Name;
                    if (fieldAttr != null && fieldAttr.Name != null && fieldAttr.Name.Length > 0)
                    {
                        fieldName = fieldAttr.Name;
                    }

                    var val = property.GetValue(obj, null);
                    if (val == null)
                    {
                        if (property.PropertyType == typeof(string))
                        {
                            continue;
                        }
                    }
                    if (property.PropertyType == typeof(DateTime) && (DateTime)val == DateTime.MinValue)
                    {
                        continue;
                    }
                    if (fieldName.Equals("ID", StringComparison.OrdinalIgnoreCase) || (fieldAttr != null && fieldAttr.IsPrimaryKey))
                    {
                        condition = string.Format("[{0}] = @{0}", fieldName);
                    }
                    else
                    {
                        sbsql.AppendFormat("[{0}] = @{0},", fieldName);
                    }
                    ps.Add(new SqlParameter("@" + fieldName, val ?? DBNull.Value));
                }
            }
            if (ps.Count == 0)
            {
                throw new ArgumentException("model里没有字段，无法修改");
            }
            sbsql.Remove(sbsql.Length - 1, 1);
            sbsql.AppendFormat(" where ");
            sbsql.Append(condition);
            int c = ExecuteNonQuery(trans, CommandType.Text, sbsql.ToString(), ps.ToArray());
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
                        var rowCount = UpdateTrans<T>(dbConn, obj, trans);
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
                    Console.WriteLine(exp.Message);
                    row = 0;
                    trans.Rollback();
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
            var table = typeof(T).GetCachedTableName();
            StringBuilder sbsql = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            sbsql.AppendFormat("update [{0}] set ", table);
            var ps = new List<SqlParameter>();
            var nofield = true;
            foreach (var field in updateFields)
            {
                if (field.Key.StartsWith("$") || field.Key.StartsWith("_"))
                {
                    sbsql.AppendFormat("[{0}] = {1},", field.Key.Substring(1), field.Value);
                    nofield = false;
                }
                else
                {
                    sbsql.AppendFormat("[{0}] = @_{0},", field.Key);
                    ps.Add(new SqlParameter("@_" + field.Key, field.Value));
                }
            }
            if (ps.Count == 0 && nofield)
            {
                throw new ArgumentException("updateFields里没有字段，无法修改");
            }
            sbsql.Remove(sbsql.Length - 1, 1);
            if (conditions != null && conditions.Length > 0)
            {
                sbsql.AppendFormat(" where {0}", conditions);
            }
            var conditionPS = ORM.DictionaryToParams(parameters);
            if (conditionPS != null)
            {
                ps.AddRange(conditionPS);
            }
            int c = ExecuteNonQuery(dbConn, CommandType.Text, sbsql.ToString(), ps.ToArray());
            return c;
        }

        public static int Delete(this SqlConnection dbConn, string sql, Dictionary<string, object> parameters = null)
        {
            return ExecuteNonQuery(dbConn, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }

        public static int Delete<T>(this SqlConnection dbConn, Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("DELETE FROM [{0}]", tableName);
            var ps = ORM.DictionaryToParams(conditions, sqlbuilder);
            return ExecuteNonQuery(dbConn, CommandType.Text, sqlbuilder.ToString(), ps);
        }

        public static int DeleteById<T>(this SqlConnection dbConn, object id, string idField = "ID")
        {
            SqlParameter sp = new SqlParameter("@" + idField, id);
            return ExecuteNonQuery(dbConn, CommandType.Text, string.Format("DELETE FROM [{0}] WHERE [{1}]=@{1}", typeof(T).GetCachedTableName(), idField), sp);
        }

        public static int DeleteByIds<T>(this SqlConnection dbConn, IEnumerable idValues,string idFields="ID")
        {
            if (idValues == null) return 0;
            bool any = false;
            var needQuot = false;
            StringBuilder sql = null;
            var enumerator = idValues.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!any)
                {
                    any = true;
                    var idType = enumerator.Current.GetType();
                    if (idType == typeof(string) || idType == typeof(DateTime))
                    {
                        needQuot = true;
                    }
                    var table = typeof(T).GetCachedTableName();
                    sql = new StringBuilder(50);
                    sql.AppendFormat("DELETE from [{0}] where [{1}] in (", table, idFields);
                }
                if (needQuot)
                {
                    sql.AppendFormat("'{0}',", enumerator.Current);
                }
                else
                {
                    sql.AppendFormat("{0},", enumerator.Current);
                }
            }
            if (!any)
            {
                return 0;
            }
            else
            {
                sql.Replace(',', ')', sql.Length - 1, 1);
                return ExecuteNonQuery(dbConn, CommandType.Text, sql.ToString());
            }
        }

        public static int Delete<T>(this SqlConnection dbConn)
        {
            var table = typeof(T).GetCachedTableName();
            return ExecuteNonQuery(dbConn, CommandType.Text, "DELETE  FROM [" + table + "]");
        }
    }
}
