using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.ENTITY.E_07_RM
{
    public class EMaterialsTranser
    {
        public class LastTransNo()
        {
            public string maxtrnno { get; set; }
            public string maxtrnno1 { get; set; }
            public string maxtrndt { get;set; }
        }
        public class ProjectFromList()
        {

            public string actcode { get; set; }
            public string actdesc1 { get; set; }
            public string actdesc { get; set; }
            public string acttdesc { get; set; }
            public string actelev { get; set; }
        }
        public class MtrReqPassList()
        {
            public string getpno { get; set; }
            public string getpno1 { get; set; }
            public string getpdat { get;set; }
            public string getpref { get; set; }
            public string mtreqno { get; set; }
            public string mtreqno1 { get;set; }
            public string rsircode { get; set; }
            public string spcfcod { get; set; }
            public decimal getpqty { get; set; }
            public decimal getpamt { get; set; }
            public decimal rate { get; set; }
            public decimal mtrfqty { get; set; }
            public string mtrref { get; set; }
            public string tfpactcode { get; set; }
            public string ttpactcode { get; set; }
            public string rsirdesc { get; set; }
            public string rsirunit { get; set; }
            public string spcfdesc { get; set; }
            public string textfield { get; set; }
            public string valuefiled { get;set; }
            public string tfpactdesc { get; set; }
            public string ttpactdesc { get; set; }

        }
    }
}
