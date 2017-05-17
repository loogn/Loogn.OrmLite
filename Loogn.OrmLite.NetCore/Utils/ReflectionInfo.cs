using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Loogn.OrmLite.NetCore.Utils;

namespace Loogn.OrmLite.NetCore
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

       

        public static string GetReaderUniqueKey(DbDataReader reader)
        {
            var names = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                names[i] = reader.GetName(i);
            }
            return string.Join("_", names);
        }

    }

    class ReflectionInfo<TObject>
    {
        public string TableName { get; set; }


        private Dictionary<string, Accessor> accessorDict;
        public Dictionary<PropertyInfo, OrmLiteFieldAttribute> FieldAttrDict { get; private set; }
        public PropertyInfo[] Properties { get; set; }
        public Func<TObject> NewInstance;
        public ReflectionInfo(Type modelType)
        {
            var tableAttr = modelType.GetTypeInfo().GetCustomAttributes(PrimitiveTypes.OrmLiteTable, true).FirstOrDefault() as OrmLiteTableAttribute;
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

            //构造委托
            NewInstance = Expression.Lambda<Func<TObject>>(Expression.New(modelType)).Compile();
 
            InitInfo();
        }

        private void InitInfo()
        {
            accessorDict = new Dictionary<string, Accessor>(Properties.Length);
            FieldAttrDict = new Dictionary<PropertyInfo, OrmLiteFieldAttribute>(Properties.Length);
            foreach (var prop in Properties)
            {
                Accessor accessor = null;

                string propName = prop.Name.ToUpper();
                var propType = prop.PropertyType;

                if (ReferenceEquals(PrimitiveTypes.String, propType))
                {
                    accessor = new StringAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.Int32, propType))
                {
                    accessor = new IntAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.NullableInt32, propType))
                {
                    accessor = new IntNullableAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.DateTime, propType))
                {
                    accessor = new DateTimeAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.NullableDateTime, propType))
                {
                    accessor = new DateTimeNullableAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.Int64, propType))
                {
                    accessor = new LongAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.NullableInt64, propType))
                {
                    accessor = new LongNullableAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.Single, propType))
                {
                    accessor = new FloatAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.NullableSingle, propType))
                {
                    accessor = new FloatNullableAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.Double, propType))
                {
                    accessor = new DoubleAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.NullableDouble, propType))
                {
                    accessor = new DoubleNullableAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.Guid, propType))
                {
                    accessor = new GuidAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.NullableGuid, propType))
                {
                    accessor = new GuidNullableAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.Int16, propType))
                {
                    accessor = new ShortAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.NullableInt16, propType))
                {
                    accessor = new ShortNullableAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.Byte, propType))
                {
                    accessor = new ByteAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.NullableByte, propType))
                {
                    accessor = new ByteNullableAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.Char, propType))
                {
                    accessor = new CharAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.NullableChar, propType))
                {
                    accessor = new CharNullableAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.Decimal, propType))
                {
                    accessor = new DecimalAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.NullableDecimal, propType))
                {
                    accessor = new DecimalNullableAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.ByteArray, propType))
                {
                    accessor = new ByteArrayAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.Bool, propType))
                {
                    accessor = new BoolAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.NullableBool, propType))
                {
                    accessor = new BoolNullableAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.TimeSpan, propType))
                {
                    accessor = new TimeSpanAccessor(prop);
                }
                else if (ReferenceEquals(PrimitiveTypes.NullableTimeSpan, propType))
                {
                    accessor = new TimeSpanNullableAccessor(prop);
                }
                accessorDict[propName] = accessor;
                //自定义属性
                var customerAttributes = prop.GetCustomAttributes(PrimitiveTypes.OrmLiteField, false);
                if (customerAttributes == null || !customerAttributes.Any())
                {
                    FieldAttrDict[prop] = null;
                }
                else
                {
                    FieldAttrDict[prop] = (OrmLiteFieldAttribute)customerAttributes.First();
                }
            }
        }


        public Accessor GetAccessor(string fieldName)
        {
            Accessor accessor;
            if (accessorDict.TryGetValue(fieldName.ToUpper(), out accessor))
            {
                return accessor;
            }
            return new EmptyAccessor();
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


        public abstract class Accessor
        {
            Action<TObject, string> setter;
            Func<TObject, string> getter;
            PropertyInfo _prop;
            public Accessor(PropertyInfo prop)
            {
                _prop = prop;
            }

            public void Set(TObject obj, object value)
            {
                if (value == null || value is DBNull)
                {
                    return;
                }
                try
                {
                    DoSet(obj, value);
                }
                catch
                {
                    throw new Exception(string.Format("将{0}类型的值{1}赋给{2}.{3}时失败", value.GetType().FullName, value, obj.GetType().FullName, _prop.Name));
                }
            }

            public object Get(TObject obj)
            {
                return DoGet(obj);
            }

            protected abstract void DoSet(TObject obj, object value);
            protected abstract object DoGet(TObject obj);

        }

        #region Accessor
        public class EmptyAccessor : Accessor
        {
            public EmptyAccessor() : base(null) { }
            protected override object DoGet(TObject obj)
            {
                return null;
            }

            protected override void DoSet(TObject obj, object value)
            {
                return;
            }
        }

        public class StringAccessor : Accessor
        {
            Action<TObject, string> setter;
            Func<TObject, string> getter;

            public StringAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, string>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {

                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, string>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (string)value);
                }

            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? null : getter(obj);
            }
        }

        public class IntAccessor : Accessor
        {
            Action<TObject, int> setter;
            Func<TObject, int> getter;
            public IntAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, int>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, int>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (int)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? 0 : getter(obj);
            }
        }

        public class IntNullableAccessor : Accessor
        {
            Action<TObject, int?> setter;
            Func<TObject, int?> getter;
            public IntNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, int?>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, int?>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (int)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? 0 : getter(obj);
            }
        }

        public class DateTimeAccessor : Accessor
        {
            Action<TObject, DateTime> setter;
            Func<TObject, DateTime> getter;
            public DateTimeAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, DateTime>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, DateTime>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (DateTime)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? DateTime.MinValue : getter(obj);
            }
        }

        public class DateTimeNullableAccessor : Accessor
        {
            Action<TObject, DateTime?> setter;
            Func<TObject, DateTime?> getter;
            public DateTimeNullableAccessor(PropertyInfo prop)
                : base(prop)
            {

                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, DateTime?>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, DateTime?>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (DateTime?)value);
                }
            }

            protected override object DoGet(TObject obj)
            {
                return getter == null ? null : getter(obj);
            }
        }

        public class LongAccessor : Accessor
        {
            Action<TObject, long> setter;
            Func<TObject, long> getter;
            public LongAccessor(PropertyInfo prop)
                : base(prop)
            {

                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, long>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, long>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (long)value);
                }
            }

            protected override object DoGet(TObject obj)
            {
                return getter == null ? 0 : getter(obj);
            }
        }

        public class LongNullableAccessor : Accessor
        {
            Action<TObject, long?> setter;
            Func<TObject, long?> getter;
            public LongNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, long?>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, long?>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (long)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? null : getter(obj);
            }
        }

        public class DoubleAccessor : Accessor
        {
            Action<TObject, double> setter;
            Func<TObject, double> getter;
            public DoubleAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, double>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, double>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (double)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? 0 : getter(obj);
            }
        }

        public class DoubleNullableAccessor : Accessor
        {
            Action<TObject, double?> setter;
            Func<TObject, double?> getter;
            public DoubleNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, double?>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, double?>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (double)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? null : getter(obj);
            }
        }

        public class FloatAccessor : Accessor
        {
            Action<TObject, float> setter;
            Func<TObject, float> getter;
            public FloatAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, float>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, float>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (float)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? 0 : getter(obj);
            }
        }

        public class FloatNullableAccessor : Accessor
        {
            Action<TObject, float?> setter;
            Func<TObject, float?> getter;
            public FloatNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, float?>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, float?>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (float)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? null : getter(obj);
            }
        }

        public class GuidAccessor : Accessor
        {
            Action<TObject, Guid> setter;
            Func<TObject, Guid> getter;
            public GuidAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, Guid>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, Guid>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (Guid)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? Guid.Empty : getter(obj);
            }
        }

        public class GuidNullableAccessor : Accessor
        {
            Action<TObject, Guid?> setter;
            Func<TObject, Guid?> getter;
            public GuidNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, Guid?>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, Guid?>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (Guid)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? null : getter(obj);
            }
        }

        public class ByteAccessor : Accessor
        {
            Action<TObject, byte> setter;
            Func<TObject, byte> getter;
            public ByteAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, byte>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, byte>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    if (value is byte)
                    {
                        setter(obj, (byte)value);
                    }
                    else
                    {
                        setter(obj, Convert.ToByte(value));
                    }
                }

            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? 0 : getter(obj);
            }
        }
        public class ByteNullableAccessor : Accessor
        {
            Action<TObject, byte?> setter;
            Func<TObject, byte?> getter;

            public ByteNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, byte?>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, byte?>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    if (value is byte)
                    {
                        setter(obj, (byte)value);
                    }
                    else
                    {
                        setter(obj, Convert.ToByte(value));
                    }
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? null : getter(obj);
            }
        }

        public class ShortAccessor : Accessor
        {
            Action<TObject, short> setter;
            Func<TObject, short> getter;
            public ShortAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, short>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, short>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (short)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? 0 : getter(obj);
            }
        }
        public class ShortNullableAccessor : Accessor
        {
            Action<TObject, short?> setter;
            Func<TObject, short?> getter;
            public ShortNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, short?>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, short?>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (short)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? 0 : getter(obj);
            }
        }

        public class CharAccessor : Accessor
        {
            Action<TObject, char> setter;
            Func<TObject, char> getter;
            public CharAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, char>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, char>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (char)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? char.MinValue : getter(obj);
            }
        }

        public class CharNullableAccessor : Accessor
        {
            Action<TObject, char?> setter;
            Func<TObject, char?> getter;
            public CharNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, char?>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, char?>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (char)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? null : getter(obj);
            }
        }

        public class BoolAccessor : Accessor
        {
            Action<TObject, bool> setter;
            Func<TObject, bool> getter;
            public BoolAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, bool>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, bool>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    if (value is bool)
                    {
                        setter(obj, (bool)value);
                    }
                    else
                    {
                        setter(obj, Convert.ToUInt16(value) > 0);
                    }
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? false : getter(obj);
            }
        }

        public class BoolNullableAccessor : Accessor
        {
            Action<TObject, bool?> setter;
            Func<TObject, bool?> getter;
            public BoolNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, bool?>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, bool?>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    if (value is bool)
                    {
                        setter(obj, (bool)value);
                    }
                    else
                    {
                        setter(obj, Convert.ToUInt16(value) > 0);
                    }
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? null : getter(obj);
            }
        }

        public class TimeSpanAccessor : Accessor
        {
            Action<TObject, TimeSpan> setter;
            Func<TObject, TimeSpan> getter;
            public TimeSpanAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, TimeSpan>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, TimeSpan>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (TimeSpan)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? TimeSpan.Zero : getter(obj);
            }
        }

        public class TimeSpanNullableAccessor : Accessor
        {
            Action<TObject, TimeSpan?> setter;
            Func<TObject, TimeSpan?> getter;
            public TimeSpanNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, TimeSpan?>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, TimeSpan?>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (TimeSpan)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? null : getter(obj);
            }
        }

        public class DecimalAccessor : Accessor
        {
            Action<TObject, decimal> setter;
            Func<TObject, decimal> getter;
            public DecimalAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, decimal>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, decimal>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (decimal)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? 0M : getter(obj);
            }
        }

        public class DecimalNullableAccessor : Accessor
        {
            Action<TObject, decimal?> setter;
            Func<TObject, decimal?> getter;
            public DecimalNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, decimal?>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, decimal?>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (decimal)value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? null : getter(obj);
            }
        }

        public class ByteArrayAccessor : Accessor
        {
            Action<TObject, byte[]> setter;
            Func<TObject, byte[]> getter;
            public ByteArrayAccessor(PropertyInfo prop)
                : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    setter = DynamicMethodBuilder.BuildSetterInvoker<TObject, byte[]>(prop);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {
                    getter = DynamicMethodBuilder.BuildGetterInvoker<TObject, byte[]>(prop);
                }
            }
            protected override void DoSet(TObject obj, object value)
            {
                if (setter != null)
                {
                    setter(obj, (byte[])value);
                }
            }
            protected override object DoGet(TObject obj)
            {
                return getter == null ? null : getter(obj);
            }
        }

        #endregion

        private Dictionary<string, Action<TObject, object[]>> setterActionDict = new Dictionary<string, Action<TObject, object[]>>();
        public Action<TObject, object[]> GetSetterAction(DbDataReader reader)
        {
            var key = ReflectionHelper.GetReaderUniqueKey(reader);
            Action<TObject, object[]> action = null;
            if (setterActionDict.TryGetValue(key, out action))
            {
                return action;
            }

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var accessor = GetAccessor(reader.GetName(i));
                var j = i; //这里不能省，很重要
                action += new Action<TObject, object[]>((obj, values) =>
                {
                    accessor.Set(obj, values[j]);
                });
            }
            if (action == null)
            {
                action = new Action<TObject, object[]>((obj, values) => { });
            }
            setterActionDict[key] = action;
            return action;
        }
    }


}
