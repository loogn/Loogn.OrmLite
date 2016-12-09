using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Loogn.OrmLite
{
    class SqlServerCmd : BaseCmd
    {

        static SqlServerCmd instance;
        public static SqlServerCmd Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SqlServerCmd();
                    return instance;
                }
                return instance;
            }
        }

        public SqlServerCmd()
        {
            provider = OrmLite.GetProvider(OrmLiteProviderType.SqlServer);
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
            return sb.AppendFormat("SELECT top 1 * FROM {2}{0}{3} where {1}", tableName, sql, L(), R()).ToString();

        }

        public override string GetLastInsertID()
        {
            return "SELECT ISNULL(SCOPE_IDENTITY(),@@rowcount)";
        }

        public override string IFNULL()
        {
            return "ISNULL";
        }

        public override string L()
        {
            return "[";
        }

        public override string PageSql(OrmLitePageFactor factor)
        {
            StringBuilder sb = new StringBuilder(100);
            sb.AppendFormat("select * from (");
            sb.AppendFormat(" select top {0} {1},ROW_NUMBER() over(order by {2}) rowid from {3}", factor.PageIndex * factor.PageSize, factor.Fields, factor.OrderBy, factor.TableName);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            sb.AppendFormat(")t where t.rowid>{0}", (factor.PageIndex - 1) * factor.PageSize);
            return sb.ToString();
        }

        public override string R()
        {
            return "]";
        }

        public override CmdInfo Single<T>(object conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = ReflectionHelper.GetInfo<T>().TableName;
            DbParameter[] ps = null;

            sqlbuilder.AppendFormat("SELECT top 1 * FROM [{0}]", tableName);
            ps = AnonTypeToParams(conditions, sqlbuilder);
            return new CmdInfo
            {
                CmdText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public override CmdInfo Single<T>(Dictionary<string, object> conditions)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);
            var tableName = ReflectionHelper.GetInfo<T>().TableName;
            DbParameter[] ps = null;

            sqlbuilder.AppendFormat("SELECT top 1 * FROM [{0}]", tableName);
            ps = DictionaryToParams(conditions, sqlbuilder);
            var cmd = new CmdInfo();
            cmd.CmdText = sqlbuilder.ToString();
            cmd.Params = ps;
            return cmd;
        }

        public override CmdInfo SingleById<T>(object idValue, string idField)
        {
            var sp = CreateParameter("@" + idField, idValue);
            var sql = string.Format("SELECT top 1 * FROM [{0}] WHERE [{1}]=@{1}", ReflectionHelper.GetInfo<T>().TableName, idField);

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

            sqlbuilder.AppendFormat("SELECT top 1 * FROM [{0}]", tableName);
            ps = AnonTypeToParams(conditions, sqlbuilder);

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

            sqlbuilder.AppendFormat("SELECT top 1 * FROM [{0}]", tableName);
            ps = DictionaryToParams(conditions, sqlbuilder);
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
            var sql = string.Format("SELECT top 1 * FROM [{0}] WHERE [{1}]=@{1} ", table, name);

            return new CmdInfo
            {
                CmdText = sql,
                Params = new DbParameter[] { p }
            };
        }
    }
}
