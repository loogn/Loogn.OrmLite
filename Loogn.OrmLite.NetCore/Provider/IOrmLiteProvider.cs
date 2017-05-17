using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite.NetCore
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

    internal class WrapOrmLiteProvider : IOrmLiteProvider
    {
        IOrmLiteProvider _provider;
        public WrapOrmLiteProvider(IOrmLiteProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// 处理参数的长度问题，用段固定长度，期望能利用上数据库的执行计划缓存
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DbParameter CreateParameter(string name, object value)
        {
            var p = _provider.CreateParameter(name, value);
            var size = GetParameterSize(value);
            if (size != null)
            {
                p.Size = size.Value;
            }
            return p;
        }

        /// <summary>
        /// 获取参数分段长度
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static int? GetParameterSize(object value)
        {
            if (value == null || value is DBNull) return null;
            if (value is string)
            {
                var length = ((string)value).Length;
                if (length <= 100) return 100;
                if (length <= 200) return 200;
                if (length <= 500) return 500;
                if (length <= 1000) return 1000;
                if (length <= 2000) return 2000;
                if (length <= 5000) return 5000;
                if (length <= 8000) return 8000;
                return null;
            }
            return null;
        }
    }
}
