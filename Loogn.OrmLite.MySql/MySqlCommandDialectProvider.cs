using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite.MySql
{
    public class MySqlCommandDialectProvider : CommandDialectBaseProvider
    {
        public static MySqlCommandDialectProvider Instance = new MySqlCommandDialectProvider();
        private MySqlCommandDialectProvider() { }

        public override string CloseQuote
        {
            get
            {
                return "`";
            }
        }

        public override string IsNullFunc
        {
            get
            {
                return "IFNULL";
            }
        }

        public override string OpenQuote
        {
            get
            {
                return "`";
            }
        }

        public override CommandInfo ColumnMetaData(string dbName, string tableName)
        {

            var GetColumnsSql = @"select Column_Name Name,
case Column_Key when 'PRI' then 1 else 0 end IsPrimaryKey,
case ExTra when 'auto_increment' then 1 else 0 end IsIdentity,
 Column_comment Description,
data_type SqlDataType,
case Is_Nullable when 'NO' then 0 else 1 end IsNullable
 from information_schema.columns where table_schema = @dbName  and table_name = @tableName";
            var p_dbName = CreateParameter("dbName", dbName);
            var p_tableName = CreateParameter("tableName", tableName);

            return new CommandInfo
            {
                CommandText = GetColumnsSql,
                Params = new IDbDataParameter[] { p_dbName, p_tableName }
            };

        }

        public override IDbConnection CreateConnection()
        {
            return new MySqlConnection();
        }

        public override IDbDataParameter CreateParameter()
        {
            return new MySqlParameter();
        }

        public override CommandInfo FullSingle<T>(string sqlOrCondition)
        {
            var sql = sqlOrCondition.TrimStart();
            if (sql.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return new CommandInfo { CommandText = sql };
            }
            var tableName = GetTableName<T>();
            StringBuilder sb = new StringBuilder(sql.Length + 50);
            return new CommandInfo
            {
                CommandText = sb.AppendFormat("SELECT * FROM `{0}` where {1} limit 1", tableName, sql).ToString()
            };
        }

        public override string GetLastInsertId()
        {
            return "SELECT LAST_INSERT_ID()";
        }

        public override CommandInfo Paged(OrmLitePageFactor factor)
        {
            StringBuilder sb = new StringBuilder(100);

            sb.AppendFormat("select {0} from {1}", factor.Fields, factor.TableName);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            sb.AppendFormat(" order by {0} limit {1},{2}", factor.OrderBy, (factor.PageIndex - 1) * factor.PageSize, factor.PageSize);
            return new CommandInfo
            {
                CommandText = sb.ToString()
            };
        }

        public override CommandInfo SingleById<T>(object idValue, string idField)
        {
            var sp = CreateParameter(idField, idValue);
            var sql = string.Format("SELECT * FROM `{0}` WHERE `{1}`=@{1} limit 1", GetTableName<T>(), idField);
            return new CommandInfo
            {
                CommandText = sql,
                Params = new IDbDataParameter[] { sp }
            };
        }

        public override CommandInfo SingleWhere<T>(IDictionary<string, object> conditions, string orderBy)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = GetTableName<T>();
            IDbDataParameter[] ps = null;

            sqlbuilder.AppendFormat("SELECT * FROM `{0}`", tableName);
            ps = this.Dictionary2Params(conditions, sqlbuilder);
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlbuilder.Append(" order by ").Append(orderBy);
            }
            sqlbuilder.Append(" limit 1");
            return new CommandInfo
            {
                CommandText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public override CommandInfo SingleWhere<T>(object conditions, string orderBy)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = GetTableName<T>();
            IDbDataParameter[] ps = null;

            sqlbuilder.AppendFormat("SELECT * FROM `{0}`", tableName);
            ps = this.Object2Params(conditions, sqlbuilder);
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlbuilder.Append(" order by ").Append(orderBy);
            }
            sqlbuilder.Append(" limit 1");
            return new CommandInfo
            {
                CommandText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public override CommandInfo SingleWhere<T>(string name, object value, string orderBy)
        {
            var tableName = GetTableName<T>();
            var p = CreateParameter(name, value);
            StringBuilder sqlbuilder = new StringBuilder(50);
            sqlbuilder.AppendFormat("SELECT * FROM `{0}` WHERE `{1}`=@{1}", tableName, name);
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlbuilder.Append(" order by ").Append(orderBy);
            }
            sqlbuilder.Append(" limit 1");

            return new CommandInfo
            {
                CommandText = sqlbuilder.ToString(),
                Params = new IDbDataParameter[] { p }
            };
        }

        public override CommandInfo TableMetaData(string dbName)
        {
            var GetTablesSql = "SELECT TABLE_NAME Name,TABLE_COMMENT Description FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @dbName";
            var p = CreateParameter("dbName", dbName);

            return new CommandInfo
            {
                CommandText = GetTablesSql,
                Params = new IDbDataParameter[] { p }
            };
        }
    }
}
