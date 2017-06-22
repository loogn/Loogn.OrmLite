﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    static class DynamicMethodHelper
    {
        /// <summary>
        /// 实例化对象 用EMIT
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public static Func<object> BuildConstructorInvoker(ConstructorInfo constructor)
        {
            //动态方法
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString("N"), typeof(object), new[] { typeof(object[]) }, true);
            //方法IL
            ILGenerator il = dynamicMethod.GetILGenerator();
            //实例化命令
            il.Emit(OpCodes.Newobj, constructor);
            //如果是值类型装箱
            if (constructor.ReflectedType.IsValueType)
                il.Emit(OpCodes.Box, constructor.ReflectedType);
            //返回
            il.Emit(OpCodes.Ret);
            //用FUNC去关联方法
            return (Func<object>)dynamicMethod.CreateDelegate(typeof(Func<object>));
        }

        public static Func<object> BuildConstructorInvoker(Type type)
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);
            return BuildConstructorInvoker(constructor);
        }

        public static Func<object, object> BuildGetterInvoker(MethodInfo methodInfo)
        {
            if (methodInfo == null) return (obj) => { return null; };
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var instanceExpr = methodInfo.IsStatic ? null : Expression.Convert(instanceParameter, methodInfo.ReflectedType);
            var callExpr = Expression.Call(instanceExpr, methodInfo, null);
            UnaryExpression castCallExpr = Expression.Convert(callExpr, typeof(object));
            var fun = Expression.Lambda<Func<object, object>>(castCallExpr, instanceParameter).Compile();
            return fun;
        }

        public static Action<object, object> BuildSetterInvoker(MethodInfo methodInfo)
        {
            if (methodInfo == null) return (obj, value) => {; };

            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var parametersParameter = Expression.Parameter(typeof(object), "value");
            var instanceExpr = methodInfo.IsStatic ? null : Expression.Convert(instanceParameter, methodInfo.ReflectedType);
            var paramInfo = methodInfo.GetParameters().First();
            var arrCase = Expression.Convert(parametersParameter, paramInfo.ParameterType);
            var callExpr = Expression.Call(instanceExpr, methodInfo, arrCase);
            var action = Expression.Lambda<Action<object, object>>(callExpr,
                instanceParameter, parametersParameter).Compile();
            return action;
        }

    }
}