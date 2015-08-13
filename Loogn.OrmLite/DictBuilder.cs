using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    public static class DictBuilder
    {
        public static ChainOperateDict Assign(string name, object value)
        {
            ChainOperateDict dict = new ChainOperateDict();
            return dict.Assign(name, value);
        }

        public static ChainOperateDict New()
        {
            return new ChainOperateDict();
        }
    }

    public class ChainOperateDict : Dictionary<string, object>
    {
        public ChainOperateDict Assign(string name, object value)
        {
            base[name] = value;
            return this;
        }
    }
}
