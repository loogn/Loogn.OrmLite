using MySql.Data.MySqlClient;
using System;
using Loogn.OrmLite;
using Loogn.OrmLite.MySql;

namespace OrmLiteTester
{
    class Program
    {

        static string MySqlConn = "server=192.168.1.95;port=3306;uid=loogn;pwd=loogn;database=ProjectTemplate";
        static MySqlConnection OpenMySql()
        {
            return new MySqlConnection(MySqlConn);
        }



        static void F(EnumGender? g)
        {
            Console.WriteLine(g);
        }


        static void Main(string[] args)
        {

            OrmLite.RegisterProvider(MySqlCommandDialectProvider.Instance);

            using(var db = OpenMySql())
            {
                var m = db.SingleById<SysUser>(1);
                Console.WriteLine($"性别:{m.Gender}");
                Console.WriteLine($"状态:{m.Status}");
            }

            
        }
    }
}
