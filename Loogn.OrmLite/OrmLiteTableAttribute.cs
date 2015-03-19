using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class OrmLiteTableAttribute : Attribute
    {
        public OrmLiteTableAttribute() : this(string.Empty) { }

        public OrmLiteTableAttribute(string name)
        {
            this.Name = name;
        }
        /// <summary>
        /// 指定表名，推荐模型类命名为"表名Info"，不用指定
        /// </summary>
        public string Name { get; set; }
    }
}
