using Chloe;
using Chloe.SqlServer;
using Loogn.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Loogn.OrmLite;
using System.Data.SqlClient;

namespace PerformanceTesting
{
    class QueryTester
    {
        static int queryCount = 20000;
        static int limit = 1;
        static int minId = 1;
        public static void Test()
        {
            Chloe(2);
            ChloeSql(2);
            Dapper(2);
            EF(2);
            EFSql(2);
            Loogn(2);
            CRL(2);

            Console.WriteLine(string.Format("Query count:{0},Limit {1} ", queryCount, limit));
            CodeTimer.Initialize();


            CodeTimer.Time("Query-Chloe", queryCount, () =>
            {
                Chloe(limit);
            });

            CodeTimer.Time("Query-ChloeSql", queryCount, () =>
            {
                ChloeSql(limit);
            });

            CodeTimer.Time("Query-Dapper", queryCount, () =>
            {
                Dapper(limit);
            });

            CodeTimer.Time("Query-EF", queryCount, () =>
            {
                EF(limit);
            });
            CodeTimer.Time("Query-EFSql", queryCount, () =>
            {
                EFSql(limit);
            });

            CodeTimer.Time("Query-Loogn", queryCount, () =>
            {
                Loogn(limit);
            });

            CodeTimer.Time("Query-CRL", queryCount, () =>
            {
                CRL(limit);
            });

        }

        static void Chloe(int limit)
        {
            using (var context = new MsSqlContext(Utils.ConnStr))
            {
                var list = context.Query<TestEntity_Chloe>().Where(a => a.Id > minId).Take(limit).ToList();
            }
        }

        static void ChloeSql(int limit)
        {
            using (var context = new MsSqlContext(Utils.ConnStr))
            {
                var list = context.SqlQuery<TestEntity_Chloe>(string.Format("select top {0} * from TestEntity where Id>@Id", limit.ToString()), DbParam.Create("@Id", minId)).ToList();
            }
        }

        static void Dapper(int limit)
        {
            using (var conn = Utils.CreateConnection())
            {
                var list = conn.Query<TestEntity>(string.Format("select top {0} * from TestEntity where Id>@Id", limit.ToString()), new { Id = minId }).ToList();
            }
        }

        static void EF(int limit)
        {
            using (EFContext efContext = new EFContext())
            {
                var list = efContext.TestEntity.AsNoTracking().Where(a => a.Id > minId).Take(limit).ToList();
            }
        }

        static void EFSql(int limit)
        {
            using (EFContext efContext = new EFContext())
            {
                var list = efContext.Database.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity where Id>@Id", limit.ToString()), new SqlParameter("@Id", minId)).ToList();
            }
        }

        static void Loogn(int limit)
        {
            using (var db = Utils.CreateConnection())
            {
                var list = db.Select<TestEntity>(string.Format("select top {0} * from TestEntity where Id>@Id", limit.ToString()), DictBuilder.Assign("Id", minId));
            }
        }

        static void CRL(int limit)
        {
            CRLProvider provider = new CRLProvider();
            var query = provider.GetLambdaQuery();
            query.Where(x => x.Id > minId);
            query.Top(limit);
            var list = query.ToList();
        }
    }
}
