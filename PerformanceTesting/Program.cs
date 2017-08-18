using Loogn.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loogn.OrmLite;
using PerformanceTesting.Tester;

namespace PerformanceTesting
{
    class Program
    {

        static void Main(string[] args)
        {
            //TesterRunner.TestMapping(500000);
            TesterRunner.TestRepeat(2000, 100);
            //TesterRunner.TestRepeatSingleContext(2000, 100);

        }
    }
}
