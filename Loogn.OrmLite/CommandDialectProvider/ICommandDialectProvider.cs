using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    /// <summary>
    /// 命令方言接口
    /// </summary>
    public interface ICommandDialectProvider
    {
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <returns></returns>
        IDbConnection CreateConnection();
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <returns></returns>
        IDbDataParameter CreateParameter();
        /// <summary>
        /// 左引号
        /// </summary>
        string OpenQuote { get; }
        /// <summary>
        /// 右引号
        /// </summary>
        string CloseQuote { get; }
        /// <summary>
        /// 判断null函数
        /// </summary>
        string IsNullFunc { get; }
        /// <summary>
        /// 获取自增列语句
        /// </summary>
        /// <returns></returns>
        string GetLastInsertId();
        /// <summary>
        /// 查询语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        CommandInfo Select<T>();
        /// <summary>
        /// 查询语句，自动填充Select * from table where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlOrCondition"></param>
        /// <returns></returns>
        CommandInfo FullSelect<T>(string sqlOrCondition);
        /// <summary>
        ///  查询语句，自动填充Select * from table where
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlOrCondition"></param>
        /// <returns></returns>
        CommandInfo FullCount<T>(string sqlOrCondition);

        /// <summary>
        /// 根据单字段查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        CommandInfo SelectWhere<T>(string name, object value, string orderBy);
        /// <summary>
        /// 根据字典条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditions"></param>
        /// <returns></returns>
        CommandInfo SelectWhere<T>(IDictionary<string, object> conditions, string orderBy);
        /// <summary>
        ///根据对象条件查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditions"></param>
        /// <returns></returns>
        CommandInfo SelectWhere<T>(object conditions, string orderBy);
        /// <summary>
        /// 根据编号查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idValues"></param>
        /// <param name="idField"></param>
        /// <param name="selectFields"></param>
        /// <returns></returns>
        CommandInfo SelectByIds<T>(IEnumerable idValues, string idField, string selectFields = "*");
        /// <summary>
        /// 根据语句查询，必要时填充
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlOrCondition"></param>
        /// <returns></returns>
        CommandInfo FullSingle<T>(string sqlOrCondition);
        /// <summary>
        /// 根据编号查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="idValue"></param>
        /// <param name="idField"></param>
        /// <returns></returns>
        CommandInfo SingleById<T>(object idValue, string idField);
        /// <summary>
        /// 根据字段查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        CommandInfo SingleWhere<T>(string name, object value,string orderBy);
        /// <summary>
        /// 根据字典条件查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditions"></param>
        /// <returns></returns>
        CommandInfo SingleWhere<T>(IDictionary<string, object> conditions,string orderBy);
        /// <summary>
        /// 根据对象条件查询单条数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditions"></param>
        /// <returns></returns>
        CommandInfo SingleWhere<T>(object conditions, string orderBy);
        /// <summary>
        /// 查询数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        CommandInfo Count<T>();
        /// <summary>
        /// 根据字段查询数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        CommandInfo CountWhere<T>(string name, object value);
        /// <summary>
        /// 根据字典条件查询数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditions"></param>
        /// <returns></returns>
        CommandInfo CountWhere<T>(IDictionary<string, object> conditions);
        /// <summary>
        /// 根据对象条件查询数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="conditions"></param>
        /// <returns></returns>
        CommandInfo CountWhere<T>(object conditions);

        CommandInfo Update(string tableName, IDictionary<string, object> updateFields, string conditions, IDictionary<string, object> parameters);

        CommandInfo UpdateWhere(string tableName, IDictionary<string, object> updateFields,  IDictionary<string, object> conditions);

        
        CommandInfo Insert<T>(T obj, bool selectIdentity = false);
        CommandInfo Insert(string table, IDictionary<string, object> fields, bool selectIdentity = false);
        CommandInfo Insert(string table, object obj, bool selectIdentity = false);
        CommandInfo Update<T>(T obj, params string[] updateFields);

        CommandInfo Update(string tableName, object obj);

        CommandInfo Delete<T>();
        CommandInfo DeleteById<T>(object id, string idField = "Id");
        CommandInfo DeleteWhere<T>(IDictionary<string, object> conditions);
        CommandInfo DeleteWhere<T>(string name, object value);
        CommandInfo DeleteWhere<T>(object obj);
        CommandInfo DeleteByIds<T>(IEnumerable idValues, string idField);
        CommandInfo Paged(OrmLitePageFactor factor);

        CommandInfo TableMetaData(string dbName);
        CommandInfo ColumnMetaData(string dbName, string tableName);
        
    }
}
