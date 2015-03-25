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
        public static SqlCommand Proc(this SqlTransaction dbTrans, string name, object inParams = null, bool excludeDefaults = false)
        {
            var cmd = dbTrans.Connection.CreateCommand();
            cmd.Transaction = dbTrans;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = name;
            if (inParams != null)
            {
                var ps = ORM.AnonTypeToParams(inParams);
                cmd.Parameters.AddRange(ps);
            }
            if (excludeDefaults)
            {
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            return cmd;
        }

        public static int ExecuteNonQuery(this SqlTransaction dbTrans, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            return SqlHelper.ExecuteNonQuery(dbTrans, commandType, commandText, ps);
        }
        public static int ExecuteNonQuery(this SqlTransaction dbTrans, CommandType commandType, string commandText, Dictionary<string, object> parameters)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            return SqlHelper.ExecuteNonQuery(dbTrans, commandType, commandText, ORM.DictionaryToParams(parameters));
        }

        public static object ExecuteScalar(this SqlTransaction dbTrans, CommandType commandType, string commandText, params SqlParameter[] ps)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            return SqlHelper.ExecuteScalar(dbTrans, commandType, commandText, ps);
        }
        public static object ExecuteScalar(this SqlTransaction dbTrans, CommandType commandType, string commandText, Dictionary<string, object> parameters)
        {
            OrmLite.SetSqlStringBuilderCapacity(commandText);
            return SqlHelper.ExecuteNonQuery(dbTrans, commandType, commandText, ORM.DictionaryToParams(parameters));
        }

        public static long Insert<T>(this SqlTransaction dbTrans, T obj, bool selectIdentity = false)
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
                var fieldAttr = (OrmLiteFieldAttribute)property.GetCachedCustomAttributes(typeof(OrmLiteFieldAttribute), false).FirstOrDefault();
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
                var identity = ExecuteScalar(dbTrans, CommandType.Text, sbsql.ToString(), ps.ToArray());
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbTrans, CommandType.Text, sbsql.ToString(), ps.ToArray());
                return raw;
            }
        }

        public static long Insert(this SqlTransaction dbTrans, string table, Dictionary<string, object> fields, bool selectIdentity = false)
        {
            StringBuilder sbsql = new StringBuilder(100);
            sbsql.AppendFormat("insert into [{0}] (", table);
            StringBuilder sbParams = new StringBuilder(") values (");
            var ps = new List<SqlParameter>();
            foreach (var field in fields)
            {
                sbsql.AppendFormat("[{0}],", field.Key);
                sbParams.AppendFormat("@{0},", field.Key);
                ps.Add(new SqlParameter("@" + field.Key, field.Value));
            }
            if (ps.Count == 0)
            {
                throw new ArgumentException("fields里没有字段，无法插入");
            }
            sbsql.Remove(sbsql.Length - 1, 1);
            sbParams.Remove(sbParams.Length - 1, 1);
            sbsql.Append(sbParams.ToString());
            sbsql.Append(")");

            if (selectIdentity)
            {
                sbsql.Append(";SELECT ISNULL(SCOPE_IDENTITY(),@@rowcount)");
                var identity = ExecuteScalar(dbTrans, CommandType.Text, sbsql.ToString(), ps.ToArray());
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbTrans, CommandType.Text, sbsql.ToString(), ps.ToArray());
                return raw;
            }
        }

        public static long Insert(this SqlTransaction dbTrans, string table, object anonType, bool selectIdentity = false)
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
                var identity = ExecuteScalar(dbTrans, CommandType.Text, sbsql.ToString(), ps.ToArray());
                return Convert.ToInt64(identity);
            }
            else
            {
                var raw = ExecuteNonQuery(dbTrans, CommandType.Text, sbsql.ToString(), ps.ToArray());
                return raw;
            }
        }

        private static int InsertTrans<T>(this SqlTransaction dbTrans, T obj)
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
                var fieldAttr = (OrmLiteFieldAttribute)property.GetCachedCustomAttributes(typeof(OrmLiteFieldAttribute), false).FirstOrDefault();
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
            var raw = ExecuteNonQuery(dbTrans, CommandType.Text, sbsql.ToString(), ps.ToArray());
            return raw;
        }

        public static void Insert<T>(this SqlTransaction dbTrans, params T[] objs)
        {
            InsertAll<T>(dbTrans, objs);
        }

        public static void InsertAll<T>(this SqlTransaction dbTrans, IEnumerable<T> objs)
        {
            if (objs != null)
            {
                foreach (var obj in objs)
                {
                    var rowCount = InsertTrans<T>(dbTrans, obj);
                }
            }
        }

        public static int Update<T>(this SqlTransaction dbTrans, T obj, bool includeDefaults = false)
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
                var fieldAttr = (OrmLiteFieldAttribute)property.GetCachedCustomAttributes(typeof(OrmLiteFieldAttribute), false).FirstOrDefault();
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
                            if (property.PropertyType == typeof(string))
                            {
                                continue;
                            }
                        }
                        else
                        {
                            if (property.PropertyType.IsValueType && val.Equals(0))
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
            int c = ExecuteNonQuery(dbTrans, CommandType.Text, sbsql.ToString(), ps.ToArray());
            return c;
        }

        public static int Update<T>(this SqlTransaction dbTrans, T obj)
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
                var fieldAttr = (OrmLiteFieldAttribute)property.GetCachedCustomAttributes(typeof(OrmLiteFieldAttribute), false).FirstOrDefault();
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
            int c = ExecuteNonQuery(dbTrans, CommandType.Text, sbsql.ToString(), ps.ToArray());
            return c;
        }

        private static int UpdateTrans<T>(this SqlTransaction dbTrans, T obj)
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
                var fieldAttr = (OrmLiteFieldAttribute)property.GetCachedCustomAttributes(typeof(OrmLiteFieldAttribute), false).FirstOrDefault();
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
            int c = ExecuteNonQuery(dbTrans, CommandType.Text, sbsql.ToString(), ps.ToArray());
            return c;
        }

        public static int Update<T>(this SqlTransaction dbTrans, params T[] objs)
        {
            return UpdateAll<T>(dbTrans, objs);
        }

        public static int UpdateAll<T>(this SqlTransaction dbTrans, IEnumerable<T> objs)
        {
            int row = 0;
            foreach (var obj in objs)
            {
                var rowCount = UpdateTrans<T>(dbTrans, obj);
                row++;
            }
            return row;
        }

        public static int Update<T>(this SqlTransaction dbTrans, Dictionary<string, object> updateFields, string conditions, Dictionary<string, object> parameters)
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
            sbsql.AppendFormat(" where {0}", conditions);
            ps.AddRange(ORM.DictionaryToParams(parameters));
            int c = ExecuteNonQuery(dbTrans, CommandType.Text, sbsql.ToString(), ps.ToArray());
            return c;
        }

        public static int Delete(this SqlTransaction dbTrans, string sql, Dictionary<string, object> parameters = null)
        {
            return ExecuteNonQuery(dbTrans, CommandType.Text, sql, ORM.DictionaryToParams(parameters));
        }

        public static int Delete<T>(this SqlTransaction dbTrans, Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("DELETE FROM [{0}]", tableName);
            var ps = ORM.DictionaryToParams(conditions, sqlbuilder);
            return ExecuteNonQuery(dbTrans, CommandType.Text, sqlbuilder.ToString(), ps);
        }

        public static int DeleteById<T>(this SqlTransaction dbTrans, object id, string idField = "ID")
        {
            SqlParameter sp = new SqlParameter("@" + idField, id);
            return ExecuteNonQuery(dbTrans, CommandType.Text, string.Format("DELETE FROM [{0}] WHERE [{1}]=@{1}", typeof(T).GetCachedTableName(), idField), sp);
        }

        public static int DeleteByIds<T>(this SqlTransaction dbTrans, IEnumerable idValues)
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
                    sql.AppendFormat("DELETE from [{0}] where [{1}] in (", table, OrmLite.KeyFieldName);
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
                return ExecuteNonQuery(dbTrans, CommandType.Text, sql.ToString());
            }
        }

        public static int Delete<T>(this SqlTransaction dbTrans)
        {
            var table = typeof(T).GetCachedTableName();
            return ExecuteNonQuery(dbTrans, CommandType.Text, "DELETE  FROM [" + table + "]");
        }

    }
}
