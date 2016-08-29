using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTesting
{
    class Utils
    {
        public static string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["test"].ConnectionString;

        public static SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnStr);
        }
    }
}
