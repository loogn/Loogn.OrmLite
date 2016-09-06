using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loogn.OrmLite
{
    class SqliteCmd : MySqlCmd
    {

        static SqliteCmd instance;
        public static SqliteCmd Instance
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
    }
}
