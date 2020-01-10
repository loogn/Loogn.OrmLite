using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Loogn.OrmLite.Utils
{
    public class ReaderToEntityDict
    {
        static ConcurrentDictionary<string, Delegate> handlers = new ConcurrentDictionary<string, Delegate>();

        /// <summary>
        /// 性能并没有优化多少，所以不用这个
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static Func<object[], T> GetMapHandle<T>(IDataReader reader)
        {
            var type = typeof(T);
            List<string> fields = new List<string>(reader.FieldCount);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                fields.Add(reader.GetName(i).ToUpperInvariant());
            }
            var key = string.Join("", fields) + type.FullName;

            var handler = handlers.GetOrAdd(key, (theKey) =>
             {
                 var readerExp = Expression.Parameter(typeof(object[]), "values");
                
                 List<Expression> setList = new List<Expression>();

                 var varExp = Expression.Variable(type, "obj");
                 var labelTarget = Expression.Label(type);

                 var varAssgExp = Expression.Assign(varExp, Expression.New(type));

                 setList.Add(varExp);
                 
                 setList.Add(varAssgExp);
                 foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                 {
                     var index = fields.IndexOf(property.Name.ToUpperInvariant());
                     if (index > 0)
                     {
                         var indexExp = Expression.Constant(index, typeof(int));
                         var propExp = Expression.Property(varExp, property);
                         var getValueExp =  Expression.ArrayIndex(readerExp,indexExp);
                         
                         var setExp = Expression.Assign(propExp,
                              Expression.Convert(getValueExp, property.PropertyType));

                         var ifSetExp = Expression.IfThen(Expression.NotEqual(getValueExp, Expression.Constant(null)), setExp);
                         setList.Add(ifSetExp);
                     }
                 }
                 //
                 var ret = Expression.Return(labelTarget, varExp);
                 //创建返回表达式的目标Label
                 LabelExpression lbl = Expression.Label(labelTarget, Expression.Default(type));
                 setList.Add(ret);
                 setList.Add(lbl);

                 var bodyExp = Expression.Block(new ParameterExpression[] { varExp }, setList);
                 var setAction = Expression.Lambda<Func<object[], T>>(bodyExp, readerExp).Compile();

                 return setAction;

                 /*
                 (values) =>
                     {
                         var obj=new T();
                         if(values[0]!=null){
                             obj.id=Convert(values[0]);
                         }
                         return obj;
                     }
                 */

             });

            return (Func<object[], T>)handler;

        }

    }
}
