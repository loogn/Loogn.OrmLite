using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Loogn.OrmLite
{
    /// <summary>
    /// 基础数据访问操作类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class AbstractDao<TEntity>
    {
        /// <summary>
        /// 打开数据库连接
        /// </summary>
        /// <returns></returns>
        protected abstract IDbConnection Open();

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="name">存储过程名称</param>
        /// <param name="inParams">参数</param>
        /// <param name="execute">是否立即执行</param>
        /// <returns></returns>
        public IDbCommand Proc(string name, object inParams = null, bool execute = false)
        {
            using (var db = Open())
            {
                return db.Proc(name, inParams, execute);
            }
        }

        /// <summary>
        /// 插入数据，可指定是否返回新增项产生的ID
        /// </summary>
        /// <param name="m">实体</param>
        /// <param name="selectIdentity">是否返回新增的ID</param>
        /// <returns></returns>
        public long Insert(TEntity m, bool selectIdentity = false)
        {
            using (var db = Open())
            {
                return db.Insert(m, selectIdentity);
            }
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool BatchInsert(IEnumerable<TEntity> list)
        {
            using (var db = Open())
            {
                return db.InsertAll(list);
            }
        }


        /// <summary>
        /// 使用实体更新数据，可在字段上使用 [OrmLiteField(UpdateIgnore = true)] 忽略指定字段
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public int Update(TEntity m)
        {
            using (var db = Open())
            {
                return db.Update(m);
            }
        }

        /// <summary>
        /// 使用实体更新数据，只更新指定的一个或多个字段
        /// </summary>
        /// <param name="m">实体</param>
        /// <param name="updateFields">要更新的字段</param>
        /// <returns></returns>
        public int Update(TEntity m, params string[] updateFields)
        {
            using (var db = Open())
            {
                return db.Update(m, updateFields);
            }
        }

        /// <summary>
        /// 指定条件更新数据
        /// </summary>
        /// <param name="updateFields"></param>
        /// <param name="conditions"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int Update(IDictionary<string, object> updateFields, string conditions, IDictionary<string, object> parameters)
        {
            using (var db = Open())
            {
                return db.Update<TEntity>(updateFields, conditions, parameters);
            }
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="objs"></param>
        /// <returns></returns>
        public int BatchUpdate(IEnumerable<TEntity> objs)
        {
            using (var db = Open())
            {
                return db.UpdateAll(objs);
            }
        }

        /// <summary>
        /// 使用主键更新指定字段的值
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int UpdateFieldById(string field, object value, long id)
        {
            using (var db = Open())
            {
                return db.UpdateFieldById<TEntity>(field, value, id);
            }
        }

        /// <summary>
        /// 使用主键更新多个字段的值
        /// </summary>
        /// <param name="fieldValues"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int UpdateFieldById(IDictionary<string, object> fieldValues, long id)
        {
            using (var db = Open())
            {
                return db.UpdateById<TEntity>(fieldValues, id);
            }
        }

        /// <summary>
        /// 指定条件批量更新数据
        /// </summary>
        /// <param name="fieldValues">要更新的字段和值</param>
        /// <param name="conditions">条件列表（字段+匹配值）</param>
        /// <returns></returns>
        public int UpdateWhere(IDictionary<string, object> fieldValues, IDictionary<string, object> conditions)
        {
            using (var db = Open())
            {
                return db.UpdateWhere<TEntity>(fieldValues, conditions);
            }
        }

        /// <summary>
        /// 使用主键删除数据
        /// </summary>
        /// <param name="id">数据主键</param>
        /// <returns>影响行数</returns>
        public int DeleteById(long id)
        {
            using (var db = Open())
            {
                return db.DeleteById<TEntity>(id);
            }
        }

        /// <summary>
        /// 指定条件删除数据
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public int DeleteWhere(IDictionary<string, object> conditions)
        {
            using (var db = Open())
            {
                return db.DeleteWhere<TEntity>(conditions);
            }
        }

        /// <summary>
        /// 指定条件删除数据
        /// </summary>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public int DeleteWhere(string field, object value)
        {
            using (var db = Open())
            {
                return db.DeleteWhere<TEntity>(field, value);
            }
        }

        /// <summary>
        /// 根据多个id批量删除多条数据
        /// </summary>
        /// <param name="ids">id集合</param>
        /// <returns></returns>
        public int DeleteByIds(IEnumerable ids)
        {
            using (var db = Open())
            {
                return db.DeleteByIds<TEntity>(ids);
            }
        }

        /// <summary>
        /// 清空表数据
        /// </summary>
        /// <returns></returns>
        public int DeleteAll()
        {
            using (var db = Open())
            {
                return db.Delete<TEntity>();
            }
        }

        /// <summary>
        /// 根据SQL查询并返回第一条数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public TEntity Single(string sql)
        {
            using (var db = Open())
            {
                return db.Single<TEntity>(sql);
            }
        }

        /// <summary>
        /// 根据SQL查询并返回第一条数据（使用占位符和参数）
        /// 例：select ID,Name from Person where age>10 and name=@name
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public TEntity Single(string sql, IDictionary<string, object> parameters)
        {
            using (var db = Open())
            {
                return db.Single<TEntity>(sql, parameters);
            }
        }
        
        /// <summary>
        /// 根据ID查询单条数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity SingleById(long id)
        {
            using (var db = Open())
            {
                return db.SingleById<TEntity>(id);
            }
        }

        /// <summary>
        /// 传入多个自定义条件查询单条数据
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public TEntity SingleWhere(IDictionary<string, object> conditions, string orderBy = "")
        {
            using (var db = Open())
            {
                return db.SingleWhere<TEntity>(conditions, orderBy);
            }
        }
        /// <summary>
        /// 指定一个条件字段查询单条数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public TEntity SingleWhere(string name, object value, string orderBy = "")
        {
            using (var db = Open())
            {
                return db.SingleWhere<TEntity>(name, value, orderBy);
            }
        }

        /// <summary>
        /// 根据多个主键查询多条数据
        /// </summary>
        /// <param name="ids">主键列表</param>
        /// <param name="fields">要查询的字段</param>
        /// <returns></returns>
        public List<TEntity> SelectByIds(IEnumerable ids, string fields = "*")
        {
            using (var db = Open())
            {
                return db.SelectByIds<TEntity>(ids, "Id", fields);
            }
        }

        /// <summary>
        /// 返回指定表所有数据
        /// </summary>
        /// <returns></returns>
        public List<TEntity> SelectAll()
        {
            using (var db = Open())
            {
                return db.Select<TEntity>();
            }
        }

        /// <summary>
        /// 指定SQL查询数据列表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<TEntity> Select(string sql)
        {
            using (var db = Open())
            {
                return db.Select<TEntity>(sql);
            }
        }

        /// <summary>
        /// 参数化查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<TEntity> Select(string sql, object parameters)
        {
            using (var db = Open())
            {
                return db.Select<TEntity>(sql, parameters);
            }
        }

        /// <summary>
        /// 查询，使用字典构建条件语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public List<TEntity> Select(string sql, IDictionary<string, object> parameters)
        {
            using (var db = Open())
            {
                return db.Select<TEntity>(sql, parameters);
            }
        }

        /// <summary>
        /// 指定条件字典和排序规则进行查询
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public List<TEntity> SelectWhere(IDictionary<string, object> conditions, string orderBy = "")
        {
            using (var db = Open())
            {
                return db.SelectWhere<TEntity>(conditions, orderBy);
            }
        }

        /// <summary>
        /// 使用匿名对象和排序语句进行查询
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public List<TEntity> SelectWhere(object conditions, string orderBy = "")
        {
            using (var db = Open())
            {
                return db.SelectWhere<TEntity>(conditions, orderBy);
            }
        }

        /// <summary>
        /// 指定单字段条件和排序语句进行查询
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="orderBy"></param>
        /// <returns></returns>
        public List<TEntity> SelectWhere(string field, object value, string orderBy = "")
        {
            using (var db = Open())
            {
                return db.SelectWhere<TEntity>(field, value, orderBy);
            }
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="factor"></param>
        /// <returns></returns>
        public OrmLitePageResult<TEntity> SelectPage(OrmLitePageFactor factor)
        {
            using (var db = Open())
            {
                return db.SelectPage<TEntity>(factor);
            }
        }

        /// <summary>
        /// 获取整表数据量
        /// </summary>
        /// <returns></returns>
        public long Count()
        {
            using (var db = Open())
            {
                return db.Count<TEntity>();
            }
        }

        /// <summary>
        /// 指定SQL统计数据量
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public long Count(string sql)
        {
            using (var db = Open())
            {
                return db.Count<TEntity>(sql);
            }
        }

        /// <summary>
        /// 使用条件字典统计数据量
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public long Count(string sql, IDictionary<string, object> parameters)
        {
            using (var db = Open())
            {
                return db.Count<TEntity>(sql, parameters);
            }
        }

        /// <summary>
        /// 参数化统计数据量
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public long Count(string sql, object parameters)
        {
            using (var db = Open())
            {
                return db.Count<TEntity>(sql, parameters);
            }
        }

        /// <summary>
        /// 指定一个字段和值统计数据量
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public long CountWhere(string field, object value)
        {
            using (var db = Open())
            {
                return db.CountWhere<TEntity>(field, value);
            }
        }

        /// <summary>
        /// 使用条件字段统计数据量
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public long CountWhere(IDictionary<string, object> conditions)
        {
            using (var db = Open())
            {
                return db.CountWhere<TEntity>(conditions);
            }
        }

        /// <summary>
        /// 使用匿名对象作为条件统计数据量
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public long CountWhere(object conditions)
        {
            using (var db = Open())
            {
                return db.CountWhere<TEntity>(conditions);
            }
        }

        /// <summary>
        /// 根据sql语句查询首行首列
        /// </summary>
        /// <param name="sql"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Scalar<T>(string sql)
        {
            using (var db = Open())
            {
                return db.Scalar<T>(sql);
            }
        }

        /// <summary>
        /// 根据sql语句查询首行首列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Scalar<T>(string sql, IDictionary<string, object> parameters)
        {
            using (var db = Open())
            {

                return db.Scalar<T>(sql, parameters);
            }
        }

        /// <summary>
        /// 根据sql语句查询首行首列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Scalar<T>(string sql, object parameters)
        {
            using (var db = Open())
            {
                return db.Scalar<T>(sql, parameters);
            }
        }

        /// <summary>
        /// 删表（后续添加删库功能，如有需要请自行完善跑路逻辑。）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public int Truncate<T>()
        {
            using (var db = Open())
            {
                return db.Truncate<T>();
            }
        }
    }
}
