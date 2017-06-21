using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    public class CommandInfo
    {
        public string CommandText;
        public IDbDataParameter[] Params;
        public CommandType CommandType = CommandType.Text;

    }
}
