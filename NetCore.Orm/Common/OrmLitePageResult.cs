using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loogn.OrmLite
{
    /// <summary>
    /// 分页数据信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OrmLitePageResult<T>
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public long PageCount
        {
            get
            {
                if (TotalCount == 0 || PageSize == 0)
                {
                    return 0;
                }
                var mod = TotalCount % PageSize;
                if (mod > 0)
                {
                    return TotalCount / PageSize + 1;
                }
                else
                {
                    return TotalCount / PageSize;
                }
            }
        }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }


        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页数据
        /// </summary>
        public List<T> List { get; set; }

    }
}
