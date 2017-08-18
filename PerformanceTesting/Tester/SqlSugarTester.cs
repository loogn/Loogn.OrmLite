﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTesting.Tester
{
    public class SqlSugarTester : ITester
    {
        public string Name => "SqlSugar";
        public List<TestEntity> GetList(int limit)
        {
            using (var db = new SqlSugarClient(DB.ConnStr))
            {
                var list = db.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity", limit));
                return list;
            }
        }

        SqlSugarClient _client;
        public SqlSugarTester()
        {
            _client = new SqlSugarClient(DB.ConnStr);
        }

        public List<TestEntity> GetListSingleContent(int limit)
        {
            var list = _client.SqlQuery<TestEntity>(string.Format("select top {0} * from TestEntity", limit));
            return list;
        }
    }
}
