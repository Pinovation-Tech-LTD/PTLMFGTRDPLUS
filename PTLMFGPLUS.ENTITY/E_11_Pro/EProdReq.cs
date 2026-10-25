using System.Collections.Generic;

namespace PTLMFGPLUS.ENTITY.E_11_Pro
{
    public class EProdReq
    {
        public class ProductionBudget
        {
            public string pbmno { get; set; }
            public string pbmno1 { get; set; }
        }

        public class BatchCode
        {
            public string actcode { get; set; }
            public string actdesc { get; set; }
        }

        public class DprNumber
        {
            public string dprno { get; set; }
        }

        public class PreviousDpr
        {
            public string preqno { get; set; }
            public string preqno1 { get; set; }
            public string pbmno { get; set; }
            public string pbmno1 { get; set; }
            public string actcode { get; set; }
            public string actdesc { get; set; }
            public string pbdate { get; set; }
        }

        public class RequisitionProduct
        {
            public string comcod { get; set; }
            public string grp { get; set; }
            public string grpdesc { get; set; }
            public string procode { get; set; }
            public string prodesc { get; set; }
            public string rsircode { get; set; }
            public string rsirdesc { get; set; }
            public string rsirunit { get; set; }
            public string spcfcod { get; set; }
            public decimal bgdqty { get; set; }
            public decimal bgdam { get; set; }
            public decimal bgdrate { get; set; }
            public decimal issqty { get; set; }
            public decimal bbqty { get; set; }
            public decimal cresqty { get; set; }
        }

        public class RequisitionMaterial : RequisitionProduct
        {
            public string spcfdesc { get; set; }
            public decimal uresqty { get; set; }
            public decimal stockqty { get; set; }
            public decimal balqty { get; set; }
            public decimal fresqty { get; set; }
        }

        public class CompanyProductionSettings
        {
            public bool useprdprc { get; set; }
            public bool useprdprcsemi { get; set; }
        }

        public class ProductionStartRow
        {
            public string batchcode { get; set; }
            public decimal fprocscode { get; set; }
            public string tprocscode { get; set; }
            public string pbdate { get; set; }
            public string rsircode { get; set; }
            public decimal rsqty { get; set; }
        }

        public class LastProductionProcessNumber
        {
            public string ppnno { get; set; }
        }

        public class RequisitionDataResponse
        {
            public IEnumerable<RequisitionProduct> products { get; set; } = new List<RequisitionProduct>();
            public IEnumerable<RequisitionMaterial> materials { get; set; } = new List<RequisitionMaterial>();
        }

        public class SaveProductionRequisitionRequest
        {
            public string type { get; set; }
            public string date { get; set; }
            public string dprNo { get; set; }
            public string budgetNo { get; set; }
            public string batchCode { get; set; }
            public List<RequisitionProduct> products { get; set; } = new();
            public List<RequisitionMaterial> materials { get; set; } = new();
        }

        public class GenerateMaterialsRequest
        {
            public string type { get; set; }
            public string budgetNo { get; set; }
            public List<RequisitionProduct> products { get; set; } = new();
        }
    }
}
