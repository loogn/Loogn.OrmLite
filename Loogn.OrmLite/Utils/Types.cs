using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    static class Types
    {
        public static Type OrmLiteTable = typeof(OrmLiteTableAttribute);
        public static Type OrmLiteField = typeof(OrmLiteFieldAttribute);
        public static Type String = typeof(string);
        public static Type Int32 = typeof(Int32);
    }
}
