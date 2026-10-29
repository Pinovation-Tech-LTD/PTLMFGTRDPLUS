using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTL.PRINTRDLC.Entity.ER_13_ProdMon
{
    public class BO_Production
    {
        [Serializable]
        public class RptProBatchBudget
        {
            public string comcod { get; set; }
            public string grp { get; set; }
            public string grpdesc { get; set; }
            public string procode { get; set; }
            public string prodesc { get; set; }
            public string rsircode { get; set; }
            public string rsirdesc { get; set; }
            public string rsirunit { get; set; }
            public double bgdqty { get; set; }
            public double bgdam { get; set; }
            public double bgdrate { get; set; }
            public double issqty { get; set; }
            public double bbqty { get; set; }
            public double cresqty { get; set; }
            public RptProBatchBudget() { }


        }
    }
}
