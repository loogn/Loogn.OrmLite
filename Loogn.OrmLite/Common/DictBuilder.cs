using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
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
            return new ChainOperateDict().Assign(name, value);
        }

        /// <summary>
        /// 新生成链式，并添加!=条件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ChainOperateDict NEQ(string name, object value)
        {
            return new ChainOperateDict().NEQ(name, value);
        }

        /// <summary>
        /// 新生成链式，并添加=条件，和Assign相同
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ChainOperateDict EQ(string name, object value)
        {
            return new ChainOperateDict().EQ(name, value);
        }

        /// <summary>
        /// 新生成链式，并添加大于条件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ChainOperateDict GT(string name, object value)
        {
            return new ChainOperateDict().GT(name, value);
        }

        /// <summary>
        /// 新生成链式，并添加大于等于条件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ChainOperateDict GTE(string name, object value)
        {
            return new ChainOperateDict().GTE(name, value);
        }

        /// <summary>
        /// 新生成链式，并添加小于条件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ChainOperateDict LT(string name, object value)
        {
            return new ChainOperateDict().LT(name, value);
        }

        /// <summary>
        /// 新生成链式，并添加小于等于条件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ChainOperateDict LTE(string name, object value)
        {
            return new ChainOperateDict().LTE(name, value);
        }

        /// <summary>
        /// 新生成链式，并添加like条件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ChainOperateDict LIKE(string name, object value)
        {
            return new ChainOperateDict().LIKE(name, value);
        }

        /// <summary>
        /// 新生成链式，并添加in条件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ChainOperateDict IN(string name, object value)
        {
            return new ChainOperateDict().IN(name, value);
        }

        /// <summary>
        /// 新生成链式，并添加notin条件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ChainOperateDict NOTIN(string name, object value)
        {
            return new ChainOperateDict().NOTIN(name, value);
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

        /// <summary>
        /// 和Assign相同
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ChainOperateDict EQ(string name, object value)
        {
            base[name] = value;
            return this;
        }

        /// <summary>
        /// 生成不等于操作参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ChainOperateDict NEQ(string name, object value)
        {
            base[name + "$!="] = value;
            return this;
        }

        /// <summary>
        /// 生成大于操作参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ChainOperateDict GT(string name, object value)
        {
            base[name + "$>"] = value;
            return this;

        }

        /// <summary>
        /// 生成大于等于操作参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ChainOperateDict GTE(string name, object value)
        {
            base[name + "$>="] = value;
            return this;
        }

        /// <summary>
        /// 生成小于操作参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ChainOperateDict LT(string name, object value)
        {
            base[name + "$<"] = value;
            return this;
        }
        /// <summary>
        /// 生成小于等于操作参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ChainOperateDict LTE(string name, object value)
        {
            base[name + "$<="] = value;
            return this;
        }

        /// <summary>
        /// 生成like操作参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ChainOperateDict LIKE(string name, object value)
        {
            base[name + "$like"] = value;
            return this;
        }

        /// <summary>
        /// 生成in操作参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ChainOperateDict IN(string name, object value)
        {
            base[name + "$in"] = value;
            return this;
        }
        /// <summary>
        /// 生成notin操作参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public ChainOperateDict NOTIN(string name, object value)
        {
            base[name + "$notin"] = value;
            return this;
        }
    }
}
