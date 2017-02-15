using Loogn.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loogn.OrmLite;


namespace PerformanceTesting
{
    class Program
    {

        static void init()
        {
            CRL.SettingConfig.GetDbAccess = (type) =>
            {
                return new CoreHelper.SqlHelper(Utils.ConnStr);
            };
            CRL.SettingConfig.AutoTrackingModel = false;

        }

        static void Main(string[] args)
        {

            //TestMutileResult();
            //return;

            init();

            //QueryTester.Test();
            //QueryTester.Test();
            //QueryTester.Test();
            //QueryTester.Test();
            //QueryTester.Test();
            //QueryTester.Test();


            MappingTester.Test();
            //QueryTester.Test();
            
            //SingleContextQueryTester.Test();
        }

        static void TestMutileResult()
        {
            using (var db = Utils.CreateConnection())
            {
                var cmds = new MutipleCmd[] {
                    new MutipleCmd { CmdText="select top 2 * from TestEntity" },
                    new MutipleCmd { CmdText="select top 1 * from TestEntity where F_Int64>@fint", Params=new Dictionary<string, object> { {"fint",23 } } },
                    new MutipleCmd { CmdText="select top 1 F_Int64 from TestEntity where F_Int64>@fint",Params=new Dictionary<string, object> { {"fint",20 } } },
                };
                var result = db.SelectMutipleResult(cmds);

                var list = result.FetchList<TestEntity>();
                var entity = result.FetchObject<TestEntity>();
                var f_int64 = result.FetchScalar<long>();

                Console.WriteLine(list[1].F_String);
                Console.WriteLine(entity.F_String);
                Console.WriteLine(f_int64);
            }
        }
    }
}
