using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.ENTITY.E_07_RM
{
    public class EPurMTReqGatePass
    {
        public class GetLastGetPNO
        {
            public string maxno {  get; set; }
            public string maxno1 { get; set; }
            public string maxmtrdt { get; set; }
        }
        public class GetMTReqList
        {
            public string comcod { get; set; }
            public string mtreqno { get; set; }
            public string mtreqno1 { get; set; }
            public string mtrref { get; set; }
            public string mtrdat { get; set; }
            public string tfpactcode { get; set; }
            public string ttpactcode { get; set; }
            public string rsircode { get; set; }
            public string spcfcod { get; set; }
            public decimal mtrfqty { get; set; }
            public decimal balqty { get; set; }
            public decimal stockqty { get; set; }
            public decimal mtrfrat { get; set; }
            public decimal mtrfamt { get; set; }
            public string tfpactdesc { get; set; }
            public string ttpactdesc { get; set; }
            public string rsirdesc { get; set; }
            public string rsirunit { get; set; }
            public string spcfdesc { get; set; }
            public string textfield { get; set; }
            public string valuefiled { get; set; }

        }
        public class GetMtreqResource
        {
            public string comcod { get; set; }
            public string mtreqno { get; set; }
            public string rsircode { get; set; }    
            public string rsirdesc { get; set; }

        }
        public class GetMTReqResList
        {
            public string mtreqno { get; set; } 
            public string valuefiled { get; set; }
            public string textfield { get; set; }
        }
        public class SaveIssueDataList
        {
            public decimal balqty { get; set; }
            public decimal approveqty { get; set; }
            public string comcod { get; set; }
            public string mtrdat { get; set; }
            public string mtreqno { get; set; }
            public string mtreqno1 { get; set; }
            public decimal mtrfamt { get; set; }
            public decimal mtrfqty { get; set; }
            public decimal mtrfrat { get; set; }
            public string mtrref { get; set; }
            public string rsircode { get; set; }
            public string rsirdesc { get; set; }
            public string rsirunit { get; set; }
            public string spcfcod { get; set; }
            public string spcfdesc { get; set; }
            public decimal stockqty { get; set; }
            public string textfield { get; set; }
            public string tfpactcode {get;set;}
            public string tfpactdesc { get; set; }
            public string ttpactcode { get;set; }
            public string ttpactdesc { get; set; }
            public string valuefiled { get;set; }
            public double getpqty { get; set; }
            public double getpamt { get; set; }           


        }

        public class ApiResponse<T>
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }
    }
}
