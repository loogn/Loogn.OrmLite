# 简要
   ***Loogn.OrmLite是一个超简单、超高效、超灵活的基于.net standard 2.0的数据访问组件！***   

# 特点
1. 支持sqlserver、mysql、sqlite3数据库；
2. 通过扩展方法扩展 _IDbConnection_ 和 _IDbTransaction_ ；
3. 支持数据库事务和批量插入
4. 支持 _dynamic_ 类型的模型（不定义模型也可使用此ORM）
5. 超高的效率，超小的体积，比Dapper快，比Dapper小，Loogn.OrmLite.dll只有 **85kb** 


# Getting Strrarted

一、 引入Loogn.OrmLite

```
    PM> Install-Package Loogn.OrmLite
```

二、引入名称空间

```csharp
    using Loogn.OrmLite;
```

三、开始使用

```csharp
   using (var db = new SqlConnection("server=.;uid=sa;pwd=sa;database=test"))
   {
       var flag = db.Insert(new Person { Id = 23 });
       if (flag > 0)
       {
           var person = db.SingleById<Person>(23);
       }
   }
```

一般可以把连接对象的创建封装起来：

```csharp
    public static class DB
    {
        public static IDbConnection Open()
        {
            return new SqlConnection("server=.;uid=sa;pwd=sa;database=test");
        }

        //public static IDbConnection OpenOther()
        //{
        //    return new SqlConnection("server=.;uid=sa;pwd=sa;database=other");
        //}
    }
```

使用起来就更简单了：

```csharp
    using (var db = DB.Open())
    {
        // do something with db
    }
```

# 偷懒做法
Orm中提供有AbstractDao，在具体项目中可以继承AbstractDao，提供一个连接字符串，做为dao层的基类

```csharp
    public class BaseDao<TEntity> : AbstractDao<TEntity>
    {
        protected override IDbConnection Open()
        {
            return new SqlConnection(ConnectionStringsSection.Instance.Db2);
        }
        // 这里可以提供项目中dao层的公共方法，不过AbstractDao已提供了很多，一般不需要
    }
```
dao类基本就不用写什么代码了
```csharp
    public class OrderDao:BaseDao<Order>
    {
        // 如果有需要，这里还是可以写dao层的东西的
    }
```

# Model

一个纯净的Model

```csharp
   public class Person
   {
       public int Id { get; set; }
       public string Name { get; set; }
       public DateTime UpdateTime { get; set; }
       public DateTime AddTime { get; set; }
   }
```

一个带有Attribute的Model

```csharp
    //指定表明
    [OrmLiteTable("t_person")]
    public class Person
    {
        //整体修改时使用主键，如果自增可以使用InsertIgnore指定插入时忽略
        [OrmLiteField(IsPrimaryKey = true, InsertIgnore = true)] 
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime UpdateTime { get; set; }

        //整体修改时忽略
        [OrmLiteField(UpdateIgnore = true)] 
        public DateTime AddTime { get; set; }

        //整体修改，添加时忽略
        [OrmLiteField(Ignore = true)] 
        public List<string> SomeAttrs { get; set; }
    }
```

# 插入

- 使用model插入，返回影响行数

```csharp
    var person = new Person { Name = "loogn" };
    var flag = db.Insert(person);
```

- 使用model插入，返回自增列值（long）

```csharp
    var person = new Person { Name = "loogn" };
    var autoId = db.Insert(person,true);
```

- 使用匿名对象插入，返回影响行数

```csharp
    var flag = db.Insert("Person",new { Name="loogn" });
```

- 使用匿名对象插入，返回自增列值（long）

```csharp
    var autoId = db.Insert("Person",new { Name="loogn" },true);
```

- 使用字典插入，返回影响行数

```csharp
    var flag = db.Insert("Person",DictBuilder.Assign("Name","loogn").Assign("Age",23));
```

- 使用字典插入，返回自增列值（long）

```csharp
    var autoId = db.Insert("Person",DictBuilder.Assign("Name","loogn").Assign("Age",23),true);
```

> DictBuilder生成的是 Dictionary<string, object>的子类

- 使用模型批量插入

```csharp
    var list = new List<Person>()
    {
         new Person{ Name="p1"},
         new Person{ Name="p2"}
    };
    var boolFlag = db.InsertAll(list);
```

- 使用匿名对象批量插入

```csharp
    var list = new List<object>()
    {
            new { Name="p1"},
            new { Name="p2"}
    };
    var boolFlag = db.InsertAll("Person",list);
```

# 修改

- 根据主键修改model，返回影响行数

```csharp
    var person = new Person { Id = 23, Name = "update name", Age = 28 };
    var flag = db.Update(person);
    //or 指定修改列
    var flag = db.Update(person,"Name","Age");
```

- 根据条件修改指定列

