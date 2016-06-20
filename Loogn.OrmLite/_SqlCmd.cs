using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace Loogn.OrmLite
{
    static class SqlCmd
    {
        public static string L(OrmLiteProviderType providerType)
        {
            if (providerType == OrmLiteProviderType.MySql)
            {
                return "`";
            }
            else if (providerType == OrmLiteProviderType.SqlServer)
            {
                return "[";
            }
            return "";
        }
        public static string R(OrmLiteProviderType providerType)
        {
            if (providerType == OrmLiteProviderType.MySql)
            {
                return "`";
            }
            else if (providerType == OrmLiteProviderType.SqlServer)
            {
                return "]";
            }
            return "";
        }

        public static string Select<T>(OrmLiteProviderType type)
        {
            var table = typeof(T).GetCachedTableName();
            return "SELECT * FROM " + L(type) + table + R(type);
        }

        public static string FullPartSql<T>(OrmLiteProviderType providerType, string sql, PartSqlType type)
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
                    return sb.AppendFormat("SELECT * FROM {2}{0}{3} where {1}", tableName, sql, L(providerType), R(providerType)).ToString();
                case PartSqlType.Count:
                    return sb.AppendFormat("SELECT COUNT(0) FROM {2}{0}{3} where {1}", tableName, sql, L(providerType), R(providerType)).ToString();
                default:
                    return sql;
            }
        }
        public static string FullPartSqlSingle<T>(OrmLiteProviderType type, string sql)
        {
            sql = sql.TrimStart();
            if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return sql;
            }
            var tableName = typeof(T).GetCachedTableName();
            StringBuilder sb = new StringBuilder(sql.Length + 50);
            if (type == OrmLiteProviderType.MySql)
            {
                return sb.AppendFormat("SELECT * FROM `{0}` where {1} limit 1", tableName, sql).ToString();
            }
            else
            {
                return sb.AppendFormat("SELECT top 1 * FROM [{0}] where {1}", tableName, sql).ToString();
            }
        }



        public static MyTuple<string, DbParameter[]> SelectWhere<T>(OrmLiteProviderType type, string name, object value)
        {
            var table = typeof(T).GetCachedTableName();
            DbParameter p = OrmLite.GetProvider(type).CreateParameter("@" + name, value);


            MyTuple<string, DbParameter[]> result = new MyTuple<string, DbParameter[]>();
            result.Item1 = string.Format("Select * from {2}{0}{3} where {2}{1}{3}=@{1}", table, name, L(type), R(type));
            result.Item2 = new DbParameter[] { p };
            return result;
        }

        public static MyTuple<string, DbParameter[]> SelectWhere<T>(OrmLiteProviderType type, Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT * FROM {1}{0}{2}", tableName, L(type), R(type));
            var ps = ORM.DictionaryToParams(type, conditions, sqlbuilder);

            MyTuple<string, DbParameter[]> result = new MyTuple<string, DbParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static MyTuple<string, DbParameter[]> SelectWhere<T>(OrmLiteProviderType type, object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT * FROM {1}{0}{2}", tableName, L(type), R(type));
            var ps = ORM.AnonTypeToParams(type, conditions, sqlbuilder);

            MyTuple<string, DbParameter[]> result = new MyTuple<string, DbParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static string SelectByIds<T>(OrmLiteProviderType providerType, IEnumerable idValues, string idField, string selectFields = "*")
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
                    sql.AppendFormat("Select {2} from {3}{0}{4} where {3}{1}{4} in (", table, idField, selectFields, L(providerType), R(providerType));
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

        public static MyTuple<string, DbParameter[]> Single<T>(OrmLiteProviderType type, Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            DbParameter[] ps = null;
            if (type == OrmLiteProviderType.MySql)
            {
                sqlbuilder.AppendFormat("SELECT * FROM `{0}`", tableName);
                ps = ORM.DictionaryToParams(type, conditions, sqlbuilder);
                sqlbuilder.Append(" limit 1");
            }
            else if (type == OrmLiteProviderType.SqlServer)
            {
                sqlbuilder.AppendFormat("SELECT top 1 * FROM [{0}]", tableName);
                ps = ORM.DictionaryToParams(type, conditions, sqlbuilder);
            }

            var result = new MyTuple<string, DbParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static MyTuple<string, DbParameter[]> Single<T>(OrmLiteProviderType type, object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            DbParameter[] ps = null;
            if (type == OrmLiteProviderType.MySql)
            {
                sqlbuilder.AppendFormat("SELECT * FROM `{0}`", tableName);
                ps = ORM.AnonTypeToParams(type, conditions, sqlbuilder);
                sqlbuilder.Append(" limit 1");
            }
            else if (type == OrmLiteProviderType.SqlServer)
            {
                sqlbuilder.AppendFormat("SELECT top 1 * FROM [{0}]", tableName);
                ps = ORM.AnonTypeToParams(type, conditions, sqlbuilder);
            }

            var result = new MyTuple<string, DbParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static MyTuple<string, DbParameter[]> SingleById<T>(OrmLiteProviderType type, object idValue, string idField)
        {
            var sp = OrmLite.GetProvider(type).CreateParameter("@" + idField, idValue);
            var sql = "";
            if (type == OrmLiteProviderType.MySql)
            {
                sql = string.Format("SELECT * FROM `{0}` WHERE `{1}`=@{1} limit 1", typeof(T).GetCachedTableName(), idField);
            }
            else if (type == OrmLiteProviderType.SqlServer)
            {
                sql = string.Format("SELECT top 1 * FROM [{0}] WHERE [{1}]=@{1}", typeof(T).GetCachedTableName(), idField);
            }

            MyTuple<string, DbParameter[]> result = new MyTuple<string, DbParameter[]>();
            result.Item1 = sql;
            result.Item2 = new DbParameter[] { sp };
            return result;
        }

        public static MyTuple<string, DbParameter[]> SingleWhere<T>(OrmLiteProviderType type, string name, object value)
        {
            var table = typeof(T).GetCachedTableName();
            var p = OrmLite.GetProvider(type).CreateParameter("@" + name, value);
            var sql = "";
            if (type == OrmLiteProviderType.MySql)
            {
                sql = string.Format("SELECT * FROM `{0}` WHERE `{1}`=@{1} limit 1 ", table, name);
            }
            else if (type == OrmLiteProviderType.SqlServer)
            {
                sql = string.Format("SELECT top 1 * FROM [{0}] WHERE [{1}]=@{1} ", table, name);
            }

            MyTuple<string, DbParameter[]> result = new MyTuple<string, DbParameter[]>();
            result.Item1 = sql;
            result.Item2 = new DbParameter[] { p };
            return result;
        }

        public static MyTuple<string, DbParameter[]> SingleWhere<T>(OrmLiteProviderType type, Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            DbParameter[] ps = null;
            if (type == OrmLiteProviderType.MySql)
            {
                sqlbuilder.AppendFormat("SELECT * FROM `{0}`", tableName);
                ps = ORM.DictionaryToParams(type, conditions, sqlbuilder);
                sqlbuilder.Append(" limit 1");
            }
            else if (type == OrmLiteProviderType.SqlServer)
            {
                sqlbuilder.AppendFormat("SELECT top 1 * FROM [{0}]", tableName);
                ps = ORM.DictionaryToParams(type, conditions, sqlbuilder);
            }
            MyTuple<string, DbParameter[]> result = new MyTuple<string, DbParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static MyTuple<string, DbParameter[]> SingleWhere<T>(OrmLiteProviderType type, object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            DbParameter[] ps = null;
            if (type == OrmLiteProviderType.MySql)
            {
                sqlbuilder.AppendFormat("SELECT * FROM `{0}`", tableName);
                ps = ORM.AnonTypeToParams(type, conditions, sqlbuilder);
                sqlbuilder.Append(" limit 1");
            }
            else if (type == OrmLiteProviderType.SqlServer)
            {
                sqlbuilder.AppendFormat("SELECT top 1 * FROM [{0}]", tableName);
                ps = ORM.AnonTypeToParams(type, conditions, sqlbuilder);
            }

            MyTuple<string, DbParameter[]> result = new MyTuple<string, DbParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static string Count<T>(OrmLiteProviderType providerType)
        {
            var table = typeof(T).GetCachedTableName();
            return "SELECT COUNT(0) FROM " + L(providerType) + table + R(providerType);
        }

        public static MyTuple<string, DbParameter[]> CountWhere<T>(OrmLiteProviderType type, string name, object value)
        {
            var table = typeof(T).GetCachedTableName();
            var provider = OrmLite.GetProvider(type);
            DbParameter p = provider.CreateParameter("@" + name, value);
            var sql = string.Format("SELECT COUNT(0) FROM {2}{0}{3} WHERE {2}{1}{3}=@{1}", table, name, L(type), R(type));
            MyTuple<string, DbParameter[]> result = new MyTuple<string, DbParameter[]>();
            result.Item1 = sql;
            result.Item2 = new DbParameter[] { p };
            return result;
        }
        public static MyTuple<string, DbParameter[]> CountWhere<T>(OrmLiteProviderType type, Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT COUNT(0) FROM {1}{0}{2}", tableName, L(type), R(type));
            var ps = ORM.DictionaryToParams(type, conditions, sqlbuilder);

            MyTuple<string, DbParameter[]> result = new MyTuple<string, DbParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }

        public static MyTuple<string, DbParameter[]> CountWhere<T>(OrmLiteProviderType type, object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("SELECT COUNT(0) FROM {1}{0}{2}", tableName, L(type), R(type));
            var ps = ORM.AnonTypeToParams(type, conditions, sqlbuilder);

            MyTuple<string, DbParameter[]> result = new MyTuple<string, DbParameter[]>();
            result.Item1 = sqlbuilder.ToString();
            result.Item2 = ps;
            return result;
        }


        public static MyTuple<string, DbParameter[]> Update(OrmLiteProviderType type, string tableName, Dictionary<string, object> updateFields, string conditions, Dictionary<string, object> parameters)
        {
            var l = L(type);
            var r = R(type);
            StringBuilder sbsql = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            sbsql.AppendFormat("update {1}{0}{2} set ", tableName, l, r);
            var ps = new List<DbParameter>();
            var nofield = true;
            var provider = OrmLite.GetProvider(type);
            foreach (var field in updateFields)
            {
                if (field.Key.StartsWith("$") || field.Key.StartsWith("_"))
                {
                    sbsql.AppendFormat("{2}{0}{3} = {1},", field.Key.Substring(1), field.Value, l, r);
                    nofield = false;
                }
                else
                {
                    sbsql.AppendFormat("{1}{0}{2} = @_{0},", field.Key, l, r);
                    ps.Add(provider.CreateParameter("@_" + field.Key, field.Value));
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
            var conditionPS = ORM.DictionaryToParams(type, parameters);
            if (conditionPS != null)
            {
                ps.AddRange(conditionPS);
            }
            return new MyTuple<string, DbParameter[]>
            {
                Item1 = sbsql.ToString(),
                Item2 = ps.ToArray()
            };

        }


        public static MyTuple<string, DbParameter[]> Insert<T>(OrmLiteProviderType type, T obj, bool selectIdentity)
        {
            var objtype = typeof(T);
            var table = objtype.GetCachedTableName();
            var propertys = objtype.GetCachedProperties();
            var l = L(type);
            var r = R(type);

            StringBuilder sbsql = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            sbsql.AppendFormat("insert into {1}{0}{2} (", table, l, r);
            StringBuilder sbParams = new StringBuilder(") values (", OrmLite.SqlStringBuilderCapacity);
            var ps = new List<DbParameter>();
            var provider = OrmLite.GetProvider(type);
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
                    sbsql.AppendFormat("{1}{0}{2},", fieldName, l, r);
                    sbParams.AppendFormat("@{0},", fieldName);
                    ps.Add(provider.CreateParameter("@" + fieldName, val ?? DBNull.Value));
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
                if (type == OrmLiteProviderType.MySql)
                {
                    sbsql.Append(";SELECT LAST_INSERT_ID()");
                }
                else if (type == OrmLiteProviderType.SqlServer)
                {
                    sbsql.Append(";SELECT ISNULL(SCOPE_IDENTITY(),@@rowcount)");
                }
            }

            return new MyTuple<string, DbParameter[]>
            {
                Item1 = sbsql.ToString(),
                Item2 = ps.ToArray()
            };
        }

        public static MyTuple<string, DbParameter[]> Insert(OrmLiteProviderType type, string table, Dictionary<string, object> fields, bool selectIdentity = false)
        {
            var l = L(type);
            var r = R(type);

            StringBuilder sbsql = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            sbsql.AppendFormat("insert into {1}{0}{2} (", table, l, r);
            StringBuilder sbParams = new StringBuilder(") values (", OrmLite.SqlStringBuilderCapacity);
            var ps = new List<DbParameter>();
            var provider = OrmLite.GetProvider(type);
            foreach (var field in fields)
            {
                sbsql.AppendFormat("{1}{0}{2},", field.Key, l, r);
                sbParams.AppendFormat("@{0},", field.Key);
                ps.Add(provider.CreateParameter("@" + field.Key, field.Value));
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
                if (type == OrmLiteProviderType.MySql)
                {
                    sbsql.Append(";SELECT LAST_INSERT_ID()");
                }
                else if (type == OrmLiteProviderType.SqlServer)
                {
                    sbsql.Append(";SELECT ISNULL(SCOPE_IDENTITY(),@@rowcount)");
                }
            }
            return new MyTuple<string, DbParameter[]>
            {
                Item1 = sbsql.ToString(),
                Item2 = ps.ToArray()
            };
        }

        public static MyTuple<string, DbParameter[]> Insert(OrmLiteProviderType type, string table, object anonType, bool selectIdentity)
        {
            var objType = anonType.GetType();
            var propertys = objType.GetCachedProperties();
            var l = L(type);
            var r = R(type);

            StringBuilder sbsql = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            sbsql.AppendFormat("insert into {1}{0}{2} (", table, l, r);
            StringBuilder sbParams = new StringBuilder(") values (", OrmLite.SqlStringBuilderCapacity);
            var ps = new List<DbParameter>();
            var provider = OrmLite.GetProvider(type);
            foreach (var property in propertys)
            {
                var fieldName = property.Name;
                var val = property.GetValue(anonType, null);
                sbsql.AppendFormat("{1}{0}{2},", fieldName, l, r);
                sbParams.AppendFormat("@{0},", fieldName);
                ps.Add(provider.CreateParameter("@" + fieldName, val ?? DBNull.Value));
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
                if (type == OrmLiteProviderType.MySql)
                {
                    sbsql.Append(";SELECT LAST_INSERT_ID()");
                }
                else if (type == OrmLiteProviderType.SqlServer)
                {
                    sbsql.Append(";SELECT ISNULL(SCOPE_IDENTITY(),@@rowcount)");
                }
            }
            return new MyTuple<string, DbParameter[]>
            {
                Item1 = sbsql.ToString(),
                Item2 = ps.ToArray()
            };
        }

        public static MyTuple<string, DbParameter[]> Update<T>(OrmLiteProviderType type, T obj, params string[] updateFields)
        {

            var l = L(type);
            var r = R(type);
            var objType = typeof(T);
            var table = objType.GetCachedTableName();
            var propertys = objType.GetCachedProperties();
            StringBuilder sbsql = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            sbsql.AppendFormat("update {1}{0}{2} set ", table, l, r);
            string condition = null;
            var ps = new List<DbParameter>();
            var provider = OrmLite.GetProvider(type);
            foreach (var property in propertys)
            {
                var fieldName = property.Name;
                var fieldAttr = (OrmLiteFieldAttribute)property.GetCachedCustomAttributes(typeof(OrmLiteFieldAttribute)).FirstOrDefault();
                if (fieldAttr == null || (!fieldAttr.UpdateIgnore && !fieldAttr.Ignore))
                {
                    if (fieldAttr != null && fieldAttr.Name != null && fieldAttr.Name.Length > 0)
                    {
                        fieldName = fieldAttr.Name;
                    }
                    if (fieldName.Equals(OrmLite.DefaultKeyName, StringComparison.OrdinalIgnoreCase) || (fieldAttr != null && fieldAttr.IsPrimaryKey))
                    {
                        condition = string.Format("{1}{0}{2} = @{0}", fieldName, l, r);
                        var val = property.GetValue(obj, null);
                        ps.Add(provider.CreateParameter("@" + fieldName, val ?? DBNull.Value));
                    }
                    else
                    {
                        if (FieldsContains(updateFields, fieldName))
                        {
                            sbsql.AppendFormat("{1}{0}{2} = @{0},", fieldName, l, r);
                            var val = property.GetValue(obj, null);
                            ps.Add(provider.CreateParameter("@" + fieldName, val ?? DBNull.Value));
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
            return new MyTuple<string, DbParameter[]>
            {
                Item1 = sbsql.ToString(),
                Item2 = ps.ToArray()
            };
        }


        public static MyTuple<string, DbParameter[]> Update(OrmLiteProviderType type, string tableName, object anonymous)
        {
            var l = L(type);
            var r = R(type);
            var propertys = anonymous.GetType().GetCachedProperties();
            StringBuilder sbsql = new StringBuilder(OrmLite.SqlStringBuilderCapacity);
            sbsql.AppendFormat("update {1}{0}{2} set ", tableName, l, r);
            string condition = null;
            var ps = new List<DbParameter>();

            var provider = OrmLite.GetProvider(type);

            foreach (var property in propertys)
            {
                var fieldName = property.Name;
                if (fieldName.Equals(OrmLite.DefaultKeyName, StringComparison.OrdinalIgnoreCase))
                {
                    condition = string.Format("{1}{0}{2} = @{0}", fieldName, l, r);
                    var val = property.GetValue(anonymous, null);
                    ps.Add(provider.CreateParameter("@" + fieldName, val ?? DBNull.Value));
                }
                else
                {
                    sbsql.AppendFormat("{1}{0}{2} = @{0},", fieldName, l, r);
                    var val = property.GetValue(anonymous, null);
                    ps.Add(provider.CreateParameter("@" + fieldName, val ?? DBNull.Value));
                }
            }

            if (ps.Count == 0)
            {
                throw new ArgumentException("model里没有字段，无法修改");
            }
            sbsql.Remove(sbsql.Length - 1, 1);
            sbsql.AppendFormat(" where ");
            sbsql.Append(condition);
            return new MyTuple<string, DbParameter[]>
            {
                Item1 = sbsql.ToString(),
                Item2 = ps.ToArray()
            };
        }


        private static bool FieldsContains(string[] fields, string value)
        {
            if (fields == null || fields.Length == 0)
            {
                if (OrmLite.UpdateIgnoreFields != null && OrmLite.UpdateIgnoreFields.Count > 0)
                {
                    foreach (var item in OrmLite.UpdateIgnoreFields)
                    {
                        if (item.Equals(value, StringComparison.OrdinalIgnoreCase))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            foreach (var item in fields)
            {
                if (item.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }


        public static MyTuple<string, DbParameter[]> Delete<T>(OrmLiteProviderType type, Dictionary<string, object> conditions)
        {
            var l = L(type);
            var r = R(type);
            StringBuilder sqlbuilder = new StringBuilder(200);
            var tableName = typeof(T).GetCachedTableName();
            sqlbuilder.AppendFormat("DELETE FROM {1}{0}{2}", tableName, l, r);
            var ps = ORM.DictionaryToParams(type, conditions, sqlbuilder);
            return new MyTuple<string, DbParameter[]>
            {
                Item1 = sqlbuilder.ToString(),
                Item2 = ps
            };
        }

        public static MyTuple<string, DbParameter[]> DeleteById<T>(OrmLiteProviderType type, object id, string idField)
        {
            var l = L(type);
            var r = R(type);
            DbParameter sp = OrmLite.GetProvider(type).CreateParameter("@" + idField, id);
            var sql = string.Format("DELETE FROM {2}{0}{3} WHERE {2}{1}{3}=@{1}", typeof(T).GetCachedTableName(), idField, l, r);
            return new MyTuple<string, DbParameter[]>
            {
                Item1 = sql,
                Item2 = new DbParameter[] { sp }
            };
        }

        public static string DeleteByIds<T>(OrmLiteProviderType type, IEnumerable idValues, string idField)
        {
            var l = L(type);
            var r = R(type);
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
                    sql.AppendFormat("DELETE from {2}{0}{3} where {2}{1}{3} in (", table, idField, l, r);
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

        public static string Delete<T>(OrmLiteProviderType type)
        {
            var l = L(type);
            var r = R(type);
            var table = typeof(T).GetCachedTableName();
            return "DELETE  FROM " + l + table + r;
        }
    }
}
