using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    /// <summary>
    /// 提供程序接口
    /// </summary>
    public interface IOrmLiteProvider
    {
        /// <summary>
        /// 创建对应提供程序的参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        DbParameter CreateParameter(string name, object value);
    }
}
