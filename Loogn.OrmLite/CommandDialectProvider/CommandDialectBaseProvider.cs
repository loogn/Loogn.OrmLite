using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    /// <summary>
    /// 命令方言基类
    /// </summary>
    public abstract class CommandDialectBaseProvider : ICommandDialectProvider
    {
        /// <summary>
        /// 获取报名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected string GetTableName<T>()
        {
            var tableName = TypeCachedDict.GetTypeCachedInfo<T>().TableName;
            return tableName;
        }

        /// <summary>
        /// 处理默认值，string datetime
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        protected object DealDefaultValue(object value, Type type)
        {
            if (value == null)
            {
                if (type == Types.String)
                {
                    return string.Empty; ;
                }
            }
            else
            {
                if (value is DateTime && DateTime.MinValue.Equals(value))
                {
                    return DateTime.Now;
                }
            }
            return value;
        }

        /// <summary>
        /// 左引号
        /// </summary>
        public abstract string CloseQuote
        {
            get;
        }

        /// <summary>
        /// 判断是否为null的函数
        /// </summary>
        public abstract string IsNullFunc
        {
            get;
        }
        /// <summary>
        /// 右引号
        /// </summary>
        public abstract string OpenQuote
        {
            get;
        }

        /// <summary>
        /// 获取列元数据
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public abstract CommandInfo ColumnMetaData(string dbName, string tableName);

        /// <summary>
        /// 获取Count命令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public CommandInfo Count<T>()
        {
            var tableName = GetTableName<T>();
            var sql = string.Format("SELECT COUNT(0) FROM {0}{1}{2}", OpenQuote, tableName, CloseQuote);
            return new CommandInfo
            {
                CommandText = sql
            };
        }

        public CommandInfo CountWhere<T>(object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = GetTableName<T>();
            sqlbuilder.AppendFormat("SELECT COUNT(0) FROM {1}{0}{2}", tableName, OpenQuote, CloseQuote);
            var ps = TransformToParameters.ObjectToParams(this, conditions, sqlbuilder);
            return new CommandInfo
            {
                CommandText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public CommandInfo CountWhere<T>(IDictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = GetTableName<T>();
            sqlbuilder.AppendFormat("SELECT COUNT(0) FROM {1}{0}{2}", tableName, OpenQuote, CloseQuote);
            var ps = TransformToParameters.DictionaryToParams(this, conditions, sqlbuilder);
            return new CommandInfo
            {
                CommandText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public CommandInfo CountWhere<T>(string name, object value)
        {
            var tableName = GetTableName<T>();
            var p = CreateParameter(name, value);
            var sql = string.Format("SELECT COUNT(0) FROM {2}{0}{3} WHERE {2}{1}{3}=@{1}", tableName, name, OpenQuote, CloseQuote);

            return new CommandInfo
            {
                CommandText = sql,
                Params = new IDbDataParameter[] { p }
            };
        }

        public abstract IDbDataParameter CreateParameter();

        protected IDbDataParameter CreateParameter(string name, object value)
        {
            var p = CreateParameter();
            p.ParameterName = "@" + name;
            p.Value = value;
            return p;
        }

        public CommandInfo Delete<T>()
        {
            var tableName = GetTableName<T>();
            var sql = string.Format("DELETE  FROM  {1}{0}{2}", tableName, OpenQuote, CloseQuote);
            return new CommandInfo
            {
                CommandText = sql
            };
        }

        public CommandInfo DeleteById<T>(object id, string idField = "Id")
        {
            var p = CreateParameter(idField, id);
            var tableName = GetTableName<T>();
            var sql = string.Format("DELETE FROM {2}{0}{3} WHERE {2}{1}{3}=@{1}", tableName, idField, OpenQuote, CloseQuote);
            return new CommandInfo
            {
                CommandText = sql,
                Params = new IDbDataParameter[] { p }
            };
        }

        public CommandInfo DeleteByIds<T>(IEnumerable idValues, string idField)
        {
            if (idValues == null) return new CommandInfo { CommandText = "" };
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
                    var tableName = GetTableName<T>();
                    sql = new StringBuilder(200);
                    sql.AppendFormat("DELETE from {2}{0}{3} where {2}{1}{3} in (", tableName, idField, OpenQuote, CloseQuote);
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
                return new CommandInfo { CommandText = "" };
            }
            else
            {
                sql.Replace(',', ')', sql.Length - 1, 1);
                return new CommandInfo
                {
                    CommandText = sql.ToString(),
                };
            }
        }

        public CommandInfo DeleteWhere<T>(object obj)
        {
            var tableName = GetTableName<T>();
            StringBuilder sb = new StringBuilder(50);
            sb.AppendFormat(" DELETE FROM {0}{1}{2}", OpenQuote, tableName, CloseQuote);
            var ps = TransformToParameters.ObjectToParams(this, obj, sb);
            return new CommandInfo
            {
                CommandText = sb.ToString(),
                Params = ps
            };
        }

        public CommandInfo DeleteWhere<T>(IDictionary<string, object> conditions)
        {
            var tableName = GetTableName<T>();
            StringBuilder sb = new StringBuilder(50);
            sb.AppendFormat(" DELETE FROM {0}{1}{2}", OpenQuote, tableName, CloseQuote);
            var ps = TransformToParameters.DictionaryToParams(this, conditions, sb);
            return new CommandInfo
            {
                CommandText = sb.ToString(),
                Params = ps
            };
        }

        public CommandInfo DeleteWhere<T>(string name, object value)
        {
            var tableName = GetTableName<T>();
            var p = CreateParameter(name, value);
            var sql = string.Format("DELETE FROM {2}{0}{3} WHERE {2}{1}{3}=@{1}", tableName, name, OpenQuote, CloseQuote);
            return new CommandInfo
            {
                CommandText = sql,
                Params = new IDbDataParameter[] { p }
            };
        }

        public CommandInfo FullCount<T>(string sqlOrCondition)
        {
            var sql = sqlOrCondition.TrimStart();
            if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return new CommandInfo
                {
                    CommandText = sql
                };
            }
            var tableName = GetTableName<T>();

            StringBuilder sb = new StringBuilder(sql.Length + 50);
            sql = sb.AppendFormat("SELECT COUNT(0) FROM {2}{0}{3} where {1}", tableName, sql, OpenQuote, CloseQuote).ToString();
            return new CommandInfo
            {
                CommandText = sql,
            };
        }

        public CommandInfo FullSelect<T>(string sqlOrCondition)
        {
            var sql = sqlOrCondition.TrimStart();
            if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return new CommandInfo
                {
                    CommandText = sql
                };
            }
            var tableName = GetTableName<T>();
            StringBuilder sb = new StringBuilder(sql.Length + 50);
            sql = sb.AppendFormat("SELECT * FROM {2}{0}{3} where {1}", tableName, sql, OpenQuote, CloseQuote).ToString();
            return new CommandInfo
            {
                CommandText = sql,
            };
        }

        public abstract CommandInfo FullSingle<T>(string sqlOrCondition);

        public abstract string GetLastInsertId();

        public CommandInfo Insert(string table, object obj, bool selectIdentity = false)
        {
            var propInvokerDict = TypeCachedDict.GetTypeCachedInfo(obj.GetType()).PropInvokerDict;
            var l = OpenQuote;
            var r = CloseQuote;

            StringBuilder sbsql = new StringBuilder(50);
            sbsql.AppendFormat("insert into {1}{0}{2} (", table, l, r);
            StringBuilder sbParams = new StringBuilder(") values (", 50);
            var ps = new IDbDataParameter[propInvokerDict.Count];
            var index = 0;
            foreach (var kv in propInvokerDict)
            {
                var fieldName = kv.Key;
                var val = kv.Value.Get(obj);
                sbsql.AppendFormat("{1}{0}{2},", fieldName, l, r);
                sbParams.AppendFormat("@{0},", fieldName);
                ps[index++] = CreateParameter(fieldName, val ?? DBNull.Value);
            }
            if (ps.Length == 0)
            {
                throw new ArgumentException("model里没有字段，无法插入");
            }
            sbsql.Remove(sbsql.Length - 1, 1);
            sbParams.Remove(sbParams.Length - 1, 1);
            sbsql.Append(sbParams.ToString());
            sbsql.Append(");");

            if (selectIdentity)
            {
                sbsql.Append(GetLastInsertId());
            }
            return new CommandInfo
            {
                CommandText = sbsql.ToString(),
                Params = ps
            };
        }

        public CommandInfo Insert(string table, IDictionary<string, object> fields, bool selectIdentity = false)
        {
            var l = OpenQuote;
            var r = CloseQuote;
            StringBuilder sbsql = new StringBuilder(50);
            sbsql.AppendFormat("insert into {1}{0}{2} (", table, l, r);
            StringBuilder sbParams = new StringBuilder(") values (", 50);
            var ps = new IDbDataParameter[fields.Count];
            var index = 0;
            foreach (var field in fields)
            {
                sbsql.AppendFormat("{1}{0}{2},", field.Key, l, r);
                sbParams.AppendFormat("@{0},", field.Key);
                ps[index++] = CreateParameter(field.Key, field.Value);
            }
            if (ps.Length == 0)
            {
                throw new ArgumentException("fields里没有字段，无法插入");
            }
            sbsql.Remove(sbsql.Length - 1, 1);
            sbParams.Remove(sbParams.Length - 1, 1);
            sbsql.Append(sbParams.ToString());
            sbsql.Append(");");

            if (selectIdentity)
            {
                sbsql.Append(GetLastInsertId());
            }
            return new CommandInfo
            {
                CommandText = sbsql.ToString(),
                Params = ps
            };
        }

        public CommandInfo Insert<T>(T obj, bool selectIdentity = false)
        {
            var typeInfo = TypeCachedDict.GetTypeCachedInfo<T>();

            var table = typeInfo.TableName;

            var l = OpenQuote;
            var r = CloseQuote;

            StringBuilder sbsql = new StringBuilder(50);
            sbsql.AppendFormat("insert into {1}{0}{2} (", table, l, r);
            StringBuilder sbParams = new StringBuilder(") values (", 50);
            var ps = new List<IDbDataParameter>();

            foreach (var kv in typeInfo.accessorDict)
            {
                var fieldAttr = kv.Value.OrmLiteField;
                if (kv.Key.Equals(OrmLite.DefaultKeyName, StringComparison.OrdinalIgnoreCase))
                {
                    if (fieldAttr == null || (!fieldAttr.InsertRequire))
                    {
                        continue;
                    }
                }
                if (fieldAttr == null || (!fieldAttr.InsertIgnore && !fieldAttr.Ignore))
                {
                    if (!kv.Value.CanGet)
                    {
                        continue;
                    }

                    var val = kv.Value.Get(obj);
                    val = DealDefaultValue(val, kv.Value.prop.PropertyType);
                    sbsql.AppendFormat("{1}{0}{2},", kv.Key, l, r);
                    sbParams.AppendFormat("@{0},", kv.Key);
                    ps.Add(CreateParameter(kv.Key, val ?? DBNull.Value));
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
                sbsql.Append(GetLastInsertId());
            }
            return new CommandInfo
            {
                CommandText = sbsql.ToString(),
                Params = ps.ToArray()
            };
        }

        public abstract CommandInfo Paged(OrmLitePageFactor factor);


        public CommandInfo Select<T>()
        {
            var tableName = GetTableName<T>();
            var sql = "SELECT * FROM " + OpenQuote + tableName + CloseQuote;
            return new CommandInfo
            {
                CommandText = sql
            };
        }

        public CommandInfo SelectByIds<T>(IEnumerable idValues, string idField, string selectFields = "*")
        {
            if (idValues == null) return new CommandInfo { CommandText = string.Empty };
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
                    var tableName = GetTableName<T>();
                    sql = new StringBuilder(50);
                    sql.AppendFormat("Select {2} from {3}{0}{4} where {3}{1}{4} in (", tableName, idField, selectFields, OpenQuote, CloseQuote);
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
                return new CommandInfo { CommandText = string.Empty };
            }
            else
            {
                sql.Replace(',', ')', sql.Length - 1, 1);
                return new CommandInfo
                {
                    CommandText = sql.ToString(),
                };
            }
        }

        public CommandInfo SelectWhere<T>(object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = GetTableName<T>();
            sqlbuilder.AppendFormat("SELECT * FROM {1}{0}{2}", tableName, OpenQuote, CloseQuote);
            var ps = TransformToParameters.ObjectToParams(this, conditions, sqlbuilder);

            return new CommandInfo
            {
                CommandText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public CommandInfo SelectWhere<T>(IDictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = GetTableName<T>();
            sqlbuilder.AppendFormat("SELECT * FROM {1}{0}{2}", tableName, OpenQuote, CloseQuote);
            var ps = TransformToParameters.DictionaryToParams(this, conditions, sqlbuilder);

            return new CommandInfo
            {
                CommandText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public CommandInfo SelectWhere<T>(string name, object value)
        {
            var tableName = GetTableName<T>();
            var p = CreateParameter(name, value);
            var sql = string.Format("Select * from {2}{0}{3} where {2}{1}{3}=@{1}", tableName, name, OpenQuote, CloseQuote);
            return new CommandInfo
            {
                CommandText = sql,
                Params = new IDbDataParameter[] { p }
            };
        }

        public abstract CommandInfo SingleById<T>(object idValue, string idField);

        public abstract CommandInfo SingleWhere<T>(object conditions);

        public abstract CommandInfo SingleWhere<T>(IDictionary<string, object> conditions);

        public abstract CommandInfo SingleWhere<T>(string name, object value);

        public abstract CommandInfo TableMetaData(string dbName);

        public CommandInfo Update(string tableName, object obj)
        {
            var l = OpenQuote;
            var r = CloseQuote;
            var typeInfo = TypeCachedDict.GetTypeCachedInfo(obj.GetType());

            StringBuilder sbsql = new StringBuilder(50);
            sbsql.AppendFormat("update {1}{0}{2} set ", tableName, l, r);
            string condition = null;
            var ps = new List<IDbDataParameter>();

            foreach (var kv in typeInfo.PropInvokerDict)
            {
                var fieldName = kv.Key;

                if (fieldName.Equals(OrmLite.DefaultKeyName, StringComparison.OrdinalIgnoreCase))
                {
                    condition = string.Format("{1}{0}{2} = @{0}", fieldName, l, r);
                    var val = kv.Value.Get(obj);
                    ps.Add(CreateParameter(fieldName, val ?? DBNull.Value));
                }
                else
                {
                    sbsql.AppendFormat("{1}{0}{2} = @{0},", fieldName, l, r);
                    var val = kv.Value.Get(obj);
                    ps.Add(CreateParameter(fieldName, val ?? DBNull.Value));
                }
            }
            if (ps.Count == 0)
            {
                throw new ArgumentException("model里没有字段，无法修改");
            }
            sbsql.Remove(sbsql.Length - 1, 1);
            sbsql.AppendFormat(" where ");
            sbsql.Append(condition);
            return new CommandInfo
            {
                CommandText = sbsql.ToString(),
                Params = ps.ToArray()
            };
        }

        public CommandInfo Update(string tableName, IDictionary<string, object> updateFields, string conditions, IDictionary<string, object> parameters)
        {
            var l = OpenQuote;
            var r = CloseQuote;
            StringBuilder sbsql = new StringBuilder(50);
            sbsql.AppendFormat("update {1}{0}{2} set ", tableName, l, r);
            var ps = new List<IDbDataParameter>();
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
                    sbsql.AppendFormat("{1}{0}{2} = @{0},", field.Key, l, r);
                    ps.Add(CreateParameter(field.Key, field.Value));
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
            var conditionPS = TransformToParameters.DictionaryToParams(this, parameters);
            if (conditionPS != null)
            {
                ps.AddRange(conditionPS);
            }
            return new CommandInfo
            {
                CommandText = sbsql.ToString(),
                Params = ps.ToArray()
            };
        }

        public CommandInfo Update<T>(T obj, params string[] updateFields)
        {
            var l = OpenQuote;
            var r = CloseQuote;
            var typeInfo = TypeCachedDict.GetTypeCachedInfo<T>();

            var tableName = typeInfo.TableName;

            StringBuilder sbsql = new StringBuilder(50);
            sbsql.AppendFormat("update {1}{0}{2} set ", tableName, l, r);
            string condition = null;
            var ps = new List<IDbDataParameter>();

            foreach (var kv in typeInfo.accessorDict)
            {
                var fieldName = kv.Key;
                var fieldAttr = kv.Value.OrmLiteField;
                if (fieldAttr == null || (!fieldAttr.UpdateIgnore && !fieldAttr.Ignore))
                {
                    if (!kv.Value.CanGet) continue;

                    if (fieldName.Equals(OrmLite.DefaultKeyName, StringComparison.OrdinalIgnoreCase) || (fieldAttr != null && fieldAttr.IsPrimaryKey))
                    {
                        condition = string.Format("{1}{0}{2} = @{0}", fieldName, l, r);
                        var val = kv.Value.Get(obj);
                        ps.Add(CreateParameter(fieldName, val ?? DBNull.Value));
                    }
                    else
                    {
                        if (FieldsContains(updateFields, fieldName))
                        {
                            sbsql.AppendFormat("{1}{0}{2} = @{0},", fieldName, l, r);
                            var val = kv.Value.Get(obj);
                            val = DealDefaultValue(val, kv.Value.prop.PropertyType);
                            ps.Add(CreateParameter(fieldName, val ?? DBNull.Value));
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

            return new CommandInfo
            {
                CommandText = sbsql.ToString(),
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

        public abstract IDbConnection CreateConnection();

    }
}
