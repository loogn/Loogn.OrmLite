using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Loogn.OrmLite
{

    static class ReflectionHelper
    {
        static Dictionary<Type, object> ReflectionInfoCache = new Dictionary<Type, object>(50);
        //用于匿名对象
        static Dictionary<Type, PropertyInfo[]> PropertysDict = new Dictionary<Type, PropertyInfo[]>();
        public static ReflectionInfo<TObject> GetInfo<TObject>()
        {
            var type = typeof(TObject);
            object info;
            if (ReflectionInfoCache.TryGetValue(type, out info))
            {
                return (ReflectionInfo<TObject>)info;
            }
            else
            {
                var refInfo = new ReflectionInfo<TObject>(type);
                ReflectionInfoCache[type] = refInfo;
                return refInfo;
            }
        }


        public static PropertyInfo[] GetCachedProperties(Type type)
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

    }

    class ReflectionInfo<TObject>
    {
        public string TableName { get; set; }

        // public string PrimaryKey { get; set; }

        private Dictionary<string, Setter> setterDict;
        private Dictionary<string, Getter> getterDict;
        public Dictionary<PropertyInfo, OrmLiteFieldAttribute> FieldAttrDict { get; private set; }
        public PropertyInfo[] Properties { get; }

        public ReflectionInfo(Type modelType)
        {
            var tableAttr = modelType.GetCustomAttributes(typeof(OrmLiteTableAttribute), true).FirstOrDefault() as OrmLiteTableAttribute;
            if (tableAttr != null && tableAttr.Name != null && tableAttr.Name.Length != 0)
            {
                TableName = tableAttr.Name;
            }
            else
            {
                if (modelType.Name.EndsWith("Info", StringComparison.OrdinalIgnoreCase))
                {
                    TableName = modelType.Name.Substring(0, modelType.Name.Length - 4);
                }
                else if (modelType.Name.EndsWith("Model", StringComparison.OrdinalIgnoreCase))
                {
                    TableName = modelType.Name.Substring(0, modelType.Name.Length - 5);
                }
                else
                {
                    TableName = modelType.Name;
                }
            }

            Properties = modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            InitInfo();
            // PrimaryKey = primaryKey;
        }

        private void InitInfo()
        {
            setterDict = new Dictionary<string, Setter>(Properties.Length);
            getterDict = new Dictionary<string, Getter>(Properties.Length);
            FieldAttrDict = new Dictionary<PropertyInfo, OrmLiteFieldAttribute>(Properties.Length);
            foreach (var prop in Properties)
            {
                Delegate getter = null;
                Delegate setter = null;
                string propName = prop.Name.ToUpper();
                var propType = prop.PropertyType;

                if (propType.IsEnum)
                {
                    propType = propType.GetEnumUnderlyingType();
                }
                
                if (typeof(string) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, string>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, string>), null, prop.GetSetMethod(true));
                }
                else if (typeof(int) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, int>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, int>), null, prop.GetSetMethod(true));
                }
                else if (typeof(int?) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, int?>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, int?>), null, prop.GetSetMethod(true));
                }

                else if (typeof(DateTime) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, DateTime>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, DateTime>), null, prop.GetSetMethod(true));
                }
                else if (typeof(DateTime?) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, DateTime?>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, DateTime?>), null, prop.GetSetMethod(true));
                }
                else if (typeof(long) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, long>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, long>), null, prop.GetSetMethod(true));
                }
                else if (typeof(long?) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, long?>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, long?>), null, prop.GetSetMethod(true));
                }
                else if (typeof(float) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, float>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, float>), null, prop.GetSetMethod(true));
                }
                else if (typeof(float?) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, float?>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, float?>), null, prop.GetSetMethod(true));
                }
                else if (typeof(double) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, double>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, double>), null, prop.GetSetMethod(true));
                }
                else if (typeof(double?) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, double?>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, double?>), null, prop.GetSetMethod(true));
                }
                else if (typeof(Guid) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, Guid>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, Guid>), null, prop.GetSetMethod(true));
                }
                else if (typeof(Guid?) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, Guid?>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, Guid?>), null, prop.GetSetMethod(true));
                }
                else if (typeof(short) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, short>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, short>), null, prop.GetSetMethod(true));
                }
                else if (typeof(short?) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, short?>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, short?>), null, prop.GetSetMethod(true));
                }
                else if (typeof(byte) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, byte>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, byte>), null, prop.GetSetMethod(true));
                }
                else if (typeof(byte?) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, byte?>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, byte?>), null, prop.GetSetMethod(true));
                }
                else if (typeof(char) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, char>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, char>), null, prop.GetSetMethod(true));
                }
                else if (typeof(char?) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, char?>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, char?>), null, prop.GetSetMethod(true));
                }
                else if (typeof(decimal) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, decimal>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, decimal>), null, prop.GetSetMethod(true));
                }
                else if (typeof(decimal?) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, decimal?>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, decimal?>), null, prop.GetSetMethod(true));
                }
                else if (typeof(byte[]) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, byte[]>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, byte[]>), null, prop.GetSetMethod(true));
                }
                else if (typeof(bool) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, bool>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, bool>), null, prop.GetSetMethod(true));
                }
                else if (typeof(bool?) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, bool?>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, bool?>), null, prop.GetSetMethod(true));
                }
                else if (typeof(TimeSpan?) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, TimeSpan?>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, TimeSpan?>), null, prop.GetSetMethod(true));
                }
                else if (typeof(TimeSpan) == propType)
                {
                    getter = Delegate.CreateDelegate(typeof(Func<TObject, TimeSpan>), null, prop.GetGetMethod(true));
                    setter = Delegate.CreateDelegate(typeof(Action<TObject, TimeSpan>), null, prop.GetSetMethod(true));
                }
                else
                {
                    setter = null;
                    getter = null;
                }

                setterDict[propName] = new Setter(setter, propType);
                getterDict[propName] = new Getter(getter, propType);


                //自定义属性
                var customerAttributes = prop.GetCustomAttributes(typeof(OrmLiteFieldAttribute), false);
                if (customerAttributes == null || customerAttributes.Length == 0)
                {
                    FieldAttrDict[prop] = null;
                }
                else
                {
                    FieldAttrDict[prop] = (OrmLiteFieldAttribute)customerAttributes[0];
                }

            }
        }



        public Setter GetSetter(string fieldName)
        {
            Setter setter;
            if (setterDict.TryGetValue(fieldName.ToUpper(), out setter))
            {
                return setter;
            }
            return null;
        }

        public Getter GetGetter(string fieldName)
        {
            Getter getter;
            if (getterDict.TryGetValue(fieldName.ToUpper(), out getter))
            {
                return getter;
            }
            return null;
        }

        public OrmLiteFieldAttribute GetFieldAttr(PropertyInfo prop)
        {
            OrmLiteFieldAttribute attr;
            if (FieldAttrDict.TryGetValue(prop, out attr))
            {
                return attr;
            }
            else
            {
                return null;
            }
        }

        public class Setter
        {
            Delegate setter;
            Type fieldType;
            public Setter(Delegate setter, Type fieldType)
            {
                this.setter = setter;
                this.fieldType = fieldType;
            }

            public void Set(TObject obj, object value)
            {
                if (value == null || value is DBNull)
                {
                    return;
                }

                if (ReferenceEquals(fieldType, typeof(string)))
                {
                    ((Action<TObject, string>)setter)(obj, (string)value);
                }
                else if (ReferenceEquals(fieldType, typeof(int)))
                {
                    ((Action<TObject, int>)setter)(obj, (int)value);
                }
                else if (ReferenceEquals(fieldType, typeof(int?)))
                {
                    ((Action<TObject, int?>)setter)(obj, (int)value);
                }
                else if (ReferenceEquals(fieldType, typeof(DateTime)))
                {
                    ((Action<TObject, DateTime>)setter)(obj, (DateTime)value);
                }
                else if (ReferenceEquals(fieldType, typeof(DateTime?)))
                {
                    ((Action<TObject, DateTime?>)setter)(obj, (DateTime)value);
                }
                else if (ReferenceEquals(fieldType, typeof(long)))
                {
                    ((Action<TObject, long>)setter)(obj, (long)value);
                }
                else if (ReferenceEquals(fieldType, typeof(long?)))
                {
                    ((Action<TObject, long?>)setter)(obj, (long)value);
                }
                else if (ReferenceEquals(fieldType, typeof(double)))
                {
                    ((Action<TObject, double>)setter)(obj, (double)value);
                }
                else if (ReferenceEquals(fieldType, typeof(double?)))
                {
                    ((Action<TObject, double?>)setter)(obj, (double)value);
                }
                else if (ReferenceEquals(fieldType, typeof(Guid)))
                {
                    ((Action<TObject, Guid>)setter)(obj, (Guid)value);
                }
                else if (ReferenceEquals(fieldType, typeof(Guid?)))
                {
                    ((Action<TObject, Guid?>)setter)(obj, (Guid)value);
                }
                else if (ReferenceEquals(fieldType, typeof(float)))
                {
                    ((Action<TObject, float>)setter)(obj, (float)value);
                }
                else if (ReferenceEquals(fieldType, typeof(float?)))
                {
                    ((Action<TObject, float?>)setter)(obj, (float)value);
                }
                else if (ReferenceEquals(fieldType, typeof(byte)))
                {
                    ((Action<TObject, byte>)setter)(obj, Convert.ToByte(value));
                }
                else if (ReferenceEquals(fieldType, typeof(byte?)))
                {
                    ((Action<TObject, byte?>)setter)(obj, (byte)value);
                }
                else if (ReferenceEquals(fieldType, typeof(char)))
                {
                    ((Action<TObject, char>)setter)(obj, (char)value);
                }
                else if (ReferenceEquals(fieldType, typeof(char?)))
                {
                    ((Action<TObject, char?>)setter)(obj, (char)value);
                }
                else if (ReferenceEquals(fieldType, typeof(bool)))
                {
                    bool theValue = false;
                    if (value.GetType() == typeof(bool))
                    {
                        theValue = (bool)value;
                    }
                    else
                    {
                        var intValue = Convert.ToInt32(value);
                        theValue = intValue > 0;
                    }
                    ((Action<TObject, bool>)setter)(obj, theValue);
                }
                else if (ReferenceEquals(fieldType, typeof(bool?)))
                {
                    bool theValue = false;
                    if (value.GetType() == typeof(bool))
                    {
                        theValue = (bool)value;
                    }
                    else
                    {
                        var intValue = Convert.ToInt32(value);
                        theValue = intValue > 0;
                    }
                    ((Action<TObject, bool?>)setter)(obj, theValue);
                }
                else if (ReferenceEquals(fieldType, typeof(decimal)))
                {
                    ((Action<TObject, decimal>)setter)(obj, (decimal)value);
                }
                else if (ReferenceEquals(fieldType, typeof(decimal?)))
                {
                    ((Action<TObject, decimal?>)setter)(obj, (decimal)value);
                }
                else if (ReferenceEquals(fieldType, typeof(byte[])))
                {
                    ((Action<TObject, byte[]>)setter)(obj, (byte[])value);
                }
            }
        }


        public class Getter
        {
            Delegate getter;
            Type fieldType;
            public Getter(Delegate getter, Type fieldType)
            {
                this.getter = getter;
                this.fieldType = fieldType;
            }

            public object Get(TObject obj)
            {
                if (ReferenceEquals(fieldType, typeof(string)))
                {
                    return ((Func<TObject, string>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(int)))
                {
                    return ((Func<TObject, int>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(int?)))
                {
                    return ((Func<TObject, int?>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(DateTime)))
                {
                    return ((Func<TObject, DateTime>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(DateTime?)))
                {
                    return ((Func<TObject, DateTime?>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(long)))
                {
                    return ((Func<TObject, long>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(long?)))
                {
                    return ((Func<TObject, long?>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(double)))
                {
                    return ((Func<TObject, double>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(double?)))
                {
                    return ((Func<TObject, double?>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(Guid)))
                {
                    return ((Func<TObject, Guid>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(Guid?)))
                {
                    return ((Func<TObject, Guid?>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(float)))
                {
                    return ((Func<TObject, float>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(float?)))
                {
                    return ((Func<TObject, float?>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(byte)))
                {
                    return ((Func<TObject, byte>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(byte?)))
                {
                    return ((Func<TObject, byte?>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(char)))
                {
                    return ((Func<TObject, char>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(char?)))
                {
                    return ((Func<TObject, char?>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(bool)))
                {
                    return ((Func<TObject, bool>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(bool?)))
                {
                    return ((Func<TObject, bool?>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(decimal)))
                {
                    return ((Func<TObject, decimal>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(decimal?)))
                {
                    return ((Func<TObject, decimal?>)getter)(obj);
                }
                else if (ReferenceEquals(fieldType, typeof(byte[])))
                {
                    return ((Func<TObject, byte[]>)getter)(obj);
                }
                else
                {
                    return null;
                }
            }
        }
    }


}
