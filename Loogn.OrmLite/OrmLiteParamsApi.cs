using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    public static partial class OrmLiteParamsApi
    {
        public static SqlParameter CreateParameter(this SqlConnection dbConn, string parameterName, object value)
        {
            SqlParameter p = new SqlParameter(parameterName, value);
            return p;
        }

        public static SqlParameter CreateReturnParameter(this SqlConnection dbConn, SqlDbType dbType)
        {
            SqlParameter p = new SqlParameter("@retValue", dbType);
            p.Direction = ParameterDirection.ReturnValue;
            return p;
        }

        public static SqlParameter CreateOutputParameter(this SqlConnection dbConn, string parameterName, SqlDbType dbType)
        {
            SqlParameter p = new SqlParameter(parameterName, dbType);
            p.Direction = ParameterDirection.Output;
            return p;
        }
    }
}
