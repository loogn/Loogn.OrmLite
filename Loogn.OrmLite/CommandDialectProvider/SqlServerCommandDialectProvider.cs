using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace Loogn.OrmLite
{
    public class SqlServerCommandDialectProvider : CommandDialectBaseProvider
    {

        public static SqlServerCommandDialectProvider Instance = new SqlServerCommandDialectProvider();

        static SqlServerCommandDialectProvider()
        {
            OrmLite.RegisterProvider(Instance);
        }

        private SqlServerCommandDialectProvider() { }
        public override string CloseQuote
        {
            get
            {
                return "]";
            }
        }

        public override string IsNullFunc
        {
            get
            {
                return "ISNULL";
            }
        }

        public override string OpenQuote
        {
            get
            {
                return "[";
            }
        }

        public override CommandInfo ColumnMetaData(string dbName, string tableName)
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
                                               AND p.major_id = c.id  AND p.name='MS_Description' 
        LEFT JOIN syscomments m ON c.cdefault = m.id
WHERE   c.id = OBJECT_ID(@tableName)
ORDER BY colorder ASC
";
            var p = CreateParameter("tableName", tableName);
            return new CommandInfo
            {
                CommandText = GetColumnsSql,
                Params = new IDbDataParameter[] { p }
            };
        }

        public override IDbConnection CreateConnection()
        {
            return new SqlConnection();
        }

        public override IDbDataParameter CreateParameter()
        {
            return new SqlParameter();
        }

        public override CommandInfo FullSingle<T>(string sqlOrCondition)
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
            sql = sb.AppendFormat("SELECT top 1 * FROM {2}{0}{3} where {1}", tableName, sql, OpenQuote, CloseQuote).ToString();
            return new CommandInfo
            {
                CommandText = sql,
            };
        }

        public override string GetLastInsertId()
        {
            return "SELECT ISNULL(SCOPE_IDENTITY(),@@rowcount)";
        }

        public override CommandInfo Paged(OrmLitePageFactor factor)
        {
            StringBuilder sb = new StringBuilder(100);
            sb.AppendFormat("select * from (");


            sb.AppendFormat(" select top {0} {1},ROW_NUMBER() over(order by {2}) rowid from {3}", factor.PageIndex * factor.PageSize, factor.Fields, factor.OrderBy, factor.TableName);
            if (!string.IsNullOrEmpty(factor.Conditions))
            {
                sb.AppendFormat(" where {0}", factor.Conditions);
            }
            sb.AppendFormat(")t where t.rowid>{0}", (factor.PageIndex - 1) * factor.PageSize);
            return new CommandInfo
            {
                CommandText = sb.ToString(),
            };
        }

        public override CommandInfo SingleById<T>(object idValue, string idField)
        {
            var sp = CreateParameter(idField, idValue);
            var sql = string.Format("SELECT top 1 * FROM [{0}] WHERE [{1}]=@{1}", GetTableName<T>(), idField);

            return new CommandInfo
            {
                CommandText = sql,
                Params = new IDbDataParameter[] { sp }
            };
        }

        public override CommandInfo SingleWhere<T>(IDictionary<string, object> conditions, string orderBy)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);

            sqlbuilder.AppendFormat("SELECT top 1 * FROM [{0}]", GetTableName<T>());
            var ps = this.Dictionary2Params(conditions, sqlbuilder);
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlbuilder.Append(" order by ").Append(orderBy);
            }
            return new CommandInfo
            {
                CommandText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public override CommandInfo SingleWhere<T>(object conditions, string orderBy)
        {
            StringBuilder sqlbuilder = new StringBuilder(50);

            sqlbuilder.AppendFormat("SELECT top 1 * FROM [{0}]", GetTableName<T>());
            var ps = this.Object2Params(conditions, sqlbuilder);
            if (!string.IsNullOrEmpty(orderBy))
            {
                sqlbuilder.Append(" order by ").Append(orderBy);
            }
            return new CommandInfo
            {
                CommandText = sqlbuilder.ToString(),
                Params = ps
            };
        }

        public override CommandInfo SingleWhere<T>(string name, object value, string orderBy)
        {
            var p = CreateParameter(name, value);
            var sql = string.Format("SELECT top 1 * FROM [{0}] WHERE [{1}]=@{1} ", GetTableName<T>(), name);
            if (!string.IsNullOrEmpty(orderBy))
            {
                sql += " order by " + orderBy;
            }
            return new CommandInfo
            {
                CommandText = sql,
                Params = new IDbDataParameter[] { p }
            };
        }

        public override CommandInfo TableMetaData(string dbName)
        {
            string GetTablesSql = @"
SELECT  t.Name ,
        CONVERT(VARCHAR(200),ISNULL(p.value, ''))  Description
FROM    sysobjects t
        LEFT JOIN sys.extended_properties p ON t.id = p.major_id
                                               AND p.minor_id = 0 and p.name='MS_Description' 
WHERE   t.xtype = 'U'
        AND t.name <> 'dtproperties'
ORDER BY t.name";
            return new CommandInfo
            {
                CommandText = GetTablesSql,
                Params = new IDbDataParameter[] { }
            };
        }
    }
}
