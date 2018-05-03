# 简要
   ***Loogn.OrmLite是一个超简单、超高效、超灵活的基于.net standard 2.0的数据访问组件！***   

# 特点
1. 支持sqlserver、mysql、sqlite3数据库；
2. 通过扩展方法扩展 _IDbConnection_ 和 _IDbTransaction_ ；
3. 支持数据库事务和批量插入
4. 支持 _dynamic_ 类型的模型（不定义模型也可使用此ORM）
5. 超高的效率，超小的体积，比Dapper快，比Dapper小，Loogn.OrmLite.dll只有 **85kb** 


# Getting Started

一、 引入Loogn.OrmLite
```
    > Install-Package Loogn.OrmLite
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
    }
```
使用起来就更简单了：
```csharp
    using (var db = DB.Open())
    {
        // do something with db
    }
```

## [完整文档](http://www.loogn.net/orm/ "ormlite") 

