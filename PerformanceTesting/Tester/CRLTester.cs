using CRL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTesting.Tester
{

    public class CRLTester : ITester
    {
        static CRLTester()
        {
            CRL.SettingConfig.GetDbAccess = (type) =>
            {
                return new CoreHelper.SqlHelper(DB.ConnStr);
            };
            CRL.SettingConfig.AutoTrackingModel = false;
        }

        public string Name => "CRL";
        DbContext dbContext = new DbContext(new CoreHelper.SqlHelper(DB.ConnStr), new DBLocation() { ManageType = typeof(CRLTester) });

        public List<TestEntity> GetList(int limit)
        {
            var db = DBExtendFactory.CreateDBExtend(dbContext);
            var list = db.ExecList<testentity>(string.Format("select top {0} * from TestEntity", limit));
            return null;
        }


        private AbsDBExtend context;
        public CRLTester()
        {
            context = DBExtendFactory.CreateDBExtend(dbContext);

        }
        public List<TestEntity> GetListSingleContent(int limit)
        {
            var list = context.ExecList<testentity>(string.Format("select top {0} * from TestEntity", limit));
            return null;
        }
    }
}
