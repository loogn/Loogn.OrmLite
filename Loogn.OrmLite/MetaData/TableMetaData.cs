using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loogn.OrmLite.MetaData
{
    /// <summary>
    /// 数据表元数据
    /// </summary>
    public class TableMetaData
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 表描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 列元数据
        /// </summary>
        public List<ColumnMetaData> Columns { get; set; }
    }
}
