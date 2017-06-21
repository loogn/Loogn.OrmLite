using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    class TypeCachedInfo
    {
        public Type Type;
        public string TableName;
        public Func<object> NewInvoker;
        public Dictionary<string, PropAccessor> PropInvokerDict;
        public TypeCachedInfo(Type type)
        {
            Type = type;
            var tableAttr = type.GetCustomAttributes(Types.OrmLiteTable, true).FirstOrDefault() as OrmLiteTableAttribute;
            if (tableAttr != null && tableAttr.Name != null && tableAttr.Name.Length != 0)
            {
                TableName = tableAttr.Name;
            }
            else
            {
                TableName = type.Name;
            }
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            PropInvokerDict = new Dictionary<string, PropAccessor>(properties.Length, StringComparer.OrdinalIgnoreCase);
            foreach (var prop in properties)
            {
                PropAccessor accessor = new PropAccessor(prop);
                PropInvokerDict[prop.Name] = accessor;
            }

            NewInvoker = DynamicMethodHelper.BuildConstructorInvoker(type);
        }

        public PropAccessor GetAccessor(string propName)
        {
            PropAccessor accessor;
            if (PropInvokerDict.TryGetValue(propName, out accessor))
            {
                return accessor;
            }
            else
            {
                return PropAccessor.Empty;
            }
        }
    }

    class PropAccessor
    {
        public static PropAccessor Empty = new PropAccessor();

        public PropertyInfo Property;
        public OrmLiteFieldAttribute OrmLiteField;
        public Action<object, object> SetterInvoker;
        public Func<object, object> GetterInvoker;
        public bool CanInvoker;
        private PropAccessor()
        {
            SetterInvoker = DynamicMethodHelper.BuildSetterInvoker(null);
            GetterInvoker = DynamicMethodHelper.BuildGetterInvoker(null);
            CanInvoker = false;
        }
        public PropAccessor(PropertyInfo prop)
        {
            Property = prop;
            //自定义属性
            var customerAttributes = prop.GetCustomAttributes(Types.OrmLiteField, false);
            if (customerAttributes != null && customerAttributes.Length > 0)
            {
                OrmLiteField = (OrmLiteFieldAttribute)customerAttributes[0];
            }
            SetterInvoker = DynamicMethodHelper.BuildSetterInvoker(prop.GetSetMethod(true));
            GetterInvoker = DynamicMethodHelper.BuildGetterInvoker(prop.GetGetMethod(true));
            CanInvoker = true;
        }
    }
}
