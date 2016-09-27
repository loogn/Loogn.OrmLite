using Chloe.SqlServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Loogn.OrmLite;
using CRL.LambdaQuery;
using Loogn.Utils;
using ServiceStack.OrmLite;
using SqlSugar;
using CRL.DBExtend.RelationDB;
using CoreHelper;
using CRL;

namespace PerformanceTesting
{
    public class MappingTester
    {
        static int limit = 5000000;
        //static int limit = 500000;
        public static void Test()
        {

            //预热
            //Chloe(2);
            ChloeSql(2);
            Dapper(2);
            //EF(2);
            EFSql(2);
            Loogn(2);
            SqlSugar(2);
            CRL(2);
            ServiceStack(2);



            Console.WriteLine("Mapping count: " + limit);
            CodeTimer.Initialize();


            //CodeTimer.Time("Mapping-Chloe", 1, () =>
            //{
            //    Chloe(limit);
            //});

            CodeTimer.Time("Mapping-ChloeSql", 1, () =>
            {
                ChloeSql(limit);
            });

            CodeTimer.Time("Mapping-Dapper", 1, () =>
            {
                Dapper(limit);
            });

            //CodeTimer.Time("Mapping-EF", 1, () =>
            //{
            //    EF(limit);
            //});

            CodeTimer.Time("Mapping-EFSql", 1, () =>
            {
                EFSql(limit);
            });

            CodeTimer.Time("Mapping-Loogn", 1, () =>
            {
                Loogn(limit);
            });

            CodeTimer.Time("Mapping-CRL", 1, () =>
            {
                CRL(limit);
            });

            CodeTimer.Time("Mapping-SqlSugar", 1, () =>
            {
                SqlSugar(limit);
            });

            CodeTimer.Time("Mapping-ServiceStack", 1, () =>
            {
                ServiceStack(limit);
            });
        }

        static void Chloe(int limit)
        {
            using (var context = new MsSqlContext(Utils.ConnStr))
            {
                var list = context.Query<TestEntity>().Take(limit).ToList();

            }
        }

        static void ChloeSql(int limit)
        {
            using (var context = new MsSqlContext(Utils.ConnStr))
            {
                var list = context.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity", limit.ToString())).ToList();

            }
        }

        static void Dapper(int limit)
        {
            using (var conn = Utils.CreateConnection())
            {
                var list = conn.Query<TestEntity>(string.Format("select top {0} * from TestEntity", limit.ToString())).ToList();

            }
        }

        static void EF(int limit)
        {
            using (EFContext efContext = new EFContext())
            {
                var list = efContext.TestEntity.AsNoTracking().Take(limit).ToList();

            }
        }

        static void EFSql(int limit)
        {
            using (EFContext efContext = new EFContext())
            {
                var list = efContext.Database.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity", limit.ToString())).ToList();

            }
        }

        static void Loogn(int limit)
        {
            using (var db = Utils.CreateConnection())
            {
                var list = db.SelectFmt<TestEntity>("select top {0} * from TestEntity", limit);
            }
        }

        static void CRL(int limit)
        {
            var dbContext = new DbContext(new CoreHelper.SqlHelper(Utils.ConnStr), new DBLocation() { ManageType = typeof(MappingTester) });
            var db = DBExtendFactory.CreateDBExtend(dbContext);

            var list = db.ExecList<testentity>(string.Format("select top {0} * from TestEntity", limit));
        }

        static void ServiceStack(int limit)
        {
            var dbFactory = new OrmLiteConnectionFactory(Utils.ConnStr, SqlServerDialect.Provider);
            using (var db = dbFactory.Open())
            {
                var list = db.Select<TestEntity>(string.Format("select top {0} * from TestEntity", limit));
            }
        }

        static void SqlSugar(int limit)
        {
            using (SqlSugarClient db = new SqlSugarClient(Utils.ConnStr))
            {

                var list = db.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity", limit));
            }
        }
    }
}
