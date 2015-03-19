using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    internal static class ReflectionCache
    {
        static Dictionary<string, object> Dict = new Dictionary<string, object>();

        public static object[] GetCustomAttributes_Cache(this Type type, Type attributeType, bool inherit)
        {
            return GetCache<object[]>(type.FullName + "_attrs_" + attributeType.FullName, () =>
            {
                return type.GetCustomAttributes(attributeType, inherit);
            });
        }

        public static object[] GetCustomAttributes_Cache(this Type type, bool inherit)
        {
            return GetCache(type.FullName + "_attrs", () =>
            {
                return type.GetCustomAttributes(inherit);
            });
        }

        public static string GetCachedTableName(this Type type)
        {
            return GetCache(type.FullName + "_tn", () =>
            {
                var table = "";
                if (type.Name.EndsWith("Info"))
                {
                    table = type.Name.Substring(0, type.Name.Length - 4);
                }
                else
                {
                    var tableAttr = (OrmLiteTableAttribute)type.GetCustomAttributes(typeof(OrmLiteTableAttribute), true).FirstOrDefault();
                    if (tableAttr != null && tableAttr.Name != null && tableAttr.Name.Length > 0)
                    {
                        table = tableAttr.Name;
                    }
                    else
                    {
                        table = type.Name;
                    }
                }
                return table;
            });
        }

        public static PropertyInfo[] GetCachedProperties(this Type type)
        {
            return GetCache(type.FullName + "_props", () =>
            {
                return type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            });
        }

        public static object[] GetCachedCustomAttributes(this MemberInfo member, Type attributeType, bool inherit)
        {
            return GetCache(member.DeclaringType.FullName + "." + member.Name + "_attrs_" + attributeType.FullName, () =>
            {
                return member.GetCustomAttributes(attributeType, inherit);
            });
        }

        public static object[] GetCachedCustomAttributes(this MemberInfo member)
        {
            return GetCache(member.DeclaringType.FullName + "." + member.Name + "_attrs", () =>
            {
                return member.GetCustomAttributes(true);
            });
        }

        private static T GetCache<T>(string key, Func<T> get)
        {
            object cval;
            if (Dict.TryGetValue(key, out cval))
            {
                return (T)cval;
            }
            var val = get();
            Dict[key] = val;
            return val;
        }
    }
}
