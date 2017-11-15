using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
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
        /// 指定表名
        /// </summary>
        public string Name { get; set; }
    }
}
