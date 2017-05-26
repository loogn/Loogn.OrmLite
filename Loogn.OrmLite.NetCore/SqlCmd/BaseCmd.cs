using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Loogn.OrmLite.NetCore
{

    /// <summary>
    /// sql命令信息
    /// </summary>
    class CmdInfo
    {
        public string CmdText { get; set; }
        public DbParameter[] Params { get; set; }

    }

    /// <summary>
    /// sql语句基类
    /// </summary>
    abstract class BaseCmd
    {

        public DbParameter[] AnonTypeToParams(object anonType)
        {
            if (anonType != null)
            {
                var props = ReflectionHelper.GetCachedProperties(anonType.GetType());
                var ps = new DbParameter[props.Length];
                for (int i = 0, len = props.Length; i < len; i++)
                {
                    var prop = props[i];
                    var p = provider.CreateParameter("@" + prop.Name, prop.GetValue(anonType, null));
                    ps[i] = p;
                }
                return ps;
            }
            return null;
        }

        public DbParameter[] AnonTypeToParams(object anonType, StringBuilder appendWhere)
        {
            var props = ReflectionHelper.GetCachedProperties(anonType.GetType());

            if (props.Length > 0)
            {
                DbParameter[] ps = new DbParameter[props.Length];
                int i = 0;
                appendWhere.Append(" where ");
                foreach (var prop in props)
                {
                    var p = CreateParameter("@" + prop.Name, prop.GetValue(anonType, null));
                    ps[i++] = p;
                    appendWhere.AppendFormat(" {0}=@{0} and ", prop.Name);
                }
                appendWhere.Length -= 4;
                return ps;
            }
            return null;
        }

        public DbParameter[] DictionaryToParams(IDictionary<string, object> dict)
        {
            if (dict != null)
            {
                DbParameter[] ps = new DbParameter[dict.Count];
                int i = 0;
                foreach (var kv in dict)
                {
                    var p = CreateParameter("@" + kv.Key, kv.Value);
                    ps[i++] = p;
                }
                return ps;
            }
            return null;
        }

        public DbParameter[] DictionaryToParams(IDictionary<string, object> conditions, StringBuilder appendWhere)
        {
            if (conditions != null && conditions.Count > 0)
            {
                DbParameter[] ps = new DbParameter[conditions.Count];
                int i = 0;
                appendWhere.Append(" where ");
                foreach (var kv in conditions)
                {
                    var p = CreateParameter("@" + kv.Key, kv.Value);

                    ps[i++] = p;
                    appendWhere.AppendFormat(" {0}=@{0} and ", kv.Key);
                }
                appendWhere.Length -= 4;
                return ps;
            }
            return null;
        }


        public static BaseCmd GetCmd(OrmLiteProviderType providerType)
        {
            if (providerType == OrmLiteProviderType.SqlServer)
            {
                return SqlServerCmd.Instance;
            }
            else if (providerType == OrmLiteProviderType.MySql)
            {
                return MySqlCmd.Instance;
            }
            else if (providerType == OrmLiteProviderType.Sqlite)
            {
                return SqliteCmd.Instance;
            }
            else
            {
                throw new Exception("what?");
            }
        }


        abstract public string R();
        abstract public string L();

        abstract public string IFNULL();

        abstract public string GetLastInsertID();

        protected IOrmLiteProvider provider;

        public DbParameter CreateParameter(string name, object value)
        {
            return provider.CreateParameter(name, value);
        }


        public string Select<T>()
        {
            var table = ReflectionHelper.GetInfo<T>().TableName;
            return "SELECT * FROM " + L() + table + R();
        }

        public string FullPartSql<T>(string sql, PartSqlType type)
        {
            sql = sql.TrimStart();
            if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return sql;
            }

            var tableName = ReflectionHelper.GetInfo<T>().TableName;

            StringBuilder sb = new StringBuilder(sql.Length + 50);
            switch (type)
            {
                case PartSqlType.Select:
                    return sb.AppendFormat("SELECT * FROM {2}{0}{3} where {1}", tableName, sql, L(), R()).ToString();
                case PartSqlType.Count:
                    return sb.AppendFormat("SELECT COUNT(0) FROM {2}{0}{3} where {1}", tableName, sql, L(), R()).ToString();
                default:
                    return sql;
            }
        }


        public abstract string FullPartSqlSingle<T>(string sql);


        public CmdInfo SelectWhere<T>(string name, object value)
        {
            var table = ReflectionHelper.GetInfo<T>().TableName;
            var p = provider.CreateParameter("@" + name, value);
            var cmd = new CmdInfo();
            cmd.CmdText = string.Format("Select * from {2}{0}{3} where {2}{1}{3}=@{1}", table, name, L(), R());
            cmd.Params = new DbParameter[] { p };
            return cmd;
        }

        public CmdInfo SelectWhere<T>(IDictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = ReflectionHelper.GetInfo<T>().TableName;
            sqlbuilder.AppendFormat("SELECT * FROM {1}{0}{2}", tableName, L(), R());
            var ps = DictionaryToParams(conditions, sqlbuilder);

            var cmd = new CmdInfo();
            cmd.CmdText = sqlbuilder.ToString();
            cmd.Params = ps;
            return cmd;
        }



        public CmdInfo SelectWhere<T>(object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = ReflectionHelper.GetInfo<T>().TableName;
            sqlbuilder.AppendFormat("SELECT * FROM {1}{0}{2}", tableName, L(), R());
            var ps = AnonTypeToParams(conditions, sqlbuilder);

            var cmd = new CmdInfo();
            cmd.CmdText = sqlbuilder.ToString();
            cmd.Params = ps;
            return cmd;

        }

        public string SelectByIds<T>(IEnumerable idValues, string idField, string selectFields = "*")
        {
            if (idValues == null) return null;
            bool any = false;
            var needQuot = false;
            StringBuilder sql = null;
            var enumerator = idValues.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var obj = enumerator.Current;
                if (!any)
                {
                    any = true;

                    if (obj is string || obj is DateTime)
                    {
                        needQuot = true;
                    }
                    var table = ReflectionHelper.GetInfo<T>().TableName;
                    sql = new StringBuilder(50);
                    sql.AppendFormat("Select {2} from {3}{0}{4} where {3}{1}{4} in (", table, idField, selectFields, L(), R());
                }
                if (needQuot)
                {
                    sql.AppendFormat("'{0}',", obj);
                }
                else
                {
                    sql.AppendFormat("{0},", obj);
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

        abstract public CmdInfo Single<T>(IDictionary<string, object> conditions);


        abstract public CmdInfo Single<T>(object conditions);

        abstract public CmdInfo SingleById<T>(object idValue, string idField);

        abstract public CmdInfo SingleWhere<T>(string name, object value);

        abstract public CmdInfo SingleWhere<T>(IDictionary<string, object> conditions);

        abstract public CmdInfo SingleWhere<T>(object conditions);


        public string Count<T>()
        {
            var table = ReflectionHelper.GetInfo<T>().TableName;
            return "SELECT COUNT(0) FROM " + L() + table + R();
        }

        public CmdInfo CountWhere<T>(string name, object value)
        {
            var table = ReflectionHelper.GetInfo<T>().TableName;
            DbParameter p = provider.CreateParameter("@" + name, value);
            var sql = string.Format("SELECT COUNT(0) FROM {2}{0}{3} WHERE {2}{1}{3}=@{1}", table, name, L(), R());

            return new CmdInfo
            {
                CmdText = sql,
                Params = new DbParameter[] { p }
            };
        }
        public CmdInfo CountWhere<T>(IDictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = ReflectionHelper.GetInfo<T>().TableName;
            sqlbuilder.AppendFormat("SELECT COUNT(0) FROM {1}{0}{2}", tableName, L(), R());
            var ps = DictionaryToParams(conditions, sqlbuilder);
            return new CmdInfo
            {
                CmdText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public CmdInfo CountWhere<T>(object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = ReflectionHelper.GetInfo<T>().TableName;
            sqlbuilder.AppendFormat("SELECT COUNT(0) FROM {1}{0}{2}", tableName, L(), R());
            var ps = AnonTypeToParams(conditions, sqlbuilder);
            return new CmdInfo
            {
                CmdText = sqlbuilder.ToString(),
                Params = ps
            };
        }


        public CmdInfo Update(string tableName, IDictionary<string, object> updateFields, string conditions, IDictionary<string, object> parameters)
        {
            var l = L();
            var r = R();
            StringBuilder sbsql = new StringBuilder(50);
            sbsql.AppendFormat("update {1}{0}{2} set ", tableName, l, r);
            var ps = new List<DbParameter>();
            var nofield = true;
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
            var conditionPS = DictionaryToParams(parameters);
            if (conditionPS != null)
            {
                ps.AddRange(conditionPS);
            }
            return new CmdInfo
            {
                CmdText = sbsql.ToString(),
                Params = ps.ToArray()
            };
        }

        public CmdInfo Insert<T>(T obj, bool selectIdentity)
        {
            var refInfo = ReflectionHelper.GetInfo<T>();

            var table = refInfo.TableName;
            var propertys = refInfo.Properties;
            var l = L();
            var r = R();

            StringBuilder sbsql = new StringBuilder(50);
            sbsql.AppendFormat("insert into {1}{0}{2} (", table, l, r);
            StringBuilder sbParams = new StringBuilder(") values (", 50);
            var ps = new List<DbParameter>();

            foreach (var property in propertys)
            {
                var fieldAttr = refInfo.GetFieldAttr(property);
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
                    var accessor = refInfo.GetAccessor(fieldName);
                    if (accessor == null) continue;

                    var val = accessor.Get(obj);
                    if (val == null)
                    {
                        if (property.PropertyType == PrimitiveTypes.String)
                        {
                            val = string.Empty;
                        }
                    }
                    else
                    {
                        if (val is DateTime && DateTime.MinValue.Equals(val))
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
            sbsql.Append(");");

            if (selectIdentity)
            {
                sbsql.Append(GetLastInsertID());
            }
            return new CmdInfo
            {
                CmdText = sbsql.ToString(),
                Params = ps.ToArray()
            };
        }

        public CmdInfo Insert(string table, IDictionary<string, object> fields, bool selectIdentity = false)
        {
            var l = L();
            var r = R();
            StringBuilder sbsql = new StringBuilder(50);
            sbsql.AppendFormat("insert into {1}{0}{2} (", table, l, r);
            StringBuilder sbParams = new StringBuilder(") values (", 50);
            var ps = new List<DbParameter>();
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
            sbsql.Append(");");

            if (selectIdentity)
            {
                sbsql.Append(GetLastInsertID());
            }
            return new CmdInfo
            {
                CmdText = sbsql.ToString(),
                Params = ps.ToArray()
            };
        }

        public CmdInfo Insert(string table, object anonType, bool selectIdentity)
        {
            var propertys = ReflectionHelper.GetCachedProperties(anonType.GetType());
            var l = L();
            var r = R();

            StringBuilder sbsql = new StringBuilder(50);
            sbsql.AppendFormat("insert into {1}{0}{2} (", table, l, r);
            StringBuilder sbParams = new StringBuilder(") values (", 50);
            var ps = new List<DbParameter>();

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
            sbsql.Append(");");

            if (selectIdentity)
            {
                sbsql.Append(GetLastInsertID());
            }
            return new CmdInfo
            {
                CmdText = sbsql.ToString(),
                Params = ps.ToArray()
            };
        }

        public CmdInfo Update<T>(T obj, params string[] updateFields)
        {
            var l = L();
            var r = R();
            var refInfo = ReflectionHelper.GetInfo<T>();

            var table = refInfo.TableName;
            var propertys = refInfo.Properties;
            StringBuilder sbsql = new StringBuilder(50);
            sbsql.AppendFormat("update {1}{0}{2} set ", table, l, r);
            string condition = null;
            var ps = new List<DbParameter>();

            foreach (var property in propertys)
            {
                var fieldName = property.Name;
                var fieldAttr = refInfo.GetFieldAttr(property);
                if (fieldAttr == null || (!fieldAttr.UpdateIgnore && !fieldAttr.Ignore))
                {
                    var accessor = refInfo.GetAccessor(fieldName);
                    if (accessor == null) continue;

                    if (fieldName.Equals(OrmLite.DefaultKeyName, StringComparison.OrdinalIgnoreCase) || (fieldAttr != null && fieldAttr.IsPrimaryKey))
                    {
                        condition = string.Format("{1}{0}{2} = @{0}", fieldName, l, r);
                        var val = accessor.Get(obj);
                        ps.Add(provider.CreateParameter("@" + fieldName, val ?? DBNull.Value));
                    }
                    else
                    {
                        if (FieldsContains(updateFields, fieldName))
                        {
                            sbsql.AppendFormat("{1}{0}{2} = @{0},", fieldName, l, r);
                            var val = accessor.Get(obj);
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

            return new CmdInfo
            {
                CmdText = sbsql.ToString(),
                Params = ps.ToArray()
            };
        }


        public CmdInfo Update(string tableName, object anonymous)
        {
            var l = L();
            var r = R();
            var propertys = ReflectionHelper.GetCachedProperties(anonymous.GetType());
            StringBuilder sbsql = new StringBuilder(50);
            sbsql.AppendFormat("update {1}{0}{2} set ", tableName, l, r);
            string condition = null;
            var ps = new List<DbParameter>();

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
            return new CmdInfo
            {
                CmdText = sbsql.ToString(),
                Params = ps.ToArray()
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


        public CmdInfo Delete<T>(IDictionary<string, object> conditions)
        {

            StringBuilder sqlbuilder = new StringBuilder(200);
            var tableName = ReflectionHelper.GetInfo<T>().TableName;
            sqlbuilder.AppendFormat("DELETE FROM {1}{0}{2}", tableName, L(), R());
            var ps = DictionaryToParams(conditions, sqlbuilder);
            return new CmdInfo
            {
                CmdText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public CmdInfo DeleteById<T>(object id, string idField)
        {
            var sp = provider.CreateParameter("@" + idField, id);
            var sql = string.Format("DELETE FROM {2}{0}{3} WHERE {2}{1}{3}=@{1}", ReflectionHelper.GetInfo<T>().TableName, idField, L(), R());
            return new CmdInfo
            {
                CmdText = sql,
                Params = new DbParameter[] { sp }
            };
        }

        public string DeleteByIds<T>(IEnumerable idValues, string idField)
        {
            if (idValues == null) return null;
            bool any = false;
            var needQuot = false;
            StringBuilder sql = null;
            var enumerator = idValues.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var obj = enumerator.Current;
                if (!any)
                {
                    any = true;
                    if (obj is string || obj is DateTime)
                    {
                        needQuot = true;
                    }
                    var table = ReflectionHelper.GetInfo<T>().TableName;
                    sql = new StringBuilder(200);
                    sql.AppendFormat("DELETE from {2}{0}{3} where {2}{1}{3} in (", table, idField, L(), R());
                }
                if (needQuot)
                {
                    sql.AppendFormat("'{0}',", obj);
                }
                else
                {
                    sql.AppendFormat("{0},", obj);
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

        public string Delete<T>()
        {
            var table = ReflectionHelper.GetInfo<T>().TableName;
            return string.Format("DELETE  FROM  {1}{0}{2}", table, L(), R());
        }

        abstract public string PageSql(OrmLitePageFactor factor);


        private static string PageSql(OrmLiteProviderType providerType, OrmLitePageFactor factor)
        {
            StringBuilder sb = new StringBuilder(100);
            if (providerType == OrmLiteProviderType.SqlServer)
            {
                sb.AppendFormat("select * from (");
                sb.AppendFormat(" select top {0} {1},ROW_NUMBER() over(order by {2}) rowid from {3}", factor.PageIndex * factor.PageSize, factor.Fields, factor.OrderBy, factor.TableName);
                if (!string.IsNullOrEmpty(factor.Conditions))
                {
                    sb.AppendFormat(" where {0}", factor.Conditions);
                }
                sb.AppendFormat(")t where t.rowid>{0}", (factor.PageIndex - 1) * factor.PageSize);
            }
            else if (providerType == OrmLiteProviderType.MySql || providerType == OrmLiteProviderType.Sqlite)
            {
                sb.AppendFormat("select {0} from {1}", factor.Fields, factor.TableName);
                if (!string.IsNullOrEmpty(factor.Conditions))
                {
                    sb.AppendFormat(" where {0}", factor.Conditions);
                }
                sb.AppendFormat(" order by {0} limit {1},{2}", factor.OrderBy, (factor.PageIndex - 1) * factor.PageSize, factor.PageSize);
            }
            else
            {
                throw new Exception("没有为" + providerType + "提供PageSql");
            }
            return sb.ToString();
        }

        abstract public CmdInfo TableMetaDataSql(string dbName);
        abstract public CmdInfo ColumnMetaDataSql(string dbName, string tableName);
    }
}
