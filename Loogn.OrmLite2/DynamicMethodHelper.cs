using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite2
{
    static class DynamicMethodHelper
    {
        /// <summary>
        /// 编译Task<TResult>的调用委托
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        public static Func<object, Task<object>> BuildContinueTaskInvoker(Type taskType)
        {
            var resultMethod = BuildMethodInvoker(taskType.GetProperty("Result").GetGetMethod(true));
            return new Func<object, Task<object>>(obj =>
            {
                var task = (Task)obj;
                task.Start();
                return task.ContinueWith(t =>
                {
                    //异常？
                    var result = resultMethod(t, null);
                    return result;
                });
            });
        }

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


        public static Func<T> BuildConstructorInvoker<T>() where T : class, new()
        {
            var type = typeof(T);
            //动态方法
            var dynamicMethod = new DynamicMethod(Guid.NewGuid().ToString("N"), type, new[] { typeof(object[]) }, true);
            //方法IL
            ILGenerator il = dynamicMethod.GetILGenerator();
            //实例化命令
            il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            //返回
            il.Emit(OpCodes.Ret);
            //用FUNC去关联方法
            return (Func<T>)dynamicMethod.CreateDelegate(typeof(Func<T>));
        }

        /// <summary>
        /// 动态编译成委托
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public static Func<object, object[], object> BuildMethodInvoker(MethodInfo methodInfo)
        {
            var instanceParameter = Expression.Parameter(typeof(object), "instance");
            var parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            var instanceExpr = methodInfo.IsStatic ? null : Expression.Convert(instanceParameter, methodInfo.ReflectedType);

            List<Expression> parameterExpressions = new List<Expression>();
            var paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                var arrItem = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                var arrCase = Expression.Convert(arrItem, paramInfos[i].ParameterType);
                parameterExpressions.Add(arrCase);
            }
            var callExpr = Expression.Call(instanceExpr, methodInfo, parameterExpressions);

            if (methodInfo.ReturnType == typeof(void))
            {
                var action = Expression.Lambda<Action<object, object[]>>(callExpr,
                    instanceParameter, parametersParameter).Compile();
                return (instance, parameters) =>
                {
                    action(instance, parameters);
                    return null;
                };
            }
            else
            {
                UnaryExpression castCallExpr = Expression.Convert(callExpr, typeof(object));
                var fun = Expression.Lambda<Func<object, object[], object>>(castCallExpr, instanceParameter, parametersParameter).Compile();
                return fun;
            }
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
