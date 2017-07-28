using ConsoleApplication1.GeneralParadigm.Model;
using Loogn.OrmLite;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;


namespace ConsoleApplication1
{
    //指定表明
    [OrmLiteTable("t_person")]
    public class Person
    {
        //整体修改时使用主键，如果自增可以使用InsertIgnore指定插入时忽略
        [OrmLiteField(IsPrimaryKey = true, InsertIgnore = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public DateTime UpdateTime { get; set; }

        //整体修改时忽略
        [OrmLiteField(UpdateIgnore = true)]
        public DateTime AddTime { get; set; }

        //整体修改，添加时忽略
        [OrmLiteField(Ignore = true)]
        public List<string> SomeAttrs { get; set; }
    }

    public static class DB
    {
        public static IDbConnection Open()
        {
            return new SqlConnection("server=.;uid=sa;pwd=sa;database=test");
        }

        public static IDbConnection OpenOther()
        {
            return new SqlConnection("server=.;uid=sa;pwd=sa;database=other");
        }
    }

    class Program
    {
        static string MySqlConnstr = "Server=192.168.2.254;Uid=admin;Password=123456;Database=test;Port=3308;TreatTinyAsBoolean=false";

        static void Main(string[] args)
        {
            using (var db = DB.Open())
            {
                db.SingleById<Person>(1);
                db.Single<Person>("select ID,Name from Person where age>10 and sex=1");



            }

        }



        static void ShowList(params Person[] persons)
        {
            Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", "ID", "Name", "Sex", "Age", "Money", "AddDate");
            foreach (var person in persons)
            {
                //Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", person.ID, person.Name, person.Sex, person.Age, person.Money, person.AddDate);
            }
        }
        static void ShowList(List<Person> list)
        {
            Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", "ID", "Name", "Sex", "Age", "Money", "AddDate");
            foreach (var person in list)
            {
                //Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", person.ID, person.Name, person.Sex, person.Age, person.Money, person.AddDate);
            }
        }

    }
}
