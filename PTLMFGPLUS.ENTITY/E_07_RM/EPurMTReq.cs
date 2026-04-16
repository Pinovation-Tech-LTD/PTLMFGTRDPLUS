using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.ENTITY.E_07_RM
{
    public class EPurMTReq
    {
        public class PreviousOrder
        {
            public string mtreqno { get; set; }
            public string mtreqno1 { get; set; }
            public string mtrdat { get; set; }

        }
        public class MatTrnsNo
        {
            public string maxmtrno { get; set; }
            public string maxtrnno1 { get; set; }
            public string maxmtrdt { get; set; }

        }

        public class ProjectFromList
        {
            public string comcod { get; set; }
            public string actcode { get; set; }
            public string actdesc1 { get; set; }
        }
        public class ProjectResourseList
        {
            public string comcod { get; set; }
            public string pactcode { get; set; }
            public string rsircode { get; set; }
            public string spcfcod { get; set; }
            public string resdesc { get; set; }
            public string sirdesc { get; set; }
            public string sirunit { get; set; }
            public decimal qty { get; set; }
            public decimal rate { get; set; }
            public decimal balqty { get; set; }
            public decimal amt { get; set; }
            public string spcfdesc { get; set; }

        }
        public class ProjectResourseList1
        {
            public string mspcfcod { get; set; }
            public string spcfcod { get; set; }
            public string spcfdesc { get; set; }
        }
        public class SelectedItemListSave
        {
            public string rsircode { get; set; }
            public string spcfcod { get; set; }
            public string resdesc { get; set; }
            public string sirunit { get; set; }
            public double balqty { get; set; }
            public double qty { get; set; }
            public double rate { get; set; }
            public double amt { get; set; }
        } 
        public class GetMatTransInfo
        {
            public string comcod { get; set; }
            public string mtreqno { get; set; }
            public string rsircode { get; set; }
            public string spcfcod { get; set; }
            public string resdesc { get; set; }
            public string sirunit { get; set; }
            public double balqty { get; set; }
            public double qty { get; set; }
            public double rate { get; set; }
            public double amt { get; set; }
        }


    }
}
