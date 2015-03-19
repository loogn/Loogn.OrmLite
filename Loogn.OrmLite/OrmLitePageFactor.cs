using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    public class OrmLitePageFactor
    {
        public string Conditions { get; set; }
        public string TableName { get; set; }

        public string Fields { get; set; }

        public string OrderBy { get; set; }
        public object Params { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
