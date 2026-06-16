using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.ENTITY.E_13_ProdMon
{
    public class EProductionInterface
    {
        public class InterfaceProductInfoData()
        {
            public string comcod { get; set; }
            public string pbmno { get; set; }
            public string pbmno1 { get; set; }
            public string prodcode { get; set; }
            public string prodesc { get; set; }
            public decimal bgdwqty { get; set; }
            public decimal bgdamt { get; set; }
            public string bgddat { get; set; }
            public string itemcount { get; set; }
            public decimal bgdbal { get; set; }
            public string pbmststus { get; set; }
            public string sdate { get; set; }
            public string enddate { get; set; }
            public string actcode { get; set; }
            public string actdesc { get; set; }
        }
        public class InterfaceProductProIssue()
        {
            public string comcod { get; set; }
            public string pbmno { get; set; }
            public string pbmno1 { get; set; }
            public string preqno { get; set; }
            public string preqno1 { get; set; }            
            public string bactcode { get; set; }
            public string batchdesc { get; set; }
            public string rcount { get; set; }           
            public string pbdate { get; set; }
            public string isstatus { get; set; }
            public string trescount { get; set; }
            public string prstatus { get; set; }
            public decimal fgreqqty { get; set; }
        }
        public class InterfaceProductProdEntry()
        {
            public string comcod { get; set; }
            public string pbmno { get; set; }
            public string pbmno1 { get; set; }               
            public string batchcode { get; set; }
            public string batchdesc { get; set; }
            public string prodcode { get; set; }
            public string prodesc { get; set; }
            public decimal acqty { get; set; }           
            public decimal balqty { get; set; }
            public decimal proamt { get; set; }
            public string pbdate { get; set; }
            public string itemcount { get; set; }
            public string proatatus { get; set; }
        }
        public class InterfaceProductQcEntry()
        {
            public string comcod { get; set; }
            public string prodid { get; set; }
            public string prodid1 { get; set; }
            public string pbmno { get; set; }
            public string pbmno1 { get; set; }               
            public string bactcode { get; set; }
            public string batchdesc { get; set; }
            public string prodcode { get; set; }
            public string prodesc { get; set; }                      
            public decimal balqty { get; set; }
            public decimal proamt { get; set; }
            public string prodate { get; set; }
            public string itemcount { get; set; }
            public string qcststus { get; set; }
            public string pstatus { get; set; }
        }
        public class InterfaceProductFlorRcv()
        {
            public string comcod { get; set; }
            public string prodid { get; set; }
            public string production { get; set; }
            public string grrno { get; set; }            
            public string batchdesc { get; set; }
                                 
            public decimal proqty { get; set; }
            public decimal proamt { get; set; }
            public string itemcount { get; set; }
            public string prodate { get; set; }
            public string rcvtype { get; set; }
            public string qcapprove { get; set; }
           
        }
        public class InterfaceProductInterNo()
        {
            public string reqqty { get; set; }
            public string proreqqty { get; set; }
            public string issueqty { get; set; }
            public string prodqty { get; set; }            
            public string qcqty { get; set; }
            public string strecqty { get; set; }
            public string compqty { get; set; }
            public string procsqty { get; set; }
            public string florcv { get; set; }
            public string isuapqty { get; set; }
            public string rework { get; set; }
                                 
            
           
        }
        public class InterfaceProductprodprocess()
        {
            public string comcod { get; set; }
            public string pbno { get; set; }
            public string batchcode { get; set; }             
            public string batchdesc { get; set; }
            public string rsircode { get; set; }
            public string rsirdesc { get; set; }

            public decimal acqty { get; set; }
            public decimal balqty { get; set; }
            public decimal prorate { get; set; }
            public decimal proamt { get; set; }
            public string pbdate { get; set; }
            public string itemcount { get; set; }
            public string preqno { get; set; }
            public string processbal { get; set; }
            public string pstatus { get; set; }
           
        }
        public class InterfaceProductIssueApproval()
        {
            public string comcod { get; set; }
            public string misuno { get; set; }
            public string misuno1 { get; set; }
            public string misudate { get; set; }             
            public string preqno { get; set; }
            public string preqno1 { get; set; }
            public string actcode { get; set; }
            public string actdesc { get; set; }
            public string bactcode { get; set; }
            public string batchdesc { get; set; }

            public decimal isuqty { get; set; }
            public string approve { get; set; }
            public decimal fgisuqty { get; set; }
            public decimal fgreqqty { get; set; }            
           
        }
        public class InterfaceProductReWork()
        {
            public string comcod { get; set; }
            public string preqno { get; set; }
            public decimal itemqty { get; set; }
            public string reworkdone { get; set; }
            public decimal balqty { get; set; }
            public string batchcode { get; set; }
            public string batchdesc { get; set; }
            public string rpbdate { get; set; }
                    
           
        }
    }
}
