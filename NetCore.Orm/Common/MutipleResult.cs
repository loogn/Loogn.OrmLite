using System;
using System.Collections.Generic;
using System.Data;
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

        IDataReader _reader;

        public MutipleResult() { }
        public MutipleResult(IDataReader reader)
        {
            _reader = reader;
        }

        /// <summary>
        /// 从DataReader获取多行数据，填充为TModel集合返回
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public List<TModel> FetchList<TModel>()
        {
            if (_reader == null || _reader.IsClosed) return new List<TModel>();

            var list = TransformForDataReader.ReaderToObjectList<TModel>(_reader);
            if (!_reader.NextResult())
            {
                _reader.Close();
            }
            return list;
        }

        /// <summary>
        /// 从DataReader获取一行数据，填充为TModel返回
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public TModel FetchObject<TModel>()
        {
            if (_reader == null || _reader.IsClosed) return default(TModel);

            var m = TransformForDataReader.ReaderToObject<TModel>(_reader);
            if (!_reader.NextResult())
            {
                _reader.Close();
            }
            return m;
        }

        /// <summary>
        /// 从DataReader获取首行首列的值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        public TValue FetchScalar<TValue>()
        {
            if (_reader == null || _reader.IsClosed) return default(TValue);

            var scalar = TransformForDataReader.ReaderToScalar<TValue>(_reader);
            if (!_reader.NextResult())
            {
                _reader.Close();
            }
            return scalar;
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
