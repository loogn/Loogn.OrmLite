using Loogn.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1.GeneralParadigm.Model
{
    [OrmLiteTable("person_11")]
    public class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Sex { get; set; }
        public byte Age { get; set; }
        public double Money { get; set; }
        public DateTime AddDate { get; set; }

    }
}
