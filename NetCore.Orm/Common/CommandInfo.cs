using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    /// <summary>
    /// 表示一个SQL命令
    /// </summary>
    public class CommandInfo
    {
        /// <summary>
        /// 命令文本
        /// </summary>
        public string CommandText;
        /// <summary>
        /// 参数
        /// </summary>
        public IDbDataParameter[] Params;
        /// <summary>
        /// 命令类型
        /// </summary>
        public CommandType CommandType = CommandType.Text;

    }
}
