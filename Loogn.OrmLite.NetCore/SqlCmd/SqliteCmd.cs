using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loogn.OrmLite.NetCore
{
    class SqliteCmd : MySqlCmd
    {

        static SqliteCmd instance;
        public new static SqliteCmd Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SqliteCmd();
                    return instance;
                }
                return instance;
            }
        }

        public SqliteCmd()
        {
            provider = OrmLite.GetProvider(OrmLiteProviderType.Sqlite);
        }

        public override string GetLastInsertID()
        {
            return "select last_insert_rowid()";
        }

        public override CmdInfo ColumnMetaDataSql(string dbName, string tableName)
        {
            throw new NotImplementedException("未实现sqlite数据库的元数据查询");
        }
        public override CmdInfo TableMetaDataSql(string dbName)
        {
            throw new NotImplementedException("未实现sqlite数据库的元数据查询");
        }
    }
}
