using Loogn.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
