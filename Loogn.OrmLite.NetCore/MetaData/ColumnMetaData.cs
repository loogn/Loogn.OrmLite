using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loogn.OrmLite.NetCore.MetaData
{
    /// <summary>
    /// 数据列元数据
    /// </summary>
    public class ColumnMetaData
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否可为空
        /// </summary>
        public bool IsNullable { get; set; }
        /// <summary>
        /// 是否是标识列（自增）
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// sql数据类型
        /// </summary>
        public string SqlDataType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 返回csharp类型
        /// </summary>
        /// <returns></returns>
        public string GetDataType()
        {
            return MetaDataHelper.SqlDataType2DataType(SqlDataType);
        }
    }
}
