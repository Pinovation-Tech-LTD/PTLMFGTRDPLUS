using System.Collections.Generic;

namespace PTLMFGPLUS.ENTITY.E_20_Service
{
    public class EAllRecProduct
    {
        public class LookupItem
        {
            public string code { get; set; } = "";
            public string description { get; set; } = "";
        }

        public class ReceiveRecord
        {
            public string recvno { get; set; } = "";
            public string recvno1 { get; set; } = "";
            public string centrid { get; set; } = "";
            public string actdesc { get; set; } = "";
            public string mimei { get; set; } = "";
            public string simei { get; set; } = "";
            public string recvdate { get; set; } = "";
            public string custname { get; set; } = "";
            public string custmob { get; set; } = "";
            public string prodesc { get; set; } = "";
            public string wrstatusdesc { get; set; } = "";
            public string probdetails { get; set; } = "";
            public string deldate { get; set; } = "";
            public string trnstatus { get; set; } = "";
            public string empid { get; set; } = "";
            public string empname { get; set; } = "";
            public string bcname { get; set; } = "";
            public string bcnno { get; set; } = "";
            public string bcdate { get; set; } = "";
            public string custstatus { get; set; } = "";
            public string cutstdesc { get; set; } = "";
            public string custhtmsg { get; set; } = "";
        }

        public class InitialResponse
        {
            public string fromDate { get; set; } = "";
            public string toDate { get; set; } = "";
            public List<LookupItem> serviceCenters { get; set; } = new();
        }

        public class UpdateRequest
        {
            public string centerId { get; set; } = "";
            public string receiveNo { get; set; } = "";
            public string employeeId { get; set; } = "";
            public string hotlineMessage { get; set; } = "";
            public bool transferred { get; set; }
        }
    }
}
