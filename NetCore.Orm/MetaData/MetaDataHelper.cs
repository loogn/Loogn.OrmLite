using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loogn.OrmLite.MetaData
{
    public static class MetaDataHelper
    {
        static Dictionary<string, string> m_dataTypeDict;
        static MetaDataHelper()
        {
            m_dataTypeDict = new Dictionary<string, string>();
            m_dataTypeDict.Add("nvarchar", "string");
            m_dataTypeDict.Add("varchar", "string");
            m_dataTypeDict.Add("char", "string");
            m_dataTypeDict.Add("nchar", "string");
            m_dataTypeDict.Add("text", "string");
            m_dataTypeDict.Add("ntext", "string");
            m_dataTypeDict.Add("longtext", "string");
            m_dataTypeDict.Add("xml", "string");

            m_dataTypeDict.Add("bit", "bool");
            m_dataTypeDict.Add("tinyint", "byte");
            m_dataTypeDict.Add("smallint", "short");
            m_dataTypeDict.Add("bigint", "long");
            m_dataTypeDict.Add("real", "float");
            m_dataTypeDict.Add("float", "double");
            m_dataTypeDict.Add("money", "decimal");
            m_dataTypeDict.Add("decimal", "decimal");
            m_dataTypeDict.Add("date", "DateTime");
            m_dataTypeDict.Add("datetime", "DateTime");
            m_dataTypeDict.Add("datetime2", "DateTime");
            m_dataTypeDict.Add("image", "byte[]");
            m_dataTypeDict.Add("binary", "byte[]");
            m_dataTypeDict.Add("uniqueidentifier", "Guid");

        }

        /// <summary>
        /// 添加数据库类型到csharp类型的映射
        /// </summary>
        /// <param name="sqlDataType"></param>
        /// <param name="dataType"></param>
        public static void AppendDataTypeMap(string sqlDataType, string dataType)
        {
            m_dataTypeDict[sqlDataType] = dataType;
        }


        internal static string SqlDataType2DataType(string sqlDataType)
        {
            string dataType;
            if (m_dataTypeDict.TryGetValue(sqlDataType.ToLowerInvariant(), out dataType))
            {
                return dataType;
            }
            return sqlDataType;
        }
    }
}
