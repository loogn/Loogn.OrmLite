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
        public Type Type;
        public string TableName { get; set; }

        public Dictionary<string, Accessor> accessorDict;

        public Func<TObject> NewInvoker;
        public TypeCachedInfo(Type modelType)
        {
            Type = modelType;
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
            InitInfo();
        }

        private void InitInfo()
        {
            var Properties = Type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            accessorDict = new Dictionary<string, Accessor>(Properties.Length);

            foreach (var prop in Properties)
            {
                Accessor accessor = null;

                string propName = prop.Name.ToUpper();
                var propType = prop.PropertyType;

                if (ReferenceEquals(Types.String, propType))
                {
                    accessor = new StringAccessor(prop);
                }
                else if (ReferenceEquals(Types.Int32, propType))
                {
                    accessor = new IntAccessor(prop);
                }
                else if (ReferenceEquals(Types.NullableInt32, propType))
                {
                    accessor = new IntNullableAccessor(prop);
                }
                else if (ReferenceEquals(Types.DateTime, propType))
                {
                    accessor = new DateTimeAccessor(prop);
                }
                else if (ReferenceEquals(Types.NullableDateTime, propType))
                {
                    accessor = new DateTimeNullableAccessor(prop);
                }
                else if (ReferenceEquals(Types.Int64, propType))
                {
                    accessor = new LongAccessor(prop);
                }
                else if (ReferenceEquals(Types.NullableInt64, propType))
                {
                    accessor = new LongNullableAccessor(prop);
                }
                else if (ReferenceEquals(Types.Single, propType))
                {
                    accessor = new FloatAccessor(prop);
                }
                else if (ReferenceEquals(Types.NullableSingle, propType))
                {
                    accessor = new FloatNullableAccessor(prop);
                }
                else if (ReferenceEquals(Types.Double, propType))
                {
                    accessor = new DoubleAccessor(prop);
                }
                else if (ReferenceEquals(Types.NullableDouble, propType))
                {
                    accessor = new DoubleNullableAccessor(prop);
                }
                else if (ReferenceEquals(Types.Guid, propType))
                {
                    accessor = new GuidAccessor(prop);
                }
                else if (ReferenceEquals(Types.NullableGuid, propType))
                {
                    accessor = new GuidNullableAccessor(prop);
                }
                else if (ReferenceEquals(Types.Int16, propType))
                {
                    accessor = new ShortAccessor(prop);
                }
                else if (ReferenceEquals(Types.NullableInt16, propType))
                {
                    accessor = new ShortNullableAccessor(prop);
                }
                else if (ReferenceEquals(Types.Byte, propType))
                {
                    accessor = new ByteAccessor(prop);
                }
                else if (ReferenceEquals(Types.NullableByte, propType))
                {
                    accessor = new ByteNullableAccessor(prop);
                }
                else if (ReferenceEquals(Types.Char, propType))
                {
                    accessor = new CharAccessor(prop);
                }
                else if (ReferenceEquals(Types.NullableChar, propType))
                {
                    accessor = new CharNullableAccessor(prop);
                }
                else if (ReferenceEquals(Types.Decimal, propType))
                {
                    accessor = new DecimalAccessor(prop);
                }
                else if (ReferenceEquals(Types.NullableDecimal, propType))
                {
                    accessor = new DecimalNullableAccessor(prop);
                }
                else if (ReferenceEquals(Types.ByteArray, propType))
                {
                    accessor = new ByteArrayAccessor(prop);
                }
                else if (ReferenceEquals(Types.Bool, propType))
                {
                    accessor = new BoolAccessor(prop);
                }
                else if (ReferenceEquals(Types.NullableBool, propType))
                {
                    accessor = new BoolNullableAccessor(prop);
                }
                else if (ReferenceEquals(Types.TimeSpan, propType))
                {
                    accessor = new TimeSpanAccessor(prop);
                }
                else if (ReferenceEquals(Types.NullableTimeSpan, propType))
                {
                    accessor = new TimeSpanNullableAccessor(prop);
                }
                accessorDict[propName] = accessor;
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

        public abstract class Accessor
        {
            public bool CanSet;
            public bool CanGet;
            public OrmLiteFieldAttribute OrmLiteField;
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
                }
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
                    throw new Exception(string.Format("将{0}类型的值{1}赋给{2}.{3}时失败", value.GetType().FullName, value, obj.GetType().FullName, prop.Name));
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
        public abstract class AccessorTpl<TValue> : Accessor
        {
            protected Action<TObject, TValue> setter;
            protected Func<TObject, TValue> getter;
            public AccessorTpl(PropertyInfo prop) : base(prop)
            {
                var setMethod = prop.GetSetMethod(true);
                if (setMethod != null)
                {
                    CanSet = true;
                    setter = (Action<TObject, TValue>)Delegate.CreateDelegate(typeof(Action<TObject, TValue>), setMethod);
                }
                var getMethod = prop.GetGetMethod(true);
                if (getMethod != null)
                {

                    CanGet = true;
                    getter = (Func<TObject, TValue>)Delegate.CreateDelegate(typeof(Func<TObject, TValue>), getMethod);
                }
            }
        }


        public class StringAccessor : AccessorTpl<String>
        {
            public StringAccessor(PropertyInfo prop) : base(prop)
            {
            }

            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (string)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? null : getter(obj);
            }
        }

        public class IntAccessor : AccessorTpl<int>
        {
            public IntAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (int)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? 0 : getter(obj);
            }
        }

        public class IntNullableAccessor : AccessorTpl<int?>
        {
            public IntNullableAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (int)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? 0 : getter(obj);
            }
        }

        public class DateTimeAccessor : AccessorTpl<DateTime>
        {
            public DateTimeAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (DateTime)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? DateTime.MinValue : getter(obj);
            }
        }

        public class DateTimeNullableAccessor : AccessorTpl<DateTime?>
        {
            public DateTimeNullableAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (DateTime?)value);
            }

            protected override object DoGet(TObject obj)
            {
                return !CanGet ? null : getter(obj);
            }
        }

        public class LongAccessor : AccessorTpl<long>
        {
            public LongAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (long)value);
            }

            protected override object DoGet(TObject obj)
            {
                return !CanGet ? 0 : getter(obj);
            }
        }

        public class LongNullableAccessor : AccessorTpl<long?>
        {
            public LongNullableAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (long)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? null : getter(obj);
            }
        }

        public class DoubleAccessor : AccessorTpl<double>
        {
            public DoubleAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (double)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? 0 : getter(obj);
            }
        }

        public class DoubleNullableAccessor : AccessorTpl<double?>
        {
            public DoubleNullableAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (double)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? null : getter(obj);
            }
        }

        public class FloatAccessor : AccessorTpl<float>
        {
            public FloatAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (float)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? 0 : getter(obj);
            }
        }

        public class FloatNullableAccessor : AccessorTpl<float?>
        {
            public FloatNullableAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (float)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? null : getter(obj);
            }
        }

        public class GuidAccessor : AccessorTpl<Guid>
        {
            public GuidAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (Guid)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? Guid.Empty : getter(obj);
            }
        }

        public class GuidNullableAccessor : AccessorTpl<Guid?>
        {
            public GuidNullableAccessor(PropertyInfo prop)
                : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (Guid)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? null : getter(obj);
            }
        }

        public class ByteAccessor : AccessorTpl<byte>
        {
            public ByteAccessor(PropertyInfo prop) : base(prop)
            {
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
                return !CanGet ? 0 : getter(obj);
            }
        }
        public class ByteNullableAccessor : AccessorTpl<byte?>
        {
            public ByteNullableAccessor(PropertyInfo prop) : base(prop)
            {
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
                return !CanGet ? null : getter(obj);
            }
        }

        public class ShortAccessor : AccessorTpl<short>
        {
            public ShortAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (short)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? 0 : getter(obj);
            }
        }
        public class ShortNullableAccessor : AccessorTpl<short?>
        {
            public ShortNullableAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (short)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? 0 : getter(obj);
            }
        }

        public class CharAccessor : AccessorTpl<char>
        {
            public CharAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (char)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? char.MinValue : getter(obj);
            }
        }

        public class CharNullableAccessor : AccessorTpl<char?>
        {
            public CharNullableAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (char)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? null : getter(obj);
            }
        }

        public class BoolAccessor : AccessorTpl<bool>
        {
            public BoolAccessor(PropertyInfo prop) : base(prop)
            {
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
                        setter(obj, Convert.ToInt32(value) > 0);
                    }
                }
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? false : getter(obj);
            }
        }

        public class BoolNullableAccessor : AccessorTpl<bool?>
        {
            public BoolNullableAccessor(PropertyInfo prop) : base(prop)
            {
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
                return !CanGet ? null : getter(obj);
            }
        }

        public class TimeSpanAccessor : AccessorTpl<TimeSpan>
        {
            public TimeSpanAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (TimeSpan)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? TimeSpan.Zero : getter(obj);
            }
        }

        public class TimeSpanNullableAccessor : AccessorTpl<TimeSpan?>
        {
            public TimeSpanNullableAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (TimeSpan)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? null : getter(obj);
            }
        }

        public class DecimalAccessor : AccessorTpl<decimal>
        {
            public DecimalAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (decimal)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? 0M : getter(obj);
            }
        }

        public class DecimalNullableAccessor : AccessorTpl<decimal?>
        {
            public DecimalNullableAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (decimal)value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? null : getter(obj);
            }
        }

        public class ByteArrayAccessor : AccessorTpl<byte[]>
        {
            public ByteArrayAccessor(PropertyInfo prop) : base(prop)
            {
            }
            protected override void DoSet(TObject obj, object value)
            {
                setter?.Invoke(obj, (byte[])value);
            }
            protected override object DoGet(TObject obj)
            {
                return !CanGet ? null : getter(obj);
            }
        }

        #endregion


    }
}
