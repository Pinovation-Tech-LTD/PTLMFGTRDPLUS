using System.Collections.Generic;

namespace PTLMFGPLUS.ENTITY.E_07_Inv
{
    public class EPBMatIssueSingle
    {
        public class Store { public string actcode { get; set; } public string actdesc { get; set; } public string acttype { get; set; } }
        public class PreviousIssue { public string misuno { get; set; } public string misuno1 { get; set; } }
        public class IssueNumber { public string maxisuno { get; set; } public string maxisuno1 { get; set; } }
        public class RequisitionOption { public string preqno { get; set; } public string preqno1 { get; set; } }
        public class IssueHeader
        {
            public string misuno { get; set; } public string misuno1 { get; set; } public string misudate { get; set; }
            public string actcode { get; set; } public string rmrks { get; set; } public string misuref { get; set; }
            public string postedbyid { get; set; } public string postrmid { get; set; } public string postseson { get; set; }
            public string entrydat { get; set; } public string editbyid { get; set; } public string editdat { get; set; }
        }
        public class MaterialIssueRow
        {
            public string preqno { get; set; } public string preqno1 { get; set; }
            public string bactcode { get; set; } public string bactdesc { get; set; }
            public string procode { get; set; } public string prodesc { get; set; }
            public string rsircode { get; set; } public string rsirdesc { get; set; }
            public string spcfcod { get; set; } public string spcfdesc { get; set; }
            public string rsirunit { get; set; } public string rsircode1 { get; set; } public string rsirdesc1 { get; set; }
            public decimal balqty { get; set; } public decimal isuqty { get; set; } public decimal isurate { get; set; }
            public decimal stockqty { get; set; } public decimal balstkqty { get; set; } public decimal reqqty { get; set; }
        }
        public class RequisitionItem
        {
            public string preqno { get; set; }
            public string procode { get; set; } 
            public string rsircode { get; set; }
            public string rsirdesc { get; set; } 
            public string rsirunit { get; set; }
            public decimal rsqty { get; set; } 
            public decimal issuecomp { get; set; } 
            public decimal balisu { get; set; }
        }
        public class IssueOpenResponse
        {
            public string issueNo { get; set; }
            public IssueHeader header { get; set; }
            public IEnumerable<MaterialIssueRow> issueRows { get; set; } = new List<MaterialIssueRow>();
            public IEnumerable<MaterialIssueRow> availableMaterials { get; set; } = new List<MaterialIssueRow>();
            public IEnumerable<RequisitionOption> requisitions { get; set; } = new List<RequisitionOption>();
            public IEnumerable<RequisitionItem> requisitionItems { get; set; } = new List<RequisitionItem>();
        }
        public class ReloadMaterialsRequest
        {
            public string storeCode { get; set; } public string date { get; set; } public string dprNo { get; set; }
            public List<RequisitionItem> requisitionItems { get; set; } = new();
        }
        public class SaveIssueRequest
        {
            public string type { get; set; } public string date { get; set; } public string issueNo { get; set; }
            public string storeCode { get; set; } public string referenceNo { get; set; } public string remarks { get; set; }
            public List<MaterialIssueRow> issueRows { get; set; } = new();
            public List<RequisitionItem> requisitionItems { get; set; } = new();
            public IssueHeader originalHeader { get; set; }
        }
    }
}
