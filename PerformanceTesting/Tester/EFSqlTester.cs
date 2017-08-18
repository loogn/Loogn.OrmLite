
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTesting.Tester
{
    public class EFSqlTester : ITester
    {
        public string Name => "EFSql";

        public List<TestEntity> GetList(int limit)
        {
            using (EFContext efContext = new EFContext())
            {
                var list = efContext.Database.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity", limit.ToString())).ToList();
                return list;
            }
        }
        EFContext _context = new EFContext();


        public List<TestEntity> GetListSingleContent(int limit)
        {
            var list = _context.Database.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity", limit.ToString())).ToList();
            return list;
        }
    }


    public class EFContext : DbContext
    {
        public EFContext() : base("name=test")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<EFContext>(null);
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.PluralizingTableNameConvention
 >();
        }

        public DbSet<TestEntity> TestEntity { get; set; }
    }
}
