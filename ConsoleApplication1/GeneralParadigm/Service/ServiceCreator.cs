using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.GeneralParadigm.Service
{
    class ServiceCreator
    {
        public static T Create<T>() where T : new()
        {
            //可做其他逻辑
            return new T();
        }
    }
}
