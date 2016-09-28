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
using ServiceStack.OrmLite;
using SqlSugar;
using CRL;

namespace PerformanceTesting
{
    class QueryTester
    {
        //static int queryCount = 20000;
        //static int limit = 10;
        static int queryCount = 1000;
        static int limit = 20;

        static int minId = 1;
        public static void Test()
        {
            //预热
            //Chloe(2);
            ChloeSql(2);
            Dapper(2);
            //EF(2);
            EFSql(2);
            Loogn(2);
            CRL(2);
            SqlSugar(2);
            ServiceStack(2);


            Console.WriteLine(string.Format("Query count:{0},Limit {1} ", queryCount, limit));
            CodeTimer.Initialize();


            //CodeTimer.Time("Query-Chloe", queryCount, () =>
            //{
            //    Chloe(limit);
            //});

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

            CodeTimer.Time("Query-SqlSugar", queryCount, () =>
            {
                SqlSugar(limit);
            });


            CodeTimer.Time("Query-CRL", queryCount, () =>
            {
                CRL(limit);
            });


            CodeTimer.Time("Query-ServiceStack", queryCount, () =>
            {
                ServiceStack(limit);
            });
        }

        static void Chloe(int limit)
        {
            using (var context = new MsSqlContext(Utils.ConnStr))
            {
                var list = context.Query<TestEntity>().Where(a => a.Id > minId).Take(limit).ToList();
            }
        }

        static void ChloeSql(int limit)
        {
            using (var context = new MsSqlContext(Utils.ConnStr))
            {
                var list = context.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity where Id>@Id", limit.ToString()), DbParam.Create("@Id", minId)).ToList();
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
                var list = db.SelectFmt<TestEntity>("select top {0} * from TestEntity where Id>{1}", limit.ToString(), minId.ToString());
            }
        }

        static void CRL(int limit)
        {
            var dbContext = new CRL.DbContext(new CoreHelper.SqlHelper(Utils.ConnStr), new DBLocation() { ManageType = typeof(QueryTester) });
            var db = DBExtendFactory.CreateDBExtend(dbContext);

            var list = db.ExecList<testentity>(string.Format("select top {0} * from TestEntity where Id>{1}", limit.ToString(), minId.ToString()));
        }


        static void SqlSugar(int limit)
        {
            using (var db = new SqlSugarClient(Utils.ConnStr))
            {
                var list = db.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity where Id>{1}", limit.ToString(), minId.ToString()));
            }
        }

        static void ServiceStack(int limit)
        {
            var dbFactory = new OrmLiteConnectionFactory(Utils.ConnStr, SqlServerDialect.Provider);
            using (var db = dbFactory.Open())
            {
                var list = db.Select<TestEntity>(string.Format("select top {0} * from TestEntity where ID>@id", limit), DictBuilder.Assign("id", minId));
            }
        }
    }
}
