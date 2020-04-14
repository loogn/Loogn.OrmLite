using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Loogn.OrmLite
{
    public abstract class AbstractDao<TEntity>
    {
        protected abstract IDbConnection Open();

        public IDbCommand Proc(string name, object inParams = null, bool execute = false)
        {
            using (var db = Open())
            {
                return db.Proc(name, inParams, execute);
            }
        }

        public long Insert(TEntity m, bool selectIdentity = false)
        {
            using (var db = Open())
            {
                return db.Insert(m, selectIdentity);
            }
        }

        public bool BatchInsert(IEnumerable<TEntity> list)
        {
            using (var db = Open())
            {
                return db.InsertAll(list);
            }
        }


        public int Update(TEntity m)
        {
            using (var db = Open())
            {
                return db.Update(m);
            }
        }

        public int Update(TEntity m, params string[] updateFields)
        {
            using (var db = Open())
            {
                return db.Update(m, updateFields);
            }
        }

        public int Update(IDictionary<string, object> updateFields, string conditions, IDictionary<string, object> parameters)
        {
            using (var db = Open())
            {
                return db.Update<TEntity>(updateFields, conditions, parameters);
            }
        }

        public int BatchUpdate(IEnumerable<TEntity> objs)
        {
            using (var db = Open())
            {
                return db.UpdateAll(objs);
            }
        }

        public int UpdateFieldById(string field, object value, long id)
        {
            using (var db = Open())
            {
                return db.UpdateFieldById<TEntity>(field, value, id);
            }
        }

        public int UpdateFieldById(IDictionary<string, object> fieldValues, long id)
        {
            using (var db = Open())
            {
                return db.UpdateById<TEntity>(fieldValues, id);
            }
        }

        public int UpdateWhere(IDictionary<string, object> fieldValues, IDictionary<string, object> conditions)
        {
            using (var db = Open())
            {
                return db.UpdateWhere<TEntity>(fieldValues, conditions);
            }
        }

        public int DeleteById(long id)
        {
            using (var db = Open())
            {
                return db.DeleteById<TEntity>(id);
            }
        }

        public int DeleteWhere(IDictionary<string, object> conditions)
        {
            using (var db = Open())
            {
                return db.DeleteWhere<TEntity>(conditions);
            }
        }

        public int DeleteWhere(string field, object value)
        {
            using (var db = Open())
            {
                return db.DeleteWhere<TEntity>(field, value);
            }
        }

        public int DeleteByIds(IEnumerable ids)
        {
            using (var db = Open())
            {
                return db.DeleteByIds<TEntity>(ids);
            }
        }

        public TEntity Single(string sql)
        {
            using (var db = Open())
            {
                return db.Single<TEntity>(sql);
            }
        }

        public TEntity Single(string sql, IDictionary<string, object> parameters)
        {
            using (var db = Open())
            {
                return db.Single<TEntity>(sql, parameters);
            }
        }


        public TEntity SingleById(long id)
        {
            using (var db = Open())
            {
                return db.SingleById<TEntity>(id);
            }
        }

        public TEntity SingleWhere(IDictionary<string, object> conditions, string orderBy = "")
        {
            using (var db = Open())
            {
                return db.SingleWhere<TEntity>(conditions, orderBy);
            }
        }
        public TEntity SingleWhere(string name, object value, string orderBy = "")
        {
            using (var db = Open())
            {
                return db.SingleWhere<TEntity>(name, value, orderBy);
            }
        }

        public List<TEntity> SelectByIds(IEnumerable ids, string fields = "*")
        {
            using (var db = Open())
            {
                return db.SelectByIds<TEntity>(ids, "Id", fields);
            }
        }

        public List<TEntity> SelectAll()
        {
            using (var db = Open())
            {
                return db.Select<TEntity>();
            }
        }

        public List<TEntity> Select(string sql)
        {
            using (var db = Open())
            {
                return db.Select<TEntity>(sql);
            }
        }

        public List<TEntity> Select(string sql, object parameters)
        {
            using (var db = Open())
            {
                return db.Select<TEntity>(sql, parameters);
            }
        }

        public List<TEntity> Select(string sql, IDictionary<string, object> parameters)
        {
            using (var db = Open())
            {
                return db.Select<TEntity>(sql, parameters);
            }
        }

        public List<TEntity> SelectWhere(IDictionary<string, object> conditions, string orderBy = "")
        {
            using (var db = Open())
            {
                return db.SelectWhere<TEntity>(conditions, orderBy);
            }
        }

        public List<TEntity> SelectWhere(object conditions, string orderBy = "")
        {
            using (var db = Open())
            {
                return db.SelectWhere<TEntity>(conditions, orderBy);
            }
        }

        public List<TEntity> SelectWhere(string field, object value, string orderBy = "")
        {
            using (var db = Open())
            {
                return db.SelectWhere<TEntity>(field, value, orderBy);
            }
        }


        public OrmLitePageResult<TEntity> SelectPage(OrmLitePageFactor factor)
        {
            using (var db = Open())
            {
                return db.SelectPage<TEntity>(factor);
            }
        }

        public long Count()
        {
            using (var db = Open())
            {
                return db.Count<TEntity>();
            }
        }

        public long Count(string sql)
        {
            using (var db = Open())
            {
                return db.Count<TEntity>(sql);
            }
        }

        public long Count(string sql, IDictionary<string, object> parameters)
        {
            using (var db = Open())
            {
                return db.Count<TEntity>(sql, parameters);
            }
        }

        public long Count(string sql, object parameters)
        {
            using (var db = Open())
            {
                return db.Count<TEntity>(sql, parameters);
            }
        }

        public long CountWhere(string field, object value)
        {
            using (var db = Open())
            {
                return db.CountWhere<TEntity>(field, value);
            }
        }

        public long CountWhere(IDictionary<string, object> conditions)
        {
            using (var db = Open())
            {
                return db.CountWhere<TEntity>(conditions);
            }
        }

        public long CountWhere(object conditions)
        {
            using (var db = Open())
            {
                return db.CountWhere<TEntity>(conditions);
            }
        }
    }
}
