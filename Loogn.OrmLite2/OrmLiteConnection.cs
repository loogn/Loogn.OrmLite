using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite2
{
    class OrmLiteConnection : IDbConnection, ICloneable
    {
        IDbConnection _innerConnection;
        ICommandDialectProvider _provider;
        public OrmLiteConnection(ICommandDialectProvider provider)
        {
            _provider = provider;
            _innerConnection = provider.CreateConnection();
        }

        //IDbConnection CreateConnection
        public string ConnectionString
        {
            get
            {
                return _innerConnection.ConnectionString;

            }

            set
            {
                _innerConnection.ConnectionString = value;
            }
        }

        public int ConnectionTimeout
        {
            get
            {
                return _innerConnection.ConnectionTimeout;
            }
        }

        public string Database
        {
            get
            {
                return _innerConnection.Database;
            }
        }

        public ConnectionState State
        {
            get
            {
                return _innerConnection.State;
            }
        }

        public IDbTransaction BeginTransaction()
        {
            return _innerConnection.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return _innerConnection.BeginTransaction();
        }

        public void ChangeDatabase(string databaseName)
        {
            _innerConnection.ChangeDatabase(databaseName);
        }

        public object Clone()
        {
            return new OrmLiteConnection(_provider);
        }

        public void Close()
        {
            _innerConnection.Close();
        }

        public IDbCommand CreateCommand()
        {
            return _innerConnection.CreateCommand();
        }

        public void Dispose()
        {
            _innerConnection.Dispose();
        }

        public void Open()
        {
            _innerConnection.Open();
        }
    }
}
