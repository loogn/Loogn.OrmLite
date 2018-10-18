
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Loogn.OrmLite.Sqlite
{
    public class SqliteCommandDialectProvider : CommandDialectBaseProvider
    {
        public static SqliteCommandDialectProvider Instance = new SqliteCommandDialectProvider();
        private SqliteCommandDialectProvider() { }

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
            throw new NotImplementedException("未实现sqlite数据库的元数据查询");
        }

        public override IDbConnection CreateConnection()
        {

            return new SQLiteConnection();
        }

        public override IDbDataParameter CreateParameter()
        {
            return new SQLiteParameter();
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
            return "select last_insert_rowid()";
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

        public override CommandInfo SingleWhere<T>(IDictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = GetTableName<T>();
            IDbDataParameter[] ps = null;

            sqlbuilder.AppendFormat("SELECT * FROM `{0}`", tableName);
            ps = this.Dictionary2Params(conditions, sqlbuilder);
            sqlbuilder.Append(" limit 1");
            return new CommandInfo
            {
                CommandText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public override CommandInfo SingleWhere<T>(object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = GetTableName<T>();
            IDbDataParameter[] ps = null;

            sqlbuilder.AppendFormat("SELECT * FROM `{0}`", tableName);
            ps = this.Object2Params(conditions, sqlbuilder);
            sqlbuilder.Append(" limit 1");
            return new CommandInfo
            {
                CommandText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public override CommandInfo SingleWhere<T>(string name, object value)
        {
            var table = GetTableName<T>();
            var p = CreateParameter("@" + name, value);

            var sql = string.Format("SELECT * FROM `{0}` WHERE `{1}`=@{1} limit 1 ", table, name);
            return new CommandInfo
            {
                CommandText = sql,
                Params = new IDbDataParameter[] { p }
            };
        }

        public override CommandInfo TableMetaData(string dbName)
        {
            throw new NotImplementedException("未实现sqlite数据库的元数据查询");
        }
    }
}
