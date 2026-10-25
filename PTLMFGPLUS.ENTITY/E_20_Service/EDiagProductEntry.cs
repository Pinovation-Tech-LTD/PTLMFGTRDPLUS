using System.Collections.Generic;

namespace PTLMFGPLUS.ENTITY.E_20_Service
{
    public class EDiagProductEntry
    {
        public class LookupItem
        {
            public string code { get; set; } = "";
            public string description { get; set; } = "";
            public string unit { get; set; } = "";
            public string specificationCode { get; set; } = "";
            public string specification { get; set; } = "";
            public double value { get; set; }
            public double stockQuantity { get; set; }
        }
        public class ProblemLine { public string grp { get; set; } = "A"; public string gcod { get; set; } = ""; public string gdesc { get; set; } = ""; public string remarks { get; set; } = ""; }
        public class SparePart
        {
            public string rsircode { get; set; } = "";
            public string sirdesc { get; set; } = "";
            public string sirunit { get; set; } = "";
            public double qty { get; set; }
            public double sirval { get; set; }
            public double amount { get; set; }
            public string apstatus { get; set; } = "False";
            public double execqty { get; set; }
            public string spcfcod { get; set; } = "";
            public string spcfdesc { get; set; } = "";
            public double stkqty { get; set; }
        }
        public class Header
        {
            public string recvno { get; set; } = ""; public string centrid { get; set; } = ""; public string actdesc { get; set; } = "";
            public string mimei { get; set; } = ""; public string simei { get; set; } = ""; public string procode { get; set; } = "";
            public string custname { get; set; } = ""; public string custmob { get; set; } = ""; public string custmob2 { get; set; } = "";
            public string custmail { get; set; } = ""; public string custadd { get; set; } = ""; public string custop { get; set; } = "";
            public string hsmissing { get; set; } = ""; public string recvdate { get; set; } = ""; public string digndate { get; set; } = "";
            public string deldate { get; set; } = ""; public string wrcode { get; set; } = ""; public string rwrcode { get; set; } = "";
            public string wrstatus { get; set; } = ""; public string rimei { get; set; } = ""; public string rsirdesc { get; set; } = "";
            public string custstatus { get; set; } = ""; public string custhtmsg { get; set; } = "";
            public string dignbyid1 { get; set; } = ""; public string digntrmid1 { get; set; } = ""; public string dignseson1 { get; set; } = "";
        }
        public class ProductInfo
        {
            public string rescod { get; set; } = ""; public string proddesc { get; set; } = ""; public string actdesc { get; set; } = "";
            public string custname { get; set; } = ""; public string distadd { get; set; } = ""; public string sdeldate { get; set; } = ""; public string activedate { get; set; } = "";
        }
        public class LoadResponse
        {
            public string companyCode { get; set; } = "";
            public Header header { get; set; } = new();
            public ProductInfo product { get; set; } = new();
            public List<ProblemLine> receivedProblems { get; set; } = new();
            public List<ProblemLine> diagnosisProblems { get; set; } = new();
            public List<ProblemLine> accessories { get; set; } = new();
            public List<SparePart> spareParts { get; set; } = new();
            public List<SparePart> pendingParts { get; set; } = new();
            public List<LookupItem> problemOptions { get; set; } = new();
            public List<LookupItem> warrantyOptions { get; set; } = new();
            public List<LookupItem> spareOptions { get; set; } = new();
        }
        public class SaveRequest
        {
            public string type { get; set; } = "DiagEnrty"; public string centerId { get; set; } = ""; public string receiveNo { get; set; } = "";
            public string imei { get; set; } = ""; public string replacementImei { get; set; } = ""; public string diagnosisDate { get; set; } = "";
            public string receivedDate { get; set; } = ""; public string productCode { get; set; } = ""; public string warrantyCode { get; set; } = "";
            public string remarks { get; set; } = ""; public List<ProblemLine> problems { get; set; } = new(); public List<SparePart> spareParts { get; set; } = new();
        }
        public class WorkflowRequest
        {
            public string type { get; set; } = ""; public string centerId { get; set; } = ""; public string receiveNo { get; set; } = "";
            public string date { get; set; } = ""; public string diagnosisDate { get; set; } = ""; public string imei { get; set; } = "";
            public string replacementImei { get; set; } = ""; public List<SparePart> spareParts { get; set; } = new();
        }
    }
}
