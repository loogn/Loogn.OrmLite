using Loogn.OrmLite;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Sex { get; set; }
        public byte Age { get; set; }
        public double Money { get; set; }
        public DateTime AddDate { get; set; }

    }

    class Program
    {
        static void Main(string[] args)
        {
            OrmLite.SetDefaultConnectionString("server=.;uid=sa;pwd=123456;database=test");
            using (var db = OrmLite.Open(true))
            {
                var obj = db.SelectWhere<Person>(DictBuilder.Assign("name", "loogn"));
                foreach (var ob in obj)
                {
                    Console.WriteLine(ob.Name);
                }
            }
        }

        static void ShowList(params Person[] persons)
        {
            Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", "ID", "Name", "Sex", "Age", "Money", "AddDate");
            foreach (var person in persons)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", person.ID, person.Name, person.Sex, person.Age, person.Money, person.AddDate);
            }
        }
        static void ShowList(List<Person> list)
        {
            Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", "ID", "Name", "Sex", "Age", "Money", "AddDate");
            foreach (var person in list)
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", person.ID, person.Name, person.Sex, person.Age, person.Money, person.AddDate);
            }
        }
    }
}
