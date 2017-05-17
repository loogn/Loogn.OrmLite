using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Loogn.OrmLite.NetCore
{
    /// <summary>
    /// 实体类特性，用户类上，指明表名
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class OrmLiteTableAttribute : Attribute
    {
        /// <summary>
        /// 实体类特性
        /// </summary>
        public OrmLiteTableAttribute() : this(string.Empty) { }

        /// <summary>
        /// 实体类特性
        /// </summary>
        /// <param name="name">数据库表名</param>
        public OrmLiteTableAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// 指定表名，推荐模型类命名为"表名Info"，不用指定
        /// </summary>
        public string Name { get; set; }
    }
}
