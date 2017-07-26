using Chloe;
using Chloe.SqlServer;
using CRL;
using Dapper;
using Loogn.OrmLite;
using Loogn.Utils;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTesting
{
    class SingleContextQueryTester
    {

        static int queryCount = 2000;
        static int limit = 10;
        static int minId = 1;
        public static void Test()
        {
            queryCount = 1;

            //Chloe(2);
            ChloeSql(2);
            Dapper(2);
            //EF(2);
            EFSql(2);
            LoognOrmLite(2);
            CRL(2);
            // ServiceStack(2);

            queryCount = 20000;

            Console.WriteLine(string.Format("SingleContextQuery count:{0},Limit {1} ", queryCount, limit));
            CodeTimer.Initialize();


            //CodeTimer.Time("SingleContextQuery-Chloe", 1, () =>
            //{
            //    Chloe(limit);
            //});

            //CodeTimer.Time("SingleContextQuery-ChloeSql", 1, () =>
            //{
            //    ChloeSql(limit);
            //});

            //CodeTimer.Time("SingleContextQuery-Dapper", 1, () =>
            //{
            //    Dapper(limit);
            //});

            //CodeTimer.Time("SingleContextQuery-EF", 1, () =>
            //{
            //    EF(limit);
            //});
            //CodeTimer.Time("SingleContextQuery-EFSql", 1, () =>
            //{
            //    EFSql(limit);
            //});

            CodeTimer.Time("SingleContextQuery-Loogn", 1, () =>
            {
                LoognOrmLite(limit);
            });

            CodeTimer.Time("SingleContextQuery-CRL", 1, () =>
            {
                CRL(limit);
            });


            CodeTimer.Time("SingleContextQuery-SqlSugar", 1, () =>
            {
                SqlSugar(limit);
            });


            //CodeTimer.Time("SingleContextQuery-ServiceStack", 1, () =>
            //{
            //    ServiceStack(limit);
            //});


        }

        static void Chloe(int limit)
        {
            using (var context = new MsSqlContext(Utils.ConnStr))
            {
                for (int i = 0; i < queryCount; i++)
                {
                    var list = context.Query<TestEntity>().Where(a => a.Id > minId).Take(limit).ToList();
                }
            }
        }

        static void ChloeSql(int limit)
        {
            using (var context = new MsSqlContext(Utils.ConnStr))
            {
                for (int i = 0; i < queryCount; i++)
                {
                    var list = context.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity where Id>@Id", limit.ToString()), DbParam.Create("@Id", minId)).ToList();
                }
            }
        }

        static void Dapper(int limit)
        {
            using (var conn = Utils.CreateConnection())
            {
                for (int i = 0; i < queryCount; i++)
                {
                    var list = conn.Query<TestEntity>(string.Format("select top {0} * from TestEntity where Id>@Id", limit.ToString()), new { Id = minId }).ToList();
                }
            }
        }

        static void EF(int limit)
        {
            using (EFContext efContext = new EFContext())
            {
                for (int i = 0; i < queryCount; i++)
                {
                    var list = efContext.TestEntity.AsNoTracking().Where(a => a.Id > minId).Take(limit).ToList();
                }
            }
        }

        static void EFSql(int limit)
        {
            using (EFContext efContext = new EFContext())
            {
                for (int i = 0; i < queryCount; i++)
                {
                    var list = efContext.Database.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity where Id>@Id", limit.ToString()), new SqlParameter("@Id", minId)).ToList();
                }
            }
        }

        static void LoognOrmLite(int limit)
        {
            using (var db = Utils.CreateConnection())
            {
                for (int i = 0; i < queryCount; i++)
                {
                    //var list = Loogn.OrmLite.OrmLiteReadApi.Select<TestEntity>(db, "select top " + limit.ToString() + " * from TestEntity where Id>@id", DictBuilder.Assign("id", minId));
                    var list = db.Select<TestEntity>(string.Format("select top {0} * from TestEntity where Id>{1}", limit.ToString(), minId));
                }
            }
        }

        static void CRL(int limit)
        {
            var dbContext = new CRL.DbContext(new CoreHelper.SqlHelper(Utils.ConnStr), new DBLocation() { ManageType = typeof(SingleContextQueryTester) });
            var db = DBExtendFactory.CreateDBExtend(dbContext);
            for (int i = 0; i < queryCount; i++)
            {
                var list = db.ExecList<testentity>(string.Format("select top {0} * from TestEntity where Id>{1}", limit.ToString(), minId));
            }
        }

        static void SqlSugar(int limit)
        {
            var db = new SqlSugarClient(Utils.ConnStr);
            using (db)
            {
                for (int i = 0; i < queryCount; i++)
                {
                    var list = db.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity where Id>{1}", limit.ToString(), minId));
                }

            }
        }

        //static void ServiceStack(int limit)
        //{
        //    var dbFactory = new OrmLiteConnectionFactory(Utils.ConnStr, SqlServerDialect.Provider);
        //    using (var db = dbFactory.Open())
        //    {
        //        for (int i = 0; i < queryCount; i++)
        //        {
        //            var list = db.Select<TestEntity>(string.Format("select top {0} * from TestEntity where ID>@id", limit), DictBuilder.Assign("id", minId));
        //        }
        //    }
        //}
    }
}
