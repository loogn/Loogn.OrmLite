using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    public enum OrmLiteProviderType
    {
        SqlServer,
        MySql
    }


    public interface IOrmLiteProvider
    {
        DbConnection CreateConnection();
        DbParameter CreateParameter(string name, object value);
    }

    internal static class EM
    {
        public static OrmLiteProviderType GetProviderType(this DbConnection conn)
        {
            var connTypeName = conn.GetType().Name;

            if (connTypeName == "SqlConnection")
            {
                return OrmLiteProviderType.SqlServer;
            }

            else if (connTypeName == "MySqlConnection")
            {
                return OrmLiteProviderType.MySql;
            }
            throw new ArgumentException("OrmLiteProviderType 参数错误");
        }

        public static OrmLiteProviderType GetProviderType(this DbTransaction trans)
        {
            var transTypeName = trans.GetType().Name;

            if (transTypeName == "SqlTransaction")
            {
                return OrmLiteProviderType.SqlServer;
            }

            else if (transTypeName == "MySqlTransaction")
            {
                return OrmLiteProviderType.MySql;
            }
            throw new ArgumentException("OrmLiteProviderType 参数错误");
        }
    }
}
