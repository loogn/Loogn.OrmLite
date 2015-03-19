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
            using (var db = OrmLite.Open())
            {
                //var list = db.Select("select * from Person");//返回List<dynamic>

                //var list = db.Select<Person>();//select * from Person

                //var list = db.Select<Person>("select * from Person where ID>@id", new Dictionary<string, object> { { "id", 23 } });
                ////或
                //var list = db.Select<Person>("select * from Person where ID>@id", DictBuilder.Assign("id", 23));


                //var list = db.SelectWhere<Person>("name", "loogn");
                ////select * from Person where name='loogn'

                //var list = db.SelectWhere<Person>(DictBuilder.Assign("name", "loogn").Assign("Age", 23));
                ////select * from person where name='loogn' and age=23

                //var list = db.SelectByIds<Person>(new int[] { 1, 2, 3 });
                ////select * from person where id in (1,2,3)

                //var list = db.SelectByIds<Person>(new string[] { "loogn", "王胜龙" }, "Name");
                ////select * from person where name in ('loogn','王胜龙')

                //var list = db.SelectFmt<Person>("select top {0} id,name from person where id={1}", 10, 23);
                ////select top 10 id,name from person where id=23

                //long totalCount = 0;
                //var list = db.SelectPage<Person>(new OrmLitePageFactor
                //{
                //    Conditions = "id>1",
                //    OrderBy = "id desc",
                //    PageIndex = 1,
                //    PageSize = 10
                //}, out totalCount);
                ////分页

                //var obj = db.Single<Person>(DictBuilder.Assign("id", 2));
                ////select top 1 * from person where id=2

                //var obj = db.Single<Person>("select top 1 id ,name from person where name=@n", DictBuilder.Assign("n", "loogn"));
                ////select top 1 id,name from person where name='loogn'

                //var obj = db.SingleById<Person>(23);
                ////select top 1 * from person where id=23;

                //var obj = db.SingleWhere<Person>("id", 23);
                ////select top 1 * from person where id=23;

                //var collist = db.Column<string>("select name from person");
                ////List<string> name集合
                //var colSet = db.ColumnDistinct<string>("select distinct(name) from person");
                // // HashSet<string> name集合(不重复)

                //var rowCount= db.Insert<Person>(new Person { Name = "loogn" }); //返回影响行数，也就是1
                //var newid = db.Insert<Person>(new Person { Name = "loogn" },  true); //返回插入的编号


                //var p1=new Person{ Name="p1"};
                //var p2=new Person{ Name="p2"};
                //db.Insert<Person>(p1, p2);//批量

                //db.InsertAll<Person>(new Person[] { p1, p2 });//批量

                //db.Update<Person>(new Person{ ID=1, Name="2323"});
                //db.Update<Person>(p1, p2);//批量
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
