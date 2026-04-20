using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.ENTITY.E_07_RM
{
    public class ERawMattInterface
    {
        public class InterfaceData()
        {
            public string comcod { get; set; }
            public string mtreqno { get; set; }
            public string mtrdat { get; set; }
            public string mtrref { get; set; }
            public string tfpactcode { get; set; }
            public string tfpactdesc { get; set; }
            public string ttpactcode { get; set; }
            public string ttpactdesc { get; set; }
            public string mtrnar {  get; set; }
            public string itmocunt {  get; set; }
            public decimal tqty { get; set; }
            public decimal tamt { get; set; }
            public string posteddat { get; set; }
            public string postedbyid { get; set; }
            public string postedusr { get; set; }
            public string approved { get; set; }
            public string getpno { get; set; }  
            public decimal gatpqty { get; set; }
            public decimal gatpamt  { get; set; }
            public decimal gatpbal {  get; set; }
        }
        public class InterfaceDataWithTrans()
        {
            public string comcod { get; set; }
            public string mtreqno { get; set; }
            public string mtrdat { get; set; }
            public string mtrref { get; set; }
            public string tfpactcode { get; set; }
            public string tfpactdesc { get; set; }
            public string ttpactcode { get; set; }
            public string ttpactdesc { get; set; }
            public string mtrnar { get; set; }
            public string itmocunt { get; set; }
            public decimal tqty { get; set; }
            public decimal tamt { get; set; }
            public string posteddat { get; set; }
            public string postedbyid { get; set; }
            public string postedusr { get; set; }
            public string approved { get; set; }
            public string getpno { get; set; }
            public decimal gatpqty { get; set; }
            public decimal gatpamt { get; set; }
            public decimal gatpbal { get; set; }
            public string trnno {  get; set; }
            public decimal trnqty { get; set; }
            public decimal trnamt { get; set; }
            public string vounum { get; set; }
            public decimal trnbal { get; set; }
            public string trndat { get; set; }
        }
        public class NumberOfQuantiy()
        {
            public decimal reqqty { get; set; }
            public decimal reqaqty { get; set; }
            public decimal gpqty { get; set; }
            public decimal trnsqty { get; set; }
            public decimal trnsappqty { get; set; }
        }
    }

}
