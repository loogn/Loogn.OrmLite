using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite2
{
    public interface ICommandDialectProvider
    {
        IDbConnection CreateConnection();
        IDbDataParameter CreateParameter();
        string OpenQuote { get; }
        string CloseQuote { get; }
        string IsNullFunc { get; }
        string GetLastInsertId();
        CommandInfo Select<T>();
        CommandInfo FullSelect<T>(string sqlOrCondition);
        CommandInfo FullCount<T>(string sqlOrCondition);
        CommandInfo SelectWhere<T>(string name, object value);
        CommandInfo SelectWhere<T>(IDictionary<string, object> conditions);
        CommandInfo SelectWhere<T>(object conditions);
        CommandInfo SelectByIds<T>(IEnumerable idValues, string idField, string selectFields = "*");

        CommandInfo FullSingle<T>(string sqlOrCondition);
        CommandInfo SingleById<T>(object idValue, string idField);
        CommandInfo SingleWhere<T>(string name, object value);
        CommandInfo SingleWhere<T>(IDictionary<string, object> conditions);
        CommandInfo SingleWhere<T>(object conditions);
        CommandInfo Count<T>();
        CommandInfo CountWhere<T>(string name, object value);
        CommandInfo CountWhere<T>(IDictionary<string, object> conditions);
        CommandInfo CountWhere<T>(object conditions);

        CommandInfo Update(string tableName, IDictionary<string, object> updateFields, string conditions, IDictionary<string, object> parameters);

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
