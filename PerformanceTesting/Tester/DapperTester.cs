using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTesting.Tester
{
    public class DapperTester : ITester
    {
        public string Name => "Dapper";
        public List<TestEntity> GetList(int limit)
        {
            using (var conn = DB.CreateConnection())
            {
                var list = conn.Query<TestEntity>(string.Format("select top {0} * from TestEntity", limit.ToString())).ToList();
                return list;
            }
        }

        private SqlConnection context = DB.CreateConnection();

        public List<TestEntity> GetListSingleContent(int limit)
        {
            var list = context.Query<TestEntity>(string.Format("select top {0} * from TestEntity", limit.ToString())).ToList();
            return list;
        }
    }
}
