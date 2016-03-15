using ConsoleApplication1.GeneralParadigm.Model;
using Loogn.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.GeneralParadigm.Service
{
    public class PersonService
    {
        public static PersonService GetInstance()
        {
            return ServiceCreator.Create<PersonService>();
        }
        OrmLiteOperator<Person> oo = new OrmLiteOperator<Person>(DB.Open);

        public int Insert(Person m)
        {
            var flag = oo.Insert(m);
            return flag;
        }

        public Person SingleById(int id)
        {
            var person = oo.SingleById(id);
            return person;
        }

        /*
        ......
        */
    }
}
