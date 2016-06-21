# 简要
   ***Loogn.OrmLite是一个简单、高效的基于.NET Framework 4.0的数据访问组件！***   

# 特点
1. 支持sqlserver、mysql数据库；
2. 通过扩展方法扩展 _DbConnection_ 和 _DbTransaction_ ；
3. 支持数据库事务和批量插入
4. 支持 _dynamic_ 类型的模型（不同定义模型也可使用此ORM）
5. 超高的效率，超小的体积，比Dapper快，比Dapper小，Loogn.OrmLite.dll只有 **61kb** 

# 支持mysql
1. 引入MySql.Data.dll
2. 实现IOrmLiteProvider
```
public class MySqlOrmLiteProvider : IOrmLiteProvider
{
    private MySqlOrmLiteProvider() { }

    public static MySqlOrmLiteProvider Instance = new MySqlOrmLiteProvider();

    public DbParameter CreateParameter(string name, object value)
    {
        return new MySqlParameter(name, value);
    }
}
```
3. 注册mysql  **(只用在程序启动时执行一次)** 
```
OrmLite.RegisterProvider(OrmLiteProviderType.MySql, MySqlOrmLiteProvider.Instance);
```

#API预览
```
using (var db = new MySqlConnection(MySqlConnstr))
{
    int totalCount;
    var list = db.SelectPage<Person>(new OrmLitePageFactor
    {
        Conditions = "name like '%loogn%'",
        PageIndex = 2,
        PageSize = 2,
        OrderBy = "id"
    }, out totalCount);

    //select * from Person
    //var list1 = db.Select<Person>();
    
    //p.Age = 2;
    //Person p = new Person() { ID = 2 };
    
    //db.Update<Person>(p, "Age", "Sex");
    
    //select * from Person where ID>3
    //List<Person> list2 = db.Select<Person>("ID>3");
    
    //List<Person> list3 = db.Select<Person>("ID>@id", new { id = 3 });

    //select * from Person Where ID>@id
    //List<Person> list4 = db.Select<Person>("ID>@id", DictBuilder.Assign("id", 3));
    
    ////select top 10 ID,Name from Person
    //List<Person> list5 = db.Select<Person>("select ID,Name from Person limit 10");
    
    //select top 10 ID,Name from Person where ID>@id
    //List<Person> list6 = db.Select<Person>("select ID,Name from Person where ID>@id limit 10", new { id = 3 });
    //List<Person> list7 = db.Select<Person>("select ID,Name from Person where ID>@id limit 10", DictBuilder.Assign("id", 3));
    
    //select * from Person where id=@ID and name=@Name
    //List<Person> list8 = db.SelectWhere<Person>(new { ID = 2, Name = "loogn" });
    //List<Person> list9 = db.SelectWhere<Person>(DictBuilder.Assign("ID", 2).Assign("Name", "p1"));
    
    //select * from Person where Name=@Name
    //List<Person> list10 = db.SelectWhere<Person>("Name", "p1");
    
    //select * from Person where ID=23;
    //List<Person> list11 = db.SelectFmt<Person>("select * from Person where ID={0}", 23);
    
    //select * from Person where id in (1,2,3);
    //List<Person> list12 = db.SelectByIds<Person>(new int[] { 1, 2, 3 });
    
    //select * from Person where name in ('a','b','c');
    //var list23= db.SelectByIds<Person>(new string[] { "p1", "loogn", "c" }, "name");


    /************************************************************************************/

    //db.Single的AIP和db.Select相似

    /************************************************************************************/


    //db.Scalar的AIP和db.Select相似

    /************************************************************************************/

    //db.Count的AIP和db.Select相似

    /************************************************************************************/

    //得到name的List集合
    //List<string> col1 = db.Column<string>("select name from Person");
    //List<string> col2 = db.Column<string>("select name from Person where ID>=@id", new { id = 1 });
    //List<string> col3 = db.Column<string>("select name from Person where ID>=@id", DictBuilder.Assign("ID", 2));


    //得到ID的HashSet集合
    //HashSet<int> set1 = db.ColumnDistinct<int>("select distinct(id) from Person");
    //HashSet<int> set2 = db.ColumnDistinct<int>("select distinct(id) from Person where name=@n", new { n = "loogn" });
    //HashSet<int> set3 = db.ColumnDistinct<int>("select distinct(id) from Person where name=@n", DictBuilder.Assign("n", "loogn"));


    /************************************************************************************/


    //得到以id为Key,name为value的字典
    //Dictionary<int, string> dict1 = db.Dictionary<int, string>("select id,name from Person where id>2");
    
    //得到以type为Key，id集合为value的字典，即type和id为一对多关系
    //Dictionary<int, List<int>> dict2 = db.Lookup<int, int>("select type,id from Person where id>2");
    

    /************************************************************************************/


    //long rowcount = db.Insert<Person>(new Person { Name = "loognsdf" }, true);
    //Console.WriteLine(rowcount);
    //返回影响行数

    //long newid = db.Insert<Person>(new Person { Name = "loogn" }, true);
    ////返回新的自增编号

    //var flag = db.Insert<Person>(
    //    new Person { Name = "loogn6", AddDate = DateTime.Now },
    //    new Person { Name = "loogn43", AddDate = DateTime.Now },
    //    new Person { Name = "loogn78", AddDate = DateTime.Now },
    //    new Person { Name = "loogn87", AddDate = DateTime.Now },
    //    new Person { Name = "loogn23", AddDate = DateTime.Now },
    //    new Person { Name = "loogn8", AddDate = DateTime.Now });
    //Console.WriteLine(flag);
    //事务批量插入

    //List<Person> plist = new List<Person>() { 
    //    new Person { Name = "loogn1" , AddDate=DateTime.Now},
    //    new Person { Name = "loogn2", AddDate = DateTime.Now },
    //    new Person { Name = "loogn2", AddDate = DateTime.Now }
    //};
    //db.InsertAll<Person>(plist);
    //事务批量插入


    /************************************************************************************/


    //var s = db.Update<Person>(new Person { ID = 2, Name = "loogn", Age = 99, Sex = true });
    //update person set name=@name where id=23

    //var s = db.Update<Person>(DictBuilder.Assign("$age", "age+10"), "ID=@id", DictBuilder.Assign("id", 1));
    //Console.WriteLine(s);
    ////update Person set age=age+1 where id=@id

    /************************************************************************************/

    //db.Delete<Person>();
    ////delete from Person

    //db.Delete<Person>(DictBuilder.Assign("id", 23).Assign("name", "loogn"));
    ////delete from person where id=@id and name=@name

    //var s= db.DeleteById<Person>(5);
    //Console.WriteLine(s);
    ////delete from person where id=23

    //db.DeleteById<Person>("loogn", "name");
    ////delete from person where name=@name

    // db.DeleteByIds<Person>(new int[] { 1,3 });


    ////delete from person where id in (1,2,3);

    //db.DeleteByIds<Person>(new string[] { "a", "b", "c" }, "name");
    ////delete from person where name in ('a','b','c');
}
```


