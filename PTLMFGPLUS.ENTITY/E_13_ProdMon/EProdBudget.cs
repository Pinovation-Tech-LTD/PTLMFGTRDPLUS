using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.ENTITY.E_13_ProdMon
{
    public class EProdBudget
    {
        public class ProductBudgetNo
        {
            public string bpno { get; set; }
        }
        public class PreviousBudget
        {
            public string comcod { get; set; }
            public string pbmno { get; set; }
            public string pbmno1 { get; set; }
            public string bactcode { get; set; }
            public string bgddat { get; set; }
            public string sdate { get; set; }
            public string edate { get; set; }
        }
        public class ProductList
        {
            public string comcod { get; set; }
            public string prodcode1 { get; set; }
            public string proddesc1 { get; set; }
            public string prodcode { get; set; }
            public string proddesc { get; set; }
            public string produnit { get; set; }
            public decimal stqty { get; set; }
            public decimal targetqty { get; set; }
            public decimal nproqty { get; set; }
            public string sirtdes { get; set; }
            public decimal stdqty { get; set; }
            public string rescode1 { get; set; }
            public string resdesc1 { get; set; }

        }

    }
}
