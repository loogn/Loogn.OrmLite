using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTesting.Tester
{
    public class ServiceStackTester : ITester
    {
        public string Name => "ServiceStack";

        OrmLiteConnectionFactory dbFactory = new OrmLiteConnectionFactory(DB.ConnStr, SqlServerDialect.Provider);
        public List<TestEntity> GetList(int limit)
        {
            using (var db = dbFactory.Open())
            {
                var list = db.Select<TestEntity>(string.Format("select top {0} * from TestEntity", limit));
                return list;
            }
        }

        IDbConnection context;
        public ServiceStackTester()
        {
            context = dbFactory.Open();
        }

        public List<TestEntity> GetListSingleContent(int limit)
        {
            var list = context.Select<TestEntity>(string.Format("select top {0} * from TestEntity", limit));
            return list;
        }
    }
}
