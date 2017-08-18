using Loogn.Utils;
using PerformanceTesting.Tester;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTesting
{
    public class TesterRunner
    {
        public static void TestRepeat(int repeat, int limit)
        {
            Console.WriteLine("查询{0}次，每次{1}条", repeat, limit);
            var testers = GetTesters(limit);

            CodeTimer.Initialize();

            foreach (var tester in testers)
            {
                CodeTimer.Time(tester.Name, repeat, () =>
                {
                    tester.GetList(limit);
                });
            }
        }

        public static void TestMapping(int limit)
        {
            TestRepeat(1, limit);
        }

        public static void TestRepeatSingleContext(int repeat, int limit)
        {
            Console.WriteLine("查询{0}次，每次{1}条", repeat, limit);
            var testers = GetTesters(limit);

            CodeTimer.Initialize();

            foreach (var tester in testers)
            {
                CodeTimer.Time(tester.Name, repeat, () =>
                {
                    tester.GetListSingleContent(limit);
                });
            }
        }

        static List<ITester> GetTesters(int limit)
        {
            List<ITester> list = new List<ITester>
            {
                new EFSqlTester(),
                new CRLTester(),
                new DapperTester(),
                new ServiceStackTester(),
                new SqlSugarTester(),
                new LoognTester()
            };
            //数据库预热
            list.First().GetList(limit);
            list.First().GetList(limit);
            //类库预热
            for (int i = 0; i < 5; i++)
            {
                foreach (var tester in list)
                {
                    tester.GetList(10);
                }
            }
            return list;
        }

    }
}
