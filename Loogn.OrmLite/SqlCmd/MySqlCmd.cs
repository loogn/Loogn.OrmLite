using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Loogn.OrmLite
{
    class MySqlCmd : BaseCmd
    {

        static MySqlCmd instance;
        public static MySqlCmd Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MySqlCmd();
                }
                return instance;
            }
        }

        public MySqlCmd()
        {
            provider = OrmLite.GetProvider(OrmLiteProviderType.MySql);
        }

        public override string FullPartSqlSingle<T>(string sql)
        {
            sql = sql.TrimStart();
            if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return sql;
            }
            var tableName = ReflectionHelper.GetInfo<T>().TableName;
            StringBuilder sb = new StringBuilder(sql.Length + 50);
            return sb.AppendFormat("SELECT * FROM `{0}` where {1} limit 1", tableName, sql).ToString();
        }

        public override string GetLastInsertID()
        {
            return "SELECT LAST_INSERT_ID()";
        }

        public override string IFNULL()
        {
            return "IFNULL";
        }

        public override string L()
        {
            return "`";
        }

        public override string PageSql(OrmLitePageFactor factor)
        {
            StringBuilder sb = new StringBuilder(100);

            var l = L();
            var r = R();
            if (factor.TableName.IndexOf(" ") > 0)
            {
                l = "";
                r = "";
            }

            sb.AppendFormat("select {0} from {2}{1}{3}", factor.Fields, factor.TableName, l, r);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            sb.AppendFormat(" order by {0} limit {1},{2}", factor.OrderBy, (factor.PageIndex - 1) * factor.PageSize, factor.PageSize);
            return sb.ToString();
        }

        public override string R()
        {
            return "`";
        }

        public override CmdInfo Single<T>(object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = ReflectionHelper.GetInfo<T>().TableName;
            DbParameter[] ps = null;
            sqlbuilder.AppendFormat("SELECT * FROM `{0}`", tableName);
            ps = AnonTypeToParams(conditions, sqlbuilder);
            sqlbuilder.Append(" limit 1");

            var cmd = new CmdInfo
            {
                CmdText = sqlbuilder.ToString(),
                Params = ps
            };
            return cmd;
        }

        public override CmdInfo Single<T>(IDictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = ReflectionHelper.GetInfo<T>().TableName;
            DbParameter[] ps = null;

            sqlbuilder.AppendFormat("SELECT * FROM `{0}`", tableName);
            ps = DictionaryToParams(conditions, sqlbuilder);
            sqlbuilder.Append(" limit 1");
            var cmd = new CmdInfo();
            cmd.CmdText = sqlbuilder.ToString();
            cmd.Params = ps;
            return cmd;
        }

        public override CmdInfo SingleById<T>(object idValue, string idField)
        {
            var sp = CreateParameter("@" + idField, idValue);

            var sql = string.Format("SELECT * FROM `{0}` WHERE `{1}`=@{1} limit 1", ReflectionHelper.GetInfo<T>().TableName, idField);
            return new CmdInfo
            {
                CmdText = sql,
                Params = new DbParameter[] { sp }
            };
        }

        public override CmdInfo SingleWhere<T>(object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = ReflectionHelper.GetInfo<T>().TableName;
            DbParameter[] ps = null;

            sqlbuilder.AppendFormat("SELECT * FROM `{0}`", tableName);
            ps = AnonTypeToParams(conditions, sqlbuilder);
            sqlbuilder.Append(" limit 1");
            return new CmdInfo
            {
                CmdText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public override CmdInfo SingleWhere<T>(IDictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = ReflectionHelper.GetInfo<T>().TableName;
            DbParameter[] ps = null;

            sqlbuilder.AppendFormat("SELECT * FROM `{0}`", tableName);
            ps = DictionaryToParams(conditions, sqlbuilder);
            sqlbuilder.Append(" limit 1");
            return new CmdInfo
            {
                CmdText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public override CmdInfo SingleWhere<T>(string name, object value)
        {
            var table = ReflectionHelper.GetInfo<T>().TableName;
            var p = CreateParameter("@" + name, value);

            var sql = string.Format("SELECT * FROM `{0}` WHERE `{1}`=@{1} limit 1 ", table, name);
            return new CmdInfo
            {
                CmdText = sql,
                Params = new DbParameter[] { p }
            };
        }

        public override CmdInfo TableMetaDataSql(string dbName)
        {
            var GetTablesSql = "SELECT TABLE_NAME Name,TABLE_COMMENT Description FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @dbName";
            var p = CreateParameter("@dbName", dbName);

            return new CmdInfo
            {
                CmdText = GetTablesSql,
                Params = new DbParameter[] { p }
            };

        }

        public override CmdInfo ColumnMetaDataSql(string dbName, string tableName)
        {
            var GetColumnsSql = @"select Column_Name Name,
case Column_Key when 'PRI' then 1 else 0 end IsPrimaryKey,
case ExTra when 'auto_increment' then 1 else 0 end IsIdentity,
 Column_comment Description,
data_type SqlDataType,
case Is_Nullable when 'NO' then 0 else 1 end IsNullable
 from information_schema.columns where table_schema = @dbName  and table_name = @tableName";
            var p_dbName = CreateParameter("@dbName", dbName);
            var p_tableName = CreateParameter("@tableName", tableName);

            return new CmdInfo
            {
                CmdText = GetColumnsSql,
                Params = new DbParameter[] { p_dbName, p_tableName }
            };

        }
    }
}
