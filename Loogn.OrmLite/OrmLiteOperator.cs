using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace Loogn.OrmLite
{

    [Obsolete("便捷操作类，虽然可以用，但是不建议用")]
    /// <summary>
    /// 便捷操作类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OrmLiteOperator<T> where T : new()
    {
        Func<DbConnection> openDb;
        /// <summary>
        /// 实例化操作类
        /// </summary>
        /// <param name="connFunc">获取连接对象的委托</param>
        public OrmLiteOperator(Func<DbConnection> connFunc)
        {
            openDb = connFunc;
        }

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="name">存储过程名字</param>
        /// <param name="inParams">输入参数</param>
        /// <param name="execute">是否立即执行</param>
        /// <returns></returns>
        public DbCommand Proc(string name, object inParams = null, bool execute = false)
        {
            using (var db = openDb())
            {
                return db.Proc(name, inParams, execute);
            }
        }

        /// <summary>
        /// 执行sql语句，返回影响行数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="ps">参数字典</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, IDictionary<string, object> ps = null)
        {
            using (var db = openDb())
            {
                return db.ExecuteNonQuery(System.Data.CommandType.Text, sql, BaseCmd.GetCmd(db.GetProviderType()).DictionaryToParams(ps));
            }
        }

        #region insert

        /// <summary>
        /// 插入实体，返回影响行数或自增列
        /// </summary>
        /// <param name="m">实体类</param>
        /// <param name="selectIdentity">是否返回自增列</param>
        /// <returns></returns>
        public long Insert(T m, bool selectIdentity = false)
        {
            using (var db = openDb())
            {
                var newid = db.Insert(m, selectIdentity);
                return newid;
            }
        }

        /// <summary>
        /// 插入数据，返回影响行数或自增列
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="fields">字段字典</param>
        /// <param name="selectIdentity">是否返回自增列</param>
        /// <returns></returns>
        public long Insert(string table, IDictionary<string, object> fields, bool selectIdentity = false)
        {
            using (var db = openDb())
            {
                return db.Insert(table, fields, selectIdentity);
            }
        }

        /// <summary>
        /// 插入数据，返回影响行数或自增列
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="anonType">拥有表字段的匿名对象</param>
        /// <param name="selectIdentity">是否返回自增列</param>
        /// <returns></returns>
        public long Insert(string table, object anonType, bool selectIdentity = false)
        {
            using (var db = openDb())
            {
                if (anonType is IDictionary<string, object>)
                {
                    return db.Insert(table, (IDictionary<string, object>)anonType, selectIdentity);
                }
                else
                {
                    return db.Insert(table, anonType, selectIdentity);
                }
            }
        }

        /// <summary>
        /// 批量插入数据，事务成功返回true，否则返回false
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="objs">对象数组</param>
        /// <returns></returns>
        public bool Insert(string table, params object[] objs)
        {
            using (var db = openDb())
            {
                return db.Insert(table, objs);
            }
        }

        /// <summary>
        /// 批量插入数据，事务成功返回true，否则返回false
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="objs">对象集合</param>
        /// <returns></returns>
        public bool InsertAll(string table, IEnumerable objs)
        {
            using (var db = openDb())
            {
                return db.InsertAll(table, objs);
            }
        }

        /// <summary>
        /// 批量插入实体，事务成功返回true，否则返回false
        /// </summary>
        /// <param name="objs">对象数组</param>
        /// <returns></returns>
        public bool Insert(params T[] objs)
        {
            using (var db = openDb())
            {
                return db.Insert(objs);
            }
        }


        /// <summary>
        /// 批量插入实体，事务成功返回true，否则返回false
        /// </summary>
        /// <param name="objs">对象集合</param>
        /// <returns></returns>
        public bool InsertAll(IEnumerable<T> objs)
        {
            using (var db = openDb())
            {
                return db.InsertAll(objs);
            }
        }
        #endregion

        #region update
        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="obj">实体对象</param>
        /// <param name="updateFields">要修改的字段</param>
        /// <returns></returns>
        public int Update(T obj, params string[] updateFields)
        {
            using (var db = openDb())
            {
                return db.Update(obj, updateFields);
            }
        }

        /// <summary>
        /// 修改匿名对象
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="anonymous">匿名对象</param>
        /// <returns></returns>
        public int UpdateAnonymous(string tableName, object anonymous)
        {
            using (var db = openDb())
            {
                return db.UpdateAnonymous(tableName, anonymous);
            }
        }

        /// <summary>
        /// T类型的表名修改匿名对象
        /// </summary>
        /// <param name="anonymous">匿名对象</param>
        /// <returns></returns>
        public int UpdateAnonymous(object anonymous)
        {
            using (var db = openDb())
            {
                return db.UpdateAnonymous<T>(anonymous);
            }
        }

        /// <summary>
        /// 批量修改实体
        /// </summary>
        /// <param name="objs">实体数组</param>
        /// <returns></returns>
        public int Update(params T[] objs)
        {
            return UpdateAll(objs);
        }

        /// <summary>
        /// 批量修改实体
        /// </summary>
        /// <param name="objs">实体集合</param>
        /// <returns></returns>
        public int UpdateAll(IEnumerable<T> objs)
        {
            using (var db = openDb())
            {
                return db.UpdateAll(objs);
            }
        }

        /// <summary>
        /// 修改字段
        /// </summary>
        /// <param name="updateFields">修改字段的列表，$开头的key，会用value的原值修改，比如实现count=count+1</param>
        /// <param name="conditions">条件语句</param>
        /// <param name="parameters">条件语句里的参数字典</param>
        /// <returns></returns>
        public int Update(IDictionary<string, object> updateFields, string conditions, IDictionary<string, object> parameters)
        {
            using (var db = openDb())
            {
                return db.Update<T>(updateFields, conditions, parameters);
            }
        }

        /// <summary>
        /// 根据某个字段修改其他字段
        /// </summary>
        /// <param name="updateFields">被修改字段的列表，$开头的key，会用value的原值修改，比如实现count=count+1</param>
        /// <param name="id">条件字段的值</param>
        /// <param name="idname">条件字段的名称，默认是ID</param>
        /// <returns></returns>
        public int UpdateById(IDictionary<string, object> updateFields, object id, string idname = OrmLite.KeyName)
        {
            using (var db = openDb())
            {
                return db.UpdateById<T>(updateFields, id, idname);
            }
        }

        /// <summary>
        /// 根据某个字段修改其他单一字段
        /// </summary>
        /// <param name="fieldName">被修改字段的名称</param>
        /// <param name="fieldValue">被修改字段的值</param>
        /// <param name="id">条件字段的值</param>
        /// <param name="idname">条件字段的名称，默认是ID</param>
        /// <returns></returns>
        public int UpdateFieldById(string fieldName, object fieldValue, object id, string idname = OrmLite.KeyName)
        {
            using (var db = openDb())
            {
                return db.UpdateFieldById<T>(fieldName, fieldValue, id, idname);
            }
        }
        #endregion

        #region delete
        /// <summary>
        /// 根据sql语句删除数据
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public int Delete(string sql, IDictionary<string, object> parameters = null)
        {
            using (var db = openDb())
            {
                return db.Delete(sql, parameters);
            }
        }

        /// <summary>
        /// 根据条件字典删除数据
        /// </summary>
        /// <param name="conditions">条件字典</param>
        /// <returns></returns>
        public int Delete(IDictionary<string, object> conditions)
        {
            using (var db = openDb())
            {
                return db.Delete<T>(conditions);
            }
        }

        /// <summary>
        /// 根据单个字段删除数据
        /// </summary>
        /// <param name="id">条件字段的值</param>
        /// <param name="idField">条件字段的名称，默认是ID</param>
        /// <returns></returns>
        public int DeleteById(object id, string idField = OrmLite.KeyName)
        {
            using (var db = openDb())
            {
                return db.DeleteById<T>(id, idField);
            }
        }

        /// <summary>
        /// 根据字段集合删除数据
        /// </summary>
        /// <param name="idValues">条件字段集合</param>
        /// <param name="idField">条件字段的名称，默认是ID</param>
        /// <returns></returns>
        public int DeleteByIds(IEnumerable idValues, string idField = OrmLite.KeyName)
        {
            using (var db = openDb())
            {
                return db.DeleteByIds<T>(idValues, idField);
            }
        }
        /// <summary>
        /// 删除所有数据
        /// </summary>
        /// <returns></returns>
        public int Delete()
        {
            using (var db = openDb())
            {
                return db.Delete<T>();
            }
        }
        #endregion

        #region select

        /// <summary>
        /// 查询所有数据
        /// </summary>
        /// <returns></returns>
        public List<T> Select()
        {
            using (var db = openDb())
            {
                return db.Select<T>();
            }
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public List<T> Select(string sql)
        {
            using (var db = openDb())
            {
                return db.Select<T>(sql);
            }
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public List<T> Select(string sql, IDictionary<string, object> parameters)
        {
            using (var db = openDb())
            {
                return db.Select<T>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据sql语句查询数据
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数匿名对象</param>
        /// <returns></returns>
        public List<T> Select(string sql, object parameters)
        {
            using (var db = openDb())
            {
                return db.Select<T>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据单个字段查询数据
        /// </summary>
        /// <param name="name">条件字段名</param>
        /// <param name="value">条件字段值</param>
        /// <returns></returns>
        public List<T> SelectWhere(string name, object value)
        {
            using (var db = openDb())
            {
                return db.SelectWhere<T>(name, value);
            }
        }

        /// <summary>
        /// 根据条件字典查询数据
        /// </summary>
        /// <param name="conditions">条件字典</param>
        /// <returns></returns>
        public List<T> SelectWhere(IDictionary<string, object> conditions)
        {
            using (var db = openDb())
            {
                return db.SelectWhere<T>(conditions);
            }
        }

        /// <summary>
        /// 根据条件匿名对象查询数据
        /// </summary>
        /// <param name="conditions">条件匿名对象</param>
        /// <returns></returns>
        public List<T> SelectWhere(object conditions)
        {
            using (var db = openDb())
            {
                return db.SelectWhere<T>(conditions);
            }
        }

        /// <summary>
        /// 根据有占位符的sql语句查询数据，类似string.Format
        /// </summary>
        /// <param name="sqlFormat">有占位符的sql语句</param>
        /// <param name="parameters">填充数组</param>
        /// <returns></returns>
        public List<T> SelectFmt(string sqlFormat, params object[] parameters)
        {
            using (var db = openDb())
            {
                return db.SelectFmt<T>(sqlFormat, parameters);
            }
        }

        /// <summary>
        /// 根据字段集合查询数据
        /// </summary>
        /// <param name="idValues">字段值集合</param>
        /// <param name="idField">字段名称</param>
        /// <param name="selectFields">查询的字段，默认是*</param>
        /// <returns></returns>
        public List<T> SelectByIds(IEnumerable idValues, string idField = OrmLite.KeyName, string selectFields = "*")
        {
            using (var db = openDb())
            {
                return db.SelectByIds<T>(idValues, idField, selectFields);
            }
        }

        /// <summary>
        /// 得到分页数据，直接返回数据数据集合
        /// </summary>
        /// <param name="factor">分页查询信息</param>
        /// <param name="totalCount">输出参数，总数</param>
        /// <returns></returns>
        public List<T> SelectPage(OrmLitePageFactor factor, out int totalCount)
        {
            using (var db = openDb())
            {
                return db.SelectPage<T>(factor, out totalCount);
            }
        }

        /// <summary>
        /// 得到分页数据，返回分页结果信息 OrmLitePageResult
        /// </summary>
        /// <param name="factor">分页查询信息</param>
        /// <returns></returns>
        public OrmLitePageResult<T> SelectPage(OrmLitePageFactor factor)
        {
            using (var db = openDb())
            {
                return db.SelectPage<T>(factor);
            }
        }

        #endregion

        #region single

        /// <summary>
        /// 根据条件字典查询单个实体
        /// </summary>
        /// <param name="conditions">条件字典</param>
        /// <returns></returns>
        public T Single(IDictionary<string, object> conditions)
        {
            using (var db = openDb())
            {
                return db.Single<T>(conditions);
            }
        }

        /// <summary>
        /// 根据条件对象查询单个实体
        /// </summary>
        /// <param name="conditions">条件对象</param>
        /// <returns></returns>
        public T Single(object conditions)
        {
            using (var db = openDb())
            {
                return db.Single<T>(conditions);
            }
        }
        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public T Single(string sql)
        {
            using (var db = openDb())
            {
                return db.Single<T>(sql);
            }
        }

        /// <summary>
        /// 根据sql语句查询单个实体
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public T Single(string sql, IDictionary<string, object> parameters)
        {
            using (var db = openDb())
            {
                return db.Single<T>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据带占位符的sql语句查询单个实体，类似string.Format
        /// </summary>
        /// <param name="sqlFormat">带占位符的sql语句</param>
        /// <param name="parameters">填充数据</param>
        /// <returns></returns>
        public T SingleFmt(string sqlFormat, params object[] parameters)
        {
            using (var db = openDb())
            {
                return db.SingleFmt<T>(sqlFormat, parameters);
            }
        }

        /// <summary>
        /// 根据单个字段查询单个实体
        /// </summary>
        /// <param name="idValue">条件字段值</param>
        /// <param name="idField">条件字段名称，默认是ID</param>
        /// <returns></returns>
        public T SingleById(object idValue, string idField = OrmLite.KeyName)
        {
            using (var db = openDb())
            {
                return db.SingleById<T>(idValue, idField);
            }
        }

        /// <summary>
        /// 根据单个字段查询单个实体
        /// </summary>
        /// <param name="name">条件字段名称</param>
        /// <param name="value">条件字段值</param>
        /// <returns></returns>
        public T SingleWhere(string name, object value)
        {
            using (var db = openDb())
            {
                return db.SingleWhere<T>(name, value);
            }
        }

        /// <summary>
        /// 根据条件字典查询单个实体
        /// </summary>
        /// <param name="conditions">条件字典</param>
        /// <returns></returns>
        public T SingleWhere(IDictionary<string, object> conditions)
        {
            using (var db = openDb())
            {
                return db.SingleWhere<T>(conditions);
            }
        }

        /// <summary>
        /// 根据条件匿名对象查询单个实体
        /// </summary>
        /// <param name="conditions">条件匿名对象</param>
        /// <returns></returns>
        public T SingleWhere(object conditions)
        {
            using (var db = openDb())
            {
                return db.SingleWhere<T>(conditions);
            }
        }
        #endregion

        #region Scalar

        /// <summary>
        /// 根据sql语句查询首行首列数据
        /// </summary>
        /// <typeparam name="RetType">查询数据的类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public RetType Scalar<RetType>(string sql)
        {
            using (var db = openDb())
            {
                return db.Scalar<RetType>(sql);
            }
        }

        /// <summary>
        /// 根据sql语句查询首行首列数据
        /// </summary>
        /// <typeparam name="RetType">查询数据的类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public RetType Scalar<RetType>(string sql, IDictionary<string, object> parameters)
        {
            using (var db = openDb())
            {
                return db.Scalar<RetType>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据sql语句查询首行首列数据
        /// </summary>
        /// <typeparam name="RetType">查询数据的类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数匿名对象</param>
        /// <returns></returns>
        public RetType Scalar<RetType>(string sql, object parameters)
        {
            using (var db = openDb())
            {
                return db.Scalar<RetType>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据格式化sql语句查询首行首列数据，类似string.Format
        /// </summary>
        /// <typeparam name="RetType">查询数据的类型</typeparam>
        /// <param name="sqlFormat">sql语句</param>
        /// <param name="parameters">参数匿名对象</param>
        /// <returns></returns>
        public RetType ScalarFmt<RetType>(string sqlFormat, params object[] parameters)
        {
            using (var db = openDb())
            {
                return db.ScalarFmt<RetType>(sqlFormat, parameters);
            }
        }

        #endregion

        #region Column

        /// <summary>
        /// 根据sql语句查询单个列
        /// </summary>
        /// <typeparam name="TField">查询列的类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public List<TField> Column<TField>(string sql)
        {
            using (var db = openDb())
            {
                return db.Column<TField>(sql);
            }
        }

        /// <summary>
        /// 根据sql语句查询单个列
        /// </summary>
        /// <typeparam name="TField">查询列的类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public List<TField> Column<TField>(string sql, IDictionary<string, object> parameters)
        {
            using (var db = openDb())
            {
                return db.Column<TField>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据sql语句查询单个列
        /// </summary>
        /// <typeparam name="TField">查询列的类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数匿名对象</param>
        /// <returns></returns>
        public List<TField> Column<TField>(string sql, object parameters)
        {
            using (var db = openDb())
            {
                return db.Column<TField>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据格式化sql语句查询单个列
        /// </summary>
        /// <typeparam name="TField">查询列的类型</typeparam>
        /// <param name="sqlFormat">sql语句</param>
        /// <param name="parameters">格式化参数</param>
        /// <returns></returns>
        public List<TField> ColumnFmt<TField>(string sqlFormat, params object[] parameters)
        {
            using (var db = openDb())
            {
                return db.ColumnFmt<TField>(sqlFormat, parameters);
            }
        }

        /// <summary>
        /// 根据sql语句查询去重的单个列
        /// </summary>
        /// <typeparam name="TField">查询列的类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public HashSet<TField> ColumnDistinct<TField>(string sql)
        {
            using (var db = openDb())
            {
                return db.ColumnDistinct<TField>(sql);
            }
        }

        /// <summary>
        /// 根据sql语句查询去重的单个列
        /// </summary>
        /// <typeparam name="TField">查询列的类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public HashSet<TField> ColumnDistinct<TField>(string sql, IDictionary<string, object> parameters)
        {
            using (var db = openDb())
            {
                return db.ColumnDistinct<TField>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据sql语句查询去重的单个列
        /// </summary>
        /// <typeparam name="TField">查询列的类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数匿名对象</param>
        /// <returns></returns>
        public HashSet<TField> ColumnDistinct<TField>(string sql, object parameters)
        {
            using (var db = openDb())
            {
                return db.ColumnDistinct<TField>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据格式化sql语句查询去重的单个列
        /// </summary>
        /// <typeparam name="TField">查询列的类型</typeparam>
        /// <param name="sqlFormat">格式化sql语句</param>
        /// <param name="parameters">格式化参数</param>
        /// <returns></returns>
        public HashSet<TField> ColumnDistinctFmt<TField>(string sqlFormat, params object[] parameters)
        {
            using (var db = openDb())
            {
                return db.ColumnDistinctFmt<TField>(sqlFormat, parameters);
            }
        }

        #endregion

        #region Lookup Dictionary

        /// <summary>
        /// 根据sql语句查询字典集合,比如:select age,name from user(用age聚合出name)
        /// </summary>
        /// <typeparam name="K">聚合列的类型</typeparam>
        /// <typeparam name="V">被聚合列的类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public Dictionary<K, List<V>> Lookup<K, V>(string sql)
        {
            using (var db = openDb())
            {
                return db.Lookup<K, V>(sql);
            }
        }

        /// <summary>
        /// 根据sql语句查询字典集合,比如:select age,name from user(用age聚合出name)
        /// </summary>
        /// <typeparam name="K">聚合列的类型</typeparam>
        /// <typeparam name="V">被聚合列的类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public Dictionary<K, List<V>> Lookup<K, V>(string sql, IDictionary<string, object> parameters)
        {
            using (var db = openDb())
            {
                return db.Lookup<K, V>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据sql语句查询字典集合,比如:select age,name from user(用age聚合出name)
        /// </summary>
        /// <typeparam name="K">聚合列的类型</typeparam>
        /// <typeparam name="V">被聚合列的类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数匿名对象</param>
        /// <returns></returns>
        public Dictionary<K, List<V>> Lookup<K, V>(string sql, object parameters)
        {
            using (var db = openDb())
            {
                return db.Lookup<K, V>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据sql语句查询字典集合,比如:select age,name from user(用age聚合出name)
        /// </summary>
        /// <typeparam name="K">聚合列的类型</typeparam>
        /// <typeparam name="V">被聚合列的类型</typeparam>
        /// <param name="sqlFormat">格式化sql语句</param>
        /// <param name="parameters">格式化参数</param>
        /// <returns></returns>
        public Dictionary<K, List<V>> LookupFmt<K, V>(string sqlFormat, params object[] parameters)
        {
            using (var db = openDb())
            {
                return db.LookupFmt<K, V>(sqlFormat, parameters);
            }
        }

        /// <summary>
        /// 根据sql语句查询字典集合,比如:select id,name from user(查询id对应的name列)
        /// </summary>
        /// <typeparam name="K">第一列类型</typeparam>
        /// <typeparam name="V">第二列类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public Dictionary<K, V> Dictionary<K, V>(string sql)
        {
            using (var db = openDb())
            {
                return db.Dictionary<K, V>(sql);
            }
        }

        /// <summary>
        /// 根据sql语句查询字典集合,比如:select id,name from user(查询id对应的name列)
        /// </summary>
        /// <typeparam name="K">第一列类型</typeparam>
        /// <typeparam name="V">第二列类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public Dictionary<K, V> Dictionary<K, V>(string sql, IDictionary<string, object> parameters)
        {
            using (var db = openDb())
            {
                return db.Dictionary<K, V>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据sql语句查询字典集合,比如:select id,name from user(查询id对应的name列)
        /// </summary>
        /// <typeparam name="K">第一列类型</typeparam>
        /// <typeparam name="V">第二列类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数匿名对象</param>
        /// <returns></returns>
        public Dictionary<K, V> Dictionary<K, V>(string sql, object parameters)
        {
            using (var db = openDb())
            {
                return db.Dictionary<K, V>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据sql语句查询字典集合,比如:select id,name from user(查询id对应的name列)
        /// </summary>
        /// <typeparam name="K">第一列类型</typeparam>
        /// <typeparam name="V">第二列类型</typeparam>
        /// <param name="sqlFormat">格式化sql语句</param>
        /// <param name="parameters">格式化参数</param>
        /// <returns></returns>
        public Dictionary<K, V> DictionaryFmt<K, V>(string sqlFormat, params object[] parameters)
        {
            using (var db = openDb())
            {
                return db.DictionaryFmt<K, V>(sqlFormat, parameters);
            }
        }
        #endregion

        #region Count
        /// <summary>
        /// 查询总条数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            using (var db = openDb())
            {
                return db.Count<T>();
            }
        }

        /// <summary>
        /// 根据sql语句查询条数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public int Count(string sql)
        {
            using (var db = openDb())
            {
                return db.Count<T>(sql);
            }
        }

        /// <summary>
        /// 根据sql语句查询条数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数字典</param>
        /// <returns></returns>
        public int Count(string sql, IDictionary<string, object> parameters)
        {
            using (var db = openDb())
            {
                return db.Count<T>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据sql语句查询条数
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="parameters">参数匿名对象</param>
        /// <returns></returns>
        public int Count(string sql, object parameters)
        {
            using (var db = openDb())
            {
                return db.Count<T>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据单个字段查询条数
        /// </summary>
        /// <param name="name">条件字段名</param>
        /// <param name="value">条件字段值</param>
        /// <returns></returns>
        public int CountWhere(string name, object value)
        {
            using (var db = openDb())
            {
                return db.CountWhere<T>(name, value);
            }
        }

        /// <summary>
        /// 根据条件字段字典查询条数
        /// </summary>
        /// <param name="conditions">条件字段字典</param>
        /// <returns></returns>
        public int CountWhere(IDictionary<string, object> conditions)
        {
            using (var db = openDb())
            {
                return db.CountWhere<T>(conditions);
            }
        }

        /// <summary>
        /// 根据条件匿名对象查询条数
        /// </summary>
        /// <param name="conditions">条件匿名对象</param>
        /// <returns></returns>
        public int CountWhere(object conditions)
        {
            using (var db = openDb())
            {
                return db.CountWhere<T>(conditions);
            }
        }

        /// <summary>
        /// 根据格式化sql语句查询条数
        /// </summary>
        /// <param name="sqlFormat">格式化sql语句</param>
        /// <param name="parameters">格式化参数列表</param>
        /// <returns></returns>
        public int CountFmt(string sqlFormat, params object[] parameters)
        {
            using (var db = openDb())
            {
                return db.CountFmt(sqlFormat, parameters);
            }
        }

        /// <summary>
        /// 查询指定列最大值
        /// </summary>
        /// <typeparam name="TField">查询列类型</typeparam>
        /// <param name="field">查询列名称，默认为ID</param>
        /// <returns></returns>
        public TField MaxId<TField>(string field = OrmLite.KeyName)
        {
            using (var db = openDb())
            {
                var tableName = ReflectionHelper.GetInfo<T>().TableName;
                return db.MaxID<TField>(tableName, field);
            }
        }
        #endregion

    }
}
