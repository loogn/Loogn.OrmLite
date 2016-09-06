using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class OrmLiteFieldAttribute : Attribute
    {
        public OrmLiteFieldAttribute()
        {
        }

        /// <summary>
        /// 为Ture时，ID列也会插入
        /// </summary>
        public bool InsertRequire { get; set; }

        /// <summary>
        /// 为Ture时，整体插入时会忽略该字段，如自增
        /// </summary>
        public bool InsertIgnore { get; set; }
        /// <summary>
        /// 为Ture时，整体修改时会忽略该字段，如较大的字段
        /// </summary>
        public bool UpdateIgnore { get; set; }
        /// <summary>
        /// 为Ture时，说明该字段是主键，整体修改时用它作为修改条件
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 为Ture时，说明该字段不对应表字段
        /// </summary>
        public bool Ignore { get; set; }

    }
}
