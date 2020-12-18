using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{

    /// <summary>
    /// 非泛型类型信息
    /// </summary>
    class TypeCachedInfo
    {
        public string TableName;
        public Func<object> NewInvoker;
        public Dictionary<string, PropAccessor> PropInvokerDict;
        public TypeCachedInfo(Type type)
        {
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
        private Action<object, object> SetterInvoker;
        private Func<object, object> GetterInvoker;
        public bool CanInvoker;
        private PropAccessor()
        {
            SetterInvoker = DynamicMethodHelper.BuildSetterInvoker(null);
            GetterInvoker = DynamicMethodHelper.BuildGetterInvoker(null);
            CanInvoker = false;
        }
        public void Set(object obj, object value)
        {
            if (value != null && value != DBNull.Value)
            {
                if (Property.PropertyType == Types.Bool && !(value is bool))
                {
                    SetterInvoker(obj, Convert.ToBoolean(value));
                    return;
                }
                if (Property.PropertyType == Types.Byte && !(value is byte))
                {
                    SetterInvoker(obj, Convert.ToByte(value));
                    return;
                }
                SetterInvoker(obj, value);
            }
        }
        public object Get(object obj)
        {
            return GetterInvoker(obj);
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

    /// <summary>
    /// 泛型类型信息
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    class TypeCachedInfo<TObject>
    {
        public string TableName { get; set; }

        public Dictionary<string, Accessor> accessorDict;

        public Func<TObject> NewInvoker;
        public TypeCachedInfo(Type modelType)
        {
            var tableAttr = modelType.GetCustomAttributes(Types.OrmLiteTable, true).FirstOrDefault() as OrmLiteTableAttribute;
            if (tableAttr != null && tableAttr.Name != null && tableAttr.Name.Length != 0)
            {
                TableName = tableAttr.Name;
            }
            else
            {
                TableName = modelType.Name;
            }
            //构造委托
            NewInvoker = DynamicMethodHelper.BuildConstructorInvoker<TObject>(modelType);
            var Properties = modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            accessorDict = new Dictionary<string, Accessor>(Properties.Length);

            //var Fields = modelType.GetFields(BindingFlags.Instance | BindingFlags.Public);

            foreach (var prop in Properties)
            {
                string propName = prop.Name.ToUpper();
                accessorDict[propName] = new Accessor(prop);
            }
        }

        public Accessor GetAccessor(string fieldName)
        {
            Accessor accessor;
            if (accessorDict.TryGetValue(fieldName.ToUpper(), out accessor))
            {
                return accessor;
            }
            return new Accessor(null);
        }

        public class Accessor
        {
            public bool CanSet;
            public bool CanGet;
            private Action<object, object> setter;
            private Func<object, object> getter;
            public OrmLiteFieldAttribute OrmLiteField { get; set; }
            public PropertyInfo prop;
            public Accessor(PropertyInfo prop)
            {
                this.prop = prop;
                //自定义属性
                if (prop != null)
                {
                    var customerAttributes = prop.GetCustomAttributes(Types.OrmLiteField, false);
                    if (customerAttributes != null && customerAttributes.Length > 0)
                    {
                        OrmLiteField = (OrmLiteFieldAttribute)customerAttributes[0];
                    }
                    var setMethod = prop.GetSetMethod(true);
                    if (setMethod != null)
                    {
                        CanSet = true;
                        setter = DynamicMethodHelper.BuildSetterInvoker(setMethod);
                    }
                    var getMethod = prop.GetGetMethod(true);
                    if (getMethod != null)
                    {

                        CanGet = true;
                        getter = DynamicMethodHelper.BuildGetterInvoker(getMethod);
                    }
                }
            }

            public void Set(TObject obj, object value)
            {
                if (value == null || value is DBNull || !CanSet)
                {
                    return;
                }
                try
                {
                    setter(obj, value);
                }
                catch
                {
                    throw new Exception(string.Format("将{0}类型的值{1}赋给{2}.{3}时失败", value.GetType().FullName, value, obj.GetType().FullName, prop.Name));
                }
            }

            public object Get(TObject obj)
            {
                if (CanGet)
                {
                    return getter(obj);
                }
                else
                {
                    throw new Exception("获取属性失败");
                }
            }

        }

    }
}
