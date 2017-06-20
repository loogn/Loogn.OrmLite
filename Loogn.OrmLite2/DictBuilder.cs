using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite2
{
    /// <summary>
    /// 链式字典生成器
    /// </summary>
    public static class DictBuilder
    {
        /// <summary>
        /// 新生成链式，并添加提供的键值
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static ChainOperateDict Assign(string name, object value)
        {
            ChainOperateDict dict = new ChainOperateDict();
            return dict.Assign(name, value);
        }

        /// <summary>
        /// 生成链式字典
        /// </summary>
        /// <returns></returns>
        public static ChainOperateDict New()
        {
            return new ChainOperateDict();
        }
    }

    /// <summary>
    /// 链式字典，继承自Dictionary 
    /// </summary>
    public class ChainOperateDict : Dictionary<string, object>
    {
        /// <summary>
        /// 往字典里赋值
        /// </summary>
        /// <param name="name">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public ChainOperateDict Assign(string name, object value)
        {
            base[name] = value;
            return this;
        }
    }
}
