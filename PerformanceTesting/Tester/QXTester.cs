using QX_Frame.Helper_DG.Bantina;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTesting.Tester
{
    class QXTester : ITester
    {
        public string Name
        {
            get
            {
                return "qx_frame";
            }
        }

        DB_QX_Frame_Test context = new DB_QX_Frame_Test();
        public List<TestEntity> GetList(int limit)
        {
            using (var db = new DB_QX_Frame_Test())
            {
                var list = db.ExecuteSqlToList<TestEntity>(string.Format("select top {0} * from TestEntity", limit.ToString()));
                return list;
            }
        }

        public List<TestEntity> GetListSingleContent(int limit)
        {
            var list = context.ExecuteSqlToList<TestEntity>(string.Format("select top {0} * from TestEntity", limit.ToString()));
            return list;
        }
    }

    class DB_QX_Frame_Test : Bantina
    {
        public DB_QX_Frame_Test() : base(DB.ConnStr) { }
    }
}
