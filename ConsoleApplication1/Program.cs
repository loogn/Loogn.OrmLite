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
            
            OrmLite.DefaultConnectionString = "server=.;uid=sa;pwd=123456;database=test";
            OrmLite.WriteSqlLog = true;
            using (var db = OrmLite.Open())
            {
                var all = db.Select<Person>("select top 10 * from Person");
                ShowList(all);
                db.EnsureOpen();
                var trans = db.BeginTransaction();
                try
                {
                    trans.Delete("delete from person where  ID>23");
                    trans.Insert<Person>(new Person { Name = "loogn", AddDate = DateTime.Now });
                    trans.Update<Person>(new Person { ID = 22, Name = "abc" });
                    trans.Commit();
                }
                catch(Exception exp)
                {
                    trans.Rollback();
                    Console.WriteLine(exp.Message);
                }
                return;
                
                List<Person> list1 = db.Select<Person>();
                //select * from Person

                List<Person> list2 = db.Select<Person>("ID>3");
                //select * from Person where ID>3

                List<Person> list3 = db.Select<Person>("ID>@id", new { id = 3 });
                List<Person> list4 = db.Select<Person>("ID>@id", DictBuilder.Assign("id", 3));
                //select * from Person Where ID>@id

                List<Person> list5 = db.Select<Person>("select top 10 ID,Name from Person");
                //select top 10 ID,Name from Person

                List<Person> list6 = db.Select<Person>("select top 10 ID,Name from Person where ID>@id", new { id = 3 });
                List<Person> list7 = db.Select<Person>("select top 10 ID,Name from Person where ID>@id", DictBuilder.Assign("id", 3));
                //select top 10 ID,Name from Person where ID>@id


                List<Person> list8 = db.SelectWhere<Person>(new { ID = 23, Name = "loogn" });
                List<Person> list9 = db.SelectWhere<Person>(DictBuilder.Assign("ID", 23).Assign("Name", "loogn"));
                //select * from Person where id=@ID and name=@Name

                List<Person> list10 = db.SelectWhere<Person>("Name", "loogn");
                //select * from Person where Name=@Name

                List<Person> list11 = db.SelectFmt<Person>("select * from Person where ID={0}", 23);
                //select * from Person where ID=23;

                List<Person> list12 = db.SelectByIds<Person>(new int[] { 1, 2, 3 });
                //select * from Person where id in (1,2,3);

                db.SelectByIds<Person>(new string[] { "a", "b", "c" }, "name");
                //select * from Person where name in ('a','b','c');

                /************************************************************************************/

                //db.Single的AIP和db.Select相似

                /************************************************************************************/

                //db.Scalar的AIP和db.Select相似

                /************************************************************************************/

                //db.Count的AIP和db.Select相似

                /************************************************************************************/

                List<string> col1 = db.Column<string>("select name from Person");
                List<string> col2 = db.Column<string>("select name from Person where ID=@id", new { id = 23 });
                List<string> col3 = db.Column<string>("select name from Person where ID=@id", DictBuilder.Assign("ID", 23));
                //得到name的List集合


                HashSet<int> set1 = db.ColumnDistinct<int>("select distinct(id) from Person");
                HashSet<int> set2 = db.ColumnDistinct<int>("select distinct(id) from Person where name=@n", new { n = "loogn" });
                HashSet<int> set3 = db.ColumnDistinct<int>("select distinct(id) from Person where name=@n", DictBuilder.Assign("n", "loogn"));
                //得到ID的不重复集合

                /************************************************************************************/

                Dictionary<int, string> dict1 = db.Dictionary<int, string>("select id,name from Person where id>2");
                //得到以id为Key,name为value的字典

                Dictionary<int, List<int>> dict2 = db.Lookup<int, int>("select type,id from Person where id>2");
                //得到以type为Key，id集合为value的字典，即type和id为一对多关系

                /************************************************************************************/

                long rowcount = db.Insert<Person>(new Person { Name = "loogn" });
                //返回影响行数

                long newid = db.Insert<Person>(new Person { Name = "loogn" }, true);
                //返回新的自增编号

                db.Insert<Person>(
                    new Person { Name = "loogn1" , AddDate=DateTime.Now},
                    new Person { Name = "loogn2", AddDate = DateTime.Now },
                    new Person { Name = "loogn2", AddDate = DateTime.Now });
                //事务批量插入

                List<Person> plist = new List<Person>() { 
                    new Person { Name = "loogn1" , AddDate=DateTime.Now},
                    new Person { Name = "loogn2", AddDate = DateTime.Now },
                    new Person { Name = "loogn2", AddDate = DateTime.Now }
                };
                db.InsertAll<Person>(plist);
                //事务批量插入

                /************************************************************************************/

                db.Update<Person>(new Person { ID = 23, Name = "loogn" });
                //update person set name=@name where id=23


                db.Update<Person>(DictBuilder.Assign("$age", "age+1"), "ID=@id", DictBuilder.Assign("id", 23));
                //update Person set age=age+1 where id=@id

                /************************************************************************************/

                db.Delete<Person>();
                //delete from Person

                db.Delete<Person>(DictBuilder.Assign("id", 23).Assign("name","loogn"));
                //delete from person where id=@id and name=@name

                db.DeleteById<Person>(23);
                //delete from person where id=23

                db.DeleteById<Person>("loogn", "name");
                //delete from person where name=@name

                db.DeleteByIds<Person>(new int[] { 1, 2, 3 });
                //delete from person where id in (1,2,3);

                db.DeleteByIds<Person>(new string[] { "a", "b", "c" }, "name");
                //delete from person where name in ('a','b','c');

                



                //var s= db.Insert("Person", new { Name = "名字sdfsdf", AddDate = DateTime.Now },true);


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
