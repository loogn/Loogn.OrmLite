using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTesting.Tester
{
    class NativeTester : ITester
    {
        public string Name => "Native";

        public List<TestEntity> GetList(int limit)
        {
            using (var db = DB.CreateConnection())
            {
                var cmd = db.CreateCommand();
                cmd.CommandText = string.Format("select top {0} * from TestEntity", limit);
                cmd.CommandType = System.Data.CommandType.Text;
                db.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    return ReaderToList(reader);
                }
            }
        }

        private List<TestEntity> ReaderToList(SqlDataReader reader)
        {
            List<TestEntity> list = new List<TestEntity>();
            while (reader.Read())
            {
                TestEntity entity = new TestEntity();
                entity.Id = reader.GetInt32(0);
                entity.F_Byte = reader.GetByte(1);
                entity.F_Int16 = reader.GetInt16(2);
                entity.F_Int32 = reader.GetInt32(3);
                entity.F_Int64 = reader.GetInt64(4);
                entity.F_Double = reader.GetDouble(5);
                entity.F_Float = reader.GetFloat(6);
                entity.F_Decimal = reader.GetDecimal(7);
                entity.F_Bool = reader.GetBoolean(8);
                entity.F_DateTime = reader.GetDateTime(9);
                entity.F_Guid = reader.GetGuid(10);
                entity.F_String = reader.GetString(11);
                list.Add(entity);
            }
            return list;
        }

        private SqlConnection context = DB.CreateConnection();

        public List<TestEntity> GetListSingleContent(int limit)
        {
            if (context.State != System.Data.ConnectionState.Open)
            {
                context.Open();
            }

            var cmd = context.CreateCommand();
            cmd.CommandText = string.Format("select top {0} * from TestEntity", limit);
            cmd.CommandType = System.Data.CommandType.Text;
            using (var reader = cmd.ExecuteReader())
            {
                return ReaderToList(reader);
            }
        }
    }
}
