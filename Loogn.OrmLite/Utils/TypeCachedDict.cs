using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Loogn.OrmLite
{
    static class TypeCachedDict
    {
        static Dictionary<Type, TypeCachedInfo> Dict = new Dictionary<Type, TypeCachedInfo>();

        public static TypeCachedInfo GetTypeCachedInfo(Type type)
        {
            TypeCachedInfo info;
            if (Dict.TryGetValue(type, out info))
            {
                return info;
            }
            else
            {
                info = new TypeCachedInfo(type);
                Dict[type] = info;
                return info;
            }
        }
        
        static Dictionary<Type, object> GDict = new Dictionary<Type, object>(50);
        public static TypeCachedInfo<TObject> GetTypeCachedInfo<TObject>()
        {
            var type = typeof(TObject);
            object info;
            if (GDict.TryGetValue(type, out info))
            {
                return (TypeCachedInfo<TObject>)info;
            }
            else
            {
                var refInfo = new TypeCachedInfo<TObject>(type);
                GDict[type] = refInfo;
                return refInfo;
            }
        }

    }
}
