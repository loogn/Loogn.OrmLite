using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTesting.Tester
{
    public class SqlSugarTester : ITester
    {
        public string Name => "SqlSugar";
        public List<TestEntity> GetList(int limit)
        {
            using (var db = new SqlSugarClient(connConfig))
            {
                var list = db.Ado.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity", limit));
                return list;
            }
        }
        ConnectionConfig connConfig = new ConnectionConfig { ConnectionString = DB.ConnStr, DbType = DbType.SqlServer };
        SqlSugarClient context;
        public SqlSugarTester()
        {
            context = new SqlSugarClient(connConfig);
        }

        public List<TestEntity> GetListSingleContent(int limit)
        {
            var list = context.Ado.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity", limit));
            return list;
        }
    }
}
