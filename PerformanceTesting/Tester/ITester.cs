using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTesting.Tester
{
    public interface ITester
    {
        List<TestEntity> GetList(int limit);
        string Name { get; }

        List<TestEntity> GetListSingleContent(int limit);

    }
}
