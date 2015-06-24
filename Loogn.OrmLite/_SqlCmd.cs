using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    static class SqlCmd
    {
        public static string Select<T>()
        {
            var table = typeof(T).GetCachedTableName();
            return "SELECT * FROM [" + table + "]";
        }

        public static string FullPartSql<T>(string sql, PartSqlType type)
        {
            sql = sql.TrimStart();
            if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return sql;
            }
            var tableName = typeof(T).GetCachedTableName();
            StringBuilder sb = new StringBuilder(sql.Length + 50);
            switch (type)
            {
                case PartSqlType.Select:
                    return sb.AppendFormat("SELECT * FROM [{0}] where {1}", tableName, sql).ToString();
                case PartSqlType.Single:
                    return sb.AppendFormat("SELECT TOP 1 * FROM [{0}] where {1}", tableName, sql).ToString();
                case PartSqlType.Count:
                    return sb.AppendFormat("SELECT COUNT(0) FROM [{0}] where {1}", tableName, sql).ToString();
                default:
                    return sql;
            }
        }

        public static MyTuple<string, SqlParameter[]> SelectWhere<T>(string name, object value)
        {
            var table = typeof(T).GetCachedTableName();
            SqlParameter p = new SqlParameter("@" + name, value);

            MyTuple<string, SqlParameter[]> result = new MyTuple<string, SqlParameter[]>();
            result.Item1 = string.Format("Select * from [{0}] where [{1}]=@{1}", table, name);
            result.Item2 = new SqlParameter[] { p };
            return result;
        }

        public static MyTuple<string, SqlParameter[]> SelectWhere<T>(Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT * FROM [{0}]", tableName);
            var ps = ORM.DictionaryToParams(conditions, sqlbuilder);

            MyTuple<string, SqlParameter[]> result = new MyTuple<string, SqlParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static MyTuple<string, SqlParameter[]> SelectWhere<T>(object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT * FROM [{0}]", tableName);
            var ps = ORM.AnonTypeToParams(conditions, sqlbuilder);

            MyTuple<string, SqlParameter[]> result = new MyTuple<string, SqlParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static string SelectByIds<T>(IEnumerable idValues, string idField)
        {
            if (idValues == null) return null;
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
                    sql.AppendFormat("Select * from [{0}] where [{1}] in (", table, idField);
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
                return null;
            }
            else
            {
                sql.Replace(',', ')', sql.Length - 1, 1);
                return sql.ToString();
            }
        }

        public static MyTuple<string, SqlParameter[]> Single<T>(Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT  TOP 1 * FROM [{0}]", tableName);
            var ps = ORM.DictionaryToParams(conditions, sqlbuilder);

            var result = new MyTuple<string, SqlParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static MyTuple<string, SqlParameter[]> Single<T>(object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT  TOP 1 * FROM [{0}]", tableName);
            var ps = ORM.AnonTypeToParams(conditions, sqlbuilder);

            var result = new MyTuple<string, SqlParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static MyTuple<string, SqlParameter[]> SingleById<T>(object idValue, string idField)
        {
            SqlParameter sp = new SqlParameter("@" + idField, idValue);
            var sql = string.Format("SELECT TOP 1 * FROM [{0}] WHERE [{1}]=@{1}", typeof(T).GetCachedTableName(), idField);
            MyTuple<string, SqlParameter[]> result = new MyTuple<string, SqlParameter[]>();
            result.Item1 = sql;
            result.Item2 = new SqlParameter[] { sp };
            return result;
        }

        public static MyTuple<string, SqlParameter[]> SingleWhere<T>(string name, object value)
        {
            var table = typeof(T).GetCachedTableName();
            SqlParameter p = new SqlParameter("@" + name, value);
            var sql = string.Format("SELECT TOP 1 * FROM [{0}] WHERE [{1}]=@{1}", table, name);

            MyTuple<string, SqlParameter[]> result = new MyTuple<string, SqlParameter[]>();
            result.Item1 = sql;
            result.Item2 = new SqlParameter[] { p };
            return result;
        }

        public static MyTuple<string, SqlParameter[]> SingleWhere<T>(Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT TOP 1 * FROM [{0}]", tableName);
            var ps = ORM.DictionaryToParams(conditions, sqlbuilder);

            MyTuple<string, SqlParameter[]> result = new MyTuple<string, SqlParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static MyTuple<string, SqlParameter[]> SingleWhere<T>(object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT TOP 1 * FROM [{0}]", tableName);
            var ps = ORM.AnonTypeToParams(conditions, sqlbuilder);

            MyTuple<string, SqlParameter[]> result = new MyTuple<string, SqlParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static string Count<T>()
        {
            var table = typeof(T).GetCachedTableName();
            return "SELECT COUNT(0) FROM [" + table + "]";
        }

        public static MyTuple<string, SqlParameter[]> CountWhere<T>(string name, object value)
        {
            var table = typeof(T).GetCachedTableName();
            SqlParameter p = new SqlParameter("@" + name, value);
            var sql = string.Format("SELECT COUNT(0) FROM [{0}] WHERE [{1}]=@{1}", table, name);
            MyTuple<string, SqlParameter[]> result = new MyTuple<string, SqlParameter[]>();
            result.Item1 = sql;
            result.Item2 = new SqlParameter[] { p };
            return result;
        }
        public static MyTuple<string, SqlParameter[]> CountWhere<T>(Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT COUNT(0) FROM [{0}]", tableName);
            var ps = ORM.DictionaryToParams(conditions, sqlbuilder);

            MyTuple<string, SqlParameter[]> result = new MyTuple<string, SqlParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static MyTuple<string, SqlParameter[]> CountWhere<T>(object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT COUNT(0) FROM [{0}]", tableName);
            var ps = ORM.AnonTypeToParams(conditions, sqlbuilder);

            MyTuple<string, SqlParameter[]> result = new MyTuple<string, SqlParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }


        public static MyTuple<string, SqlParameter[]> Update<T>(Dictionary<string, object> updateFields, string conditions, Dictionary<string, object> parameters)
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
            return new MyTuple<string, SqlParameter[]>
            {
                Item1 = sbsql.ToString(),
                Item2 = ps.ToArray()
            };

        }


        public static MyTuple<string, SqlParameter[]> Insert<T>(T obj, bool selectIdentity)
        {
            var type = typeof(T);
            var table = type.GetCachedTableName();
            var propertys = type.GetCachedProperties();

            StringBuilder sbsql = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            sbsql.AppendFormat("insert into [{0}] (", table);
            StringBuilder sbParams = new StringBuilder(") values (", OrmLite.SqlStringBuilderCapacity);
            var ps = new List<SqlParameter>();
            foreach (var property in propertys)
            {
                var fieldAttr = (OrmLiteFieldAttribute)property.GetCachedCustomAttributes(typeof(OrmLiteFieldAttribute)).FirstOrDefault();
                if (property.Name.Equals(OrmLite.DefaultKeyName, StringComparison.OrdinalIgnoreCase))
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
            }

            return new MyTuple<string, SqlParameter[]>
            {
                Item1 = sbsql.ToString(),
                Item2 = ps.ToArray()
            };
        }

        public static MyTuple<string, SqlParameter[]> Insert(string table, Dictionary<string, object> fields, bool selectIdentity = false)
        {
            StringBuilder sbsql = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            sbsql.AppendFormat("insert into [{0}] (", table);
            StringBuilder sbParams = new StringBuilder(") values (", OrmLite.SqlStringBuilderCapacity);
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
            }
            return new MyTuple<string, SqlParameter[]>
            {
                Item1 = sbsql.ToString(),
                Item2 = ps.ToArray()
            };
        }

        public static MyTuple<string, SqlParameter[]> Insert(string table, object anonType, bool selectIdentity)
        {
            var type = anonType.GetType();
            var propertys = type.GetCachedProperties();

            StringBuilder sbsql = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            sbsql.AppendFormat("insert into [{0}] (", table);
            StringBuilder sbParams = new StringBuilder(") values (", OrmLite.SqlStringBuilderCapacity);
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
            }
            return new MyTuple<string, SqlParameter[]>
            {
                Item1 = sbsql.ToString(),
                Item2 = ps.ToArray()
            };
        }

        public static MyTuple<string, SqlParameter[]> Update<T>(T obj, params string[] updateFields)
        {
            var type = typeof(T);
            var table = type.GetCachedTableName();
            var propertys = type.GetCachedProperties();
            StringBuilder sbsql = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            sbsql.AppendFormat("update [{0}] set ", table);
            string condition = null;
            var ps = new List<SqlParameter>();
            foreach (var property in propertys)
            {
                var fieldName = property.Name;
                if (FieldsIgnore(OrmLite.UpdateIgnoreFields, fieldName))
                {
                    continue;
                }
                var fieldAttr = (OrmLiteFieldAttribute)property.GetCachedCustomAttributes(typeof(OrmLiteFieldAttribute)).FirstOrDefault();
                if (fieldAttr == null || (!fieldAttr.UpdateIgnore && !fieldAttr.Ignore))
                {
                    if (fieldAttr != null && fieldAttr.Name != null && fieldAttr.Name.Length > 0)
                    {
                        fieldName = fieldAttr.Name;
                    }
                    if (fieldName.Equals(OrmLite.DefaultKeyName, StringComparison.OrdinalIgnoreCase) || (fieldAttr != null && fieldAttr.IsPrimaryKey))
                    {
                        condition = string.Format("[{0}] = @{0}", fieldName);
                        var val = property.GetValue(obj, null);
                        ps.Add(new SqlParameter("@" + fieldName, val ?? DBNull.Value));
                    }
                    else
                    {
                        if (FieldsContains(updateFields, fieldName))
                        {
                            sbsql.AppendFormat("[{0}] = @{0},", fieldName);
                            var val = property.GetValue(obj, null);
                            ps.Add(new SqlParameter("@" + fieldName, val ?? DBNull.Value));
                        }
                    }
                }
            }
            if (ps.Count == 0)
            {
                throw new ArgumentException("model里没有字段，无法修改");
            }
            sbsql.Remove(sbsql.Length - 1, 1);
            sbsql.AppendFormat(" where ");
            sbsql.Append(condition);
            return new MyTuple<string, SqlParameter[]>
            {
                Item1 = sbsql.ToString(),
                Item2 = ps.ToArray()
            };
        }
        private static bool FieldsContains(string[] fields, string value)
        {
            if (fields == null || fields.Length == 0) return true;
            foreach (var item in fields)
            {
                if (item.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        private static bool FieldsIgnore(List<string> fields, string value)
        {
            if (fields == null || fields.Count == 0) return false;
            foreach (var item in fields)
            {
                if (item.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static MyTuple<string, SqlParameter[]> Delete<T>(Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(200);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("DELETE FROM [{0}]", tableName);
            var ps = ORM.DictionaryToParams(conditions, sqlbuilder);
            return new MyTuple<string, SqlParameter[]>
            {
                Item1 = sqlbuilder.ToString(),
                Item2 = ps
            };
        }

        public static MyTuple<string, SqlParameter[]> DeleteById<T>(object id, string idField)
        {
            SqlParameter sp = new SqlParameter("@" + idField, id);
            var sql = string.Format("DELETE FROM [{0}] WHERE [{1}]=@{1}", typeof(T).GetCachedTableName(), idField);
            return new MyTuple<string, SqlParameter[]>
            {
                Item1 = sql,
                Item2 = new SqlParameter[] { sp }
            };
        }

        public static string DeleteByIds<T>(IEnumerable idValues, string idField)
        {
            if (idValues == null) return null;
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
                    sql = new StringBuilder(200);
                    sql.AppendFormat("DELETE from [{0}] where [{1}] in (", table, idField);
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
                return null;
            }
            else
            {
                sql.Replace(',', ')', sql.Length - 1, 1);
                return sql.ToString();
            }
        }

        public static string Delete<T>()
        {
            var table = typeof(T).GetCachedTableName();
            return "DELETE  FROM [" + table + "]";
        }
    }
}
