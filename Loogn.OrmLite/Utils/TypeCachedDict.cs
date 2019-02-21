﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    static class TypeCachedDict
    {
        static ConcurrentDictionary<Type, TypeCachedInfo> Dict = new ConcurrentDictionary<Type, TypeCachedInfo>();

        public static TypeCachedInfo GetTypeCachedInfo(Type type)
        {
            return Dict.GetOrAdd(type, (key) => new TypeCachedInfo(type));
        }

        static ConcurrentDictionary<Type, object> GDict = new ConcurrentDictionary<Type, object>(2, 50);
        public static TypeCachedInfo<TObject> GetTypeCachedInfo<TObject>()
        {
            var type = typeof(TObject);
            return (TypeCachedInfo<TObject>)GDict.GetOrAdd(type, (key) => new TypeCachedInfo<TObject>(key));

        }

    }
}
