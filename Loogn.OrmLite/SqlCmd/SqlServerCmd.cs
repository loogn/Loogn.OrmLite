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

            var l = L();
            var r = R();
            if (factor.TableName.IndexOf(" ") > 0)
            {
                l = "";
                r = "";
            }

            sb.AppendFormat(" select top {0} {1},ROW_NUMBER() over(order by {2}) rowid from {4}{3}{5}", factor.PageIndex * factor.PageSize, factor.Fields, factor.OrderBy, factor.TableName, l, r);
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

        public override CmdInfo TableMetaDataSql(string dbName)
        {
            string GetTablesSql = @"
SELECT  t.Name ,
        ISNULL(p.value, '') Description
FROM    sysobjects t
        LEFT JOIN sys.extended_properties p ON t.id = p.major_id
                                               AND p.minor_id = 0
WHERE   t.xtype = 'U'
        AND t.name <> 'dtproperties'
ORDER BY t.name";
            return new CmdInfo
            {
                CmdText = GetTablesSql,
                Params = new DbParameter[] { }
            };
        }

        public override CmdInfo ColumnMetaDataSql(string dbName, string tableName)
        {
            string GetColumnsSql = @"
SELECT  c.Name ,
        c.IsNullable ,
        ( CASE WHEN COLUMNPROPERTY(c.id, c.name, 'IsIdentity') = 1 THEN 1
               ELSE 0
          END ) IsIdentity ,
        ( CASE WHEN ( SELECT    COUNT(*)
                      FROM      sysobjects
                      WHERE     ( name IN (
                                  SELECT    name
                                  FROM      sysindexes
                                  WHERE     ( id = c.id )
                                            AND ( indid IN (
                                                  SELECT    indid
                                                  FROM      sysindexkeys
                                                  WHERE     ( id = c.id )
                                                            AND ( colid IN (
                                                              SELECT
                                                              colid
                                                              FROM
                                                              syscolumns
                                                              WHERE
                                                              ( id = c.id )
                                                              AND ( name = c.name ) ) ) ) ) ) )
                                AND ( xtype = 'PK' )
                    ) > 0 THEN 1
               ELSE 0
          END ) IsPrimaryKey ,
        t.name AS SqlDataType ,
        ISNULL(p.value, '') Description 
FROM    syscolumns c
        LEFT JOIN systypes t ON c.xtype = t.xtype
                                AND t.name <> 'sysname'
        LEFT JOIN sys.extended_properties p ON c.colid = p.minor_id
                                               AND p.major_id = c.id
        LEFT JOIN syscomments m ON c.cdefault = m.id
WHERE   c.id = OBJECT_ID(@tableName)
ORDER BY colorder ASC
";
            var p = CreateParameter("@tableName", tableName);
            return new CmdInfo
            {
                CmdText = GetColumnsSql,
                Params = new DbParameter[] { p }
            };
        }
    }
}
