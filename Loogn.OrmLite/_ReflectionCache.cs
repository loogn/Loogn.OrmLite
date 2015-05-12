using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    static class ReflectionCache
    {
        static Dictionary<Type, string> TableNamesDict = new Dictionary<Type, string>();
        static Dictionary<Type, PropertyInfo[]> PropertysDict = new Dictionary<Type, PropertyInfo[]>();
        static Dictionary<MemberInfo, object[]> CustomAttributesDict = new Dictionary<MemberInfo, object[]>();
        public static string GetCachedTableName(this Type type)
        {
            string tableName;
            if (TableNamesDict.TryGetValue(type, out tableName))
            {
                return tableName;
            }
            if (type.Name.EndsWith("Info", StringComparison.OrdinalIgnoreCase))
            {
                tableName = type.Name.Substring(0, type.Name.Length - 4);
            }
            else if (type.Name.EndsWith("Model", StringComparison.OrdinalIgnoreCase))
            {
                tableName = type.Name.Substring(0, type.Name.Length - 5);
            }
            else
            {
                var tableAttr = type.GetCustomAttributes(typeof(OrmLiteTableAttribute), true).FirstOrDefault() as OrmLiteTableAttribute;
                if (tableAttr != null && tableAttr.Name != null && tableAttr.Name.Length > 0)
                {
                    tableName = tableAttr.Name;
                }
                else
                {
                    tableName = type.Name;
                }
            }
            TableNamesDict[type] = tableName;
            return tableName;

        }

        public static PropertyInfo[] GetCachedProperties(this Type type)
        {
            PropertyInfo[] value;
            if (PropertysDict.TryGetValue(type, out value))
            {
                return value;
            }
            value = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropertysDict[type] = value;
            return value;
        }

        public static object[] GetCachedCustomAttributes(this MemberInfo member, Type attributeType)
        {
            object[] value;
            if (CustomAttributesDict.TryGetValue(member, out value))
            {
                return value;
            }
            value = member.GetCustomAttributes(attributeType, false);
            CustomAttributesDict[member] = value;
            return value;
        }
    }
}
