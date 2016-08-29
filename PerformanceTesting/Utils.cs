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

        public static void ShowObject(object obj)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
        }
    }
}
