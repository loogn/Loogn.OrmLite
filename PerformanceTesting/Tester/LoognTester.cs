using Loogn.OrmLite;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTesting.Tester
{
    public class LoognTester : ITester
    {
        public string Name => "Loogn";
        public List<TestEntity> GetList(int limit)
        {
            using (var db = DB.CreateConnection())
            {
                var list = db.SelectFmt<TestEntity>("select top {0} * from TestEntity", limit);
                return list;
            }
        }
        private SqlConnection context = DB.CreateConnection();
        public List<TestEntity> GetListSingleContent(int limit)
        {
            var list = context.SelectFmt<TestEntity>("select top {0} * from TestEntity", limit);
            return list;
        }
    }
}
