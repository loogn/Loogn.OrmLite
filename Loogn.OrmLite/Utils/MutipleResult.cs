using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Loogn.OrmLite
{
    /// <summary>
    /// 查询多个结果集时，返回此类型的对象，客户端手动Fetch
    /// </summary>
    public class MutipleResult : IDisposable
    {
        #region 实现Dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public void Close()
        {
            Dispose();
        }
        ~MutipleResult()
        {
            Dispose(false);
        }
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                //释放托管资源
            }
            if (_reader != null)
            {
                _reader.Dispose();
            }
            disposed = true;
        }
        #endregion

        DbDataReader _reader;
        bool _firstResult = true;

        public MutipleResult() { }
        public MutipleResult(DbDataReader reader)
        {
            _reader = reader;
        }

        public List<TModel> FetchList<TModel>()
        {
            if (_reader == null) return new List<TModel>();
            if (!_firstResult)
            {
                var hasResult = _reader.NextResult();
                if (!hasResult) return new List<TModel>();
            }
            _firstResult = false;
            return Mapping.ReaderToObjectList<TModel>(_reader);
        }

        public TModel FetchObject<TModel>()
        {
            if (_reader == null) return default(TModel);
            if (!_firstResult)
            {
                var hasResult = _reader.NextResult();
                if (!hasResult) return default(TModel);
            }
            _firstResult = false;
            return Mapping.ReaderToObject<TModel>(_reader);
        }

        public TValue FetchScalar<TValue>()
        {
            if (_reader == null) return default(TValue);
            if (!_firstResult)
            {
                var hasResult = _reader.NextResult();
                if (!hasResult) return default(TValue);
            }
            _firstResult = false;
            return Mapping.ReaderToScalar<TValue>(_reader);
        }

    }


    /// <summary>
    /// 查询多个结果的sql命令信息
    /// </summary>
    public class MutipleCmd
    {
        /// <summary>
        /// sql语句
        /// </summary>
        public string CmdText { get; set; }
        public Dictionary<string, object> Params { get; set; }

        internal Dictionary<string, object> GetUniqueParams(int index)
        {
            if (Params == null || Params.Count == 0) return new Dictionary<string, object>();
            var newParams = new Dictionary<string, object>();
            foreach (var kv in Params)
            {
                newParams.Add(kv.Key + "_" + index, kv.Value);
            }
            return newParams;
        }
        internal string GetMatchedCmdText(int index)
        {
            if (Params == null || Params.Count == 0) return CmdText;
            var newSql = CmdText;
            foreach (var item in Params)
            {
                newSql = newSql.Replace("@" + item.Key, "@" + item.Key + "_" + index);
            }
            return newSql;
        }
    }
}
