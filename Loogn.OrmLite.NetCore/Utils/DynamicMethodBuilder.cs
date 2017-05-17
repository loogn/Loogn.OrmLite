using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Loogn.OrmLite.NetCore.Utils
{
    public static class DynamicMethodBuilder
    {
        public static Func<T> BuildNewInvoker<T>()
        {
            return Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
        }
        
        public static Action<TObject, TValue> BuildSetterInvoker<TObject, TValue>(PropertyInfo propInfo)
        {
            var methodInfo = propInfo.GetSetMethod(true);
            if (methodInfo == null) return null;

            var instanceExpr = Expression.Parameter(typeof(TObject), "instance");
            var parametersExpr = Expression.Parameter(typeof(TValue), "value");
            var objExpr = methodInfo.IsStatic ? null : instanceExpr;
            var callExpr = Expression.Call(objExpr, methodInfo, parametersExpr);
            var setterINvoker = Expression.Lambda<Action<TObject, TValue>>(callExpr, instanceExpr, parametersExpr).Compile();
            return setterINvoker;
        }

        public static Func<TObject, TResult> BuildGetterInvoker<TObject, TResult>(PropertyInfo propInfo)
        {
            var methodInfo = propInfo.GetGetMethod(true);
            if (methodInfo == null) return null;
            var instanceExpr = Expression.Parameter(typeof(TObject), "instance");
            var objExpr = methodInfo.IsStatic ? null : instanceExpr;
            var callExpr = Expression.Call(objExpr, methodInfo);
            var setterINvoker = Expression.Lambda<Func<TObject, TResult>>(callExpr, instanceExpr).Compile();
            return setterINvoker;
        }

    }
}