```csharp
    var fields = DictBuilder.Assign("name", "updateName").Assign("$age", "age+1");
    var ps = DictBuilder.Assign("id", 23);
    var flag = db.Update<Person>(fields, "id=@id", ps);
    //or
    var flag = db.Update("person",fields, "id=@id", ps);
```

> fields中 $age列表示不参数化，直接使用后面的值，如上会生成 update person set name=@name,age=age+1 where id=23

- 根据主键修改指定列

```csharp
    var fields = DictBuilder.Assign("name", "updateName").Assign("$age", "age+1");
    var flag = db.UpdateById<Person>(fields, 23);
    //or
    var flag = db.UpdateById("person", fields, 23);
```

- 根据某列修改指定列

```csharp
    var fields = DictBuilder.Assign("name", "updateName").Assign("$age", "age+1");
    var flag = db.UpdateById<Person>(fields,"loogn", "Name");
    //or
    var flag = db.UpdateById("person", fields, "loogn", "Name");
```

- 根据主键修改某一列

```csharp
    var flag = db.UpdateFieldById<Person>("name","update name",1);
    //update person set name=@name where id=1; @name='update name'
```

- 根据某列修改某一列

```csharp
    var flag = db.UpdateFieldById<Person>("name","update name",23,"age");
    //update person set name=@name where age=23; @name='update name'
```

- 匿名对象修改,表和匿名对象要包含ID列

```csharp
    var flag = db.UpdateAnonymous("person", new {Name="new name",Id=2});
    //or 
    var flag = db.UpdateAnonymous<Person>(new {Name="new name",Id=2});
    //update person set Name=@name where Id=2 ; @name ="new name"
```

- 批量修改model

```csharp
    var list = new List<Person>()
    {
          new Person{ Id=1, Name="new name1"},
          new Person{ Id=2, Name="new name2"}
    };
    var flag = db.UpdateAll(list);
```

# 查询

- 根据id查询一条数据

```csharp
    var person=db.SingleById<Person>(1);
    //or
    var person=db.SingleById<Person>(1,"_id");
```

- 根据条件查询一条数据

```csharp
    var person=db.Single<Person>("age>10 and sex=1");
    //select * from person where age>10 and sex=1

    var person=db.Single<Person>("age>10 and Name=@name", DictBuilder.Assign("name", "abc"));
    //select * from person where age>10 and Name=@name; @name='abc'

    var person=db.Single<Person>("select ID,Name from Person where age>10 and sex=1");
    //原始sql
    var person=db.Single<Person>("select ID,Name from Person where age>10 and name=@name", DictBuilder.Assign("name", "abc"));
    //原始sql带参数
```

- 参数化查询语句

```csharp
    var person=db.SingleFmt<Person>("select ID,Name from Person where age>{0} and sex={1}",10,1);
```

- 根据单个查询条件查询单条数据

```csharp
    var person= db.SingleWhere<Person>("id",23);
```

- 使用字典条件查询单条数据

```csharp
    var person= db.SingleWhere<Person>(DictBuilder.Assign("name","loogn").Assign("age",23));
```

- 使用匿名对象作为条件查询单条数据

```csharp
    var person= db.SingleWhere<Person>(new{ Name = "loogn", Age=23});
```

- 查询全部

```csharp
    var list=db.Select<Person>();
```

- in查询

```csharp
    var list=db.SelectByIds<Person>(new int[] { 1, 2, 3 });
    //select * from person where id in (1,2,3);
    var list=db.SelectByIds<Person>(new string[] { "1", "2", "3" },"userId");
    //select * from person where userId in ("1","2","3");
    var list=db.SelectByIds<Person>(new string[] { "1", "2", "3" },"userId","id,name");
    //select id,name from person where userId in ("1","2","3");
```

- 返回多个结果集

```csharp
    var cmds = new MutipleCmd[] {
            new MutipleCmd{ CmdText="select * from person"},
            new MutipleCmd{ CmdText="select count(0) from person where id=@id",Params=DictBuilder.Assign("id",23)},
            new MutipleCmd{ CmdText="select top 1 * from User where age>23"},
    };
    using (var fetcher = db.SelectMutipleResult(cmds))
    {
        var personList = fetcher.FetchList<Person>();   //对应第一个命令
        var personCount = fetcher.FetchScalar<int>();   //对应第二个命令
        var user = fetcher.FetchObject<User>();         //对应第三个命令
    }
```

> 其他Select和上面Single方法类似

- 分页查询

```csharp
    var pageResult = db.SelectPage<Person>(new OrmLitePageFactor
    {
        Conditions = "id>2 and age=@age",
        Fields = "ID,Name",
        OrderBy = "ID desc",
        PageIndex = 1,
        PageSize = 10,
        Params = DictBuilder.Assign("age", 23)
    });
    //也可以连接查询
    var pageResult = db.SelectPage<Person>(new OrmLitePageFactor
    {
        Conditions = "p.id>2 and p.age=@age",
        Fields = "p.*,t.Name as TypeName",
        OrderBy = "p.ID desc",
        PageIndex = 1,
        PageSize = 10,
        Params = DictBuilder.Assign("age", 23),
        TableName="Person p inner join Type t on p.Typeid=t.id"
    });
```

# 查询2

- 查询数量

```csharp
    long count = db.Count<Person>();
    // select count(0) from Person;
    long count = db.Count<Person>("id>@id", DictBuilder.Assign("id", 1));
    long count = db.Count<Person>("id>@id", new { id=1});
    // select count(0) from person where id=@id; @id=1

    long count = db.CountWhere<Person>("age", 1);
    // select count(0) from Person where age=@age ; @age=1

    long count = db.CountWhere<Person>(DictBuilder.Assign("age", 23).Assign("name", "loogn"));
    long count = db.CountWhere<Person>(new { age=23, name="loogn" });
    // select count(0) from person where age=@age and name=@name; @age=23,@name="loogn";
```

- 查询最大值

```csharp
    long maxid= db.MaxID<long>("person");
    // select max(id) from person 

    string maxid= db.MaxID<string>("person","userid");
    // select max(userid) from person 
```

- 查询首行首列Scalar

```csharp
    int id= db.Scalar<int>("select id from Person where id=23")
    string name= db.Scalar<string>("select name from Person where age>@age",new { age=23})
```

- 查询单列值（主要是返回值）

```csharp
    List<string> nameList = db.Column<string>("select name from person where age>@age", DictBuilder.Assign("age", 23));
    HashSet<string> nameSet = db.ColumnDistinct<string>("select distinct(name) from person");
```

- 查询字典值（主要是返回值）

```csharp
    Dictionary<int,string> typeDict = db.Dictionary<int,string>("select id,name from Type");
    // id 和 name 一一对应的时候可以使用
```

- 查询一对多值（主要是返回值）

```csharp
    Dictionary<int, List<string>> lookUp = db.Lookup<int, string>("select userid,tagName from userTag");
    // 一个userid对应多个tagName
```

# 删除

- 根据id删除

```csharp
    var flag = db.DeleteById<Person>(2);
    // delete from person where id=2;
    var flag = db.DeleteById<Person>("123", "userId");
    // delete from person where userid='123';
    var flag = db.DeleteByIds<Person>(new int[] { 1, 2, 3 });
    // delete from person where id in (1,2,3);
    var flag = db.DeleteByIds<Person>(new string[] { "111", "222", "333" }, "userId");
    //delete from person where userId in ("111", "222", "333")
```

- 根据字段删除

```csharp
    var flag = db.DeleteWhere<Person>("name","loogn");
    // delete from person where name=@name; @name="loogn"

    var flag = db.DeleteWhere<Person>(DictBuilder.Assign("age",23).Assign("name","loogn"));
    var flag = db.DeleteWhere<Person>(new { age=23, name="loogn"});
    // delete from person where age=@age and name=@name; @age=23, @name="loogn";
```

- 根据条件删除

```csharp
    var flag = db.Delete("delete from person where id=@id",DictBuilder.Assign("id",23));
    var flag = db.Delete<Person>();
    // delete from person;
```

# 其他

- 执行存储过程

```csharp
    db.Proc("sp_name", DictBuilder.Assign("p1", 21).Assign("p2", "p2 value"),true); 
    //直接执行

    var cmd = db.Proc("sp_name", DictBuilder.Assign("p1", 21).Assign("p2", "p2 value"));
    //返回cmd，自己处理后续结果，如果有输出参数，可以在这里添加到cmd中
    using (var reader = cmd.ExecuteReader())
    {
        var list = TransformForDataReader.ReaderToObjectList<Person>(reader);
        //TransformForDataReader类中提供了许多从reader读取数据的方法
    }
```

- 事务模板

```csharp
    using (var db = DB.Open())
    {
        db.EnsureOpen();
        var trans = db.BeginTransaction();
        try
        {
            var flag1 = trans.Update(new Person { Id = 1, Name = "loogn2" });
            var flag2 = trans.Insert(new Person { Id = 2, Name = "loogn1" });
            if (flag1 > 0 && flag2 > 0)
            {
                trans.Commit();
            }
            else
            {
                trans.Rollback();
            }
        }
        catch (Exception exp)
        {
            trans.Rollback();
        }
    }
```

> 使用连接对象的BeginTransaction获取事务对象，在事务对象上执行方法即可;
> 事务对象上有和连接对象相同发方法;

- ```csharp
  using (var db = new MySqlConnection("server=.;uid=root;pwd=root;database=test"))
  {
      // do what you like...
  }
  ```
- 全局配置

OrmLite.DefaultKeyName属性配置默认主键名，默认是 "ID";

OrmLite.UpdateIgnoreFields属性全局配置整体修改时忽略的字段名，默认是：["AddTime","AddDate"],可根据项目增减
