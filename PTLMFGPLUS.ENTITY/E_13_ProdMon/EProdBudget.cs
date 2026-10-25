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
            public string rescod1 { get; set; }
            public string resdesc1 { get; set; }

        }
        public class SelectedItemListProductionBudget
        {
            public string scode { get; set; }
            public string prodcode1 { get; set; }
            public string proddesc1 { get; set; }
            public string prodcode { get; set; }
            public string proddesc { get; set; }
            public string produnit { get; set; }
            public decimal targetqty { get; set; }
            public decimal stqty { get; set; }
            public decimal nproqty { get; set; }
            public decimal bgdwqty { get; set; }
        }
        public class ShowProductBudgetEntity
        {
            public string comcod { get; set; }
            public string pbmno { get; set; }
            public string bactcode { get; set; }
            public string bactdesc { get; set; }
            public string prodcode1 { get; set; }
            public string proddesc1 { get; set; }
            public string prodcode { get; set; }
            public string proddesc { get; set; }
            public string produnit { get; set; }
            public string scode { get; set; }
            public decimal stqty { get; set; }
            public decimal targetqty { get; set; }
            public decimal nproqty { get; set; }
            public decimal bgdwqty { get; set; }
        }
        public class ShowProductBudgetLogEntity
        {
            public string comcod { get; set; }
            public string pbmno { get; set; }
            public string bgddat { get; set; }
            public string postedbyid { get; set; }
            public string postrmid { get; set; }
            public string postseson { get; set; }
            public string posteddat { get; set; }
            public string editbyid { get; set; }
            public string editdat { get; set; }
            public string sdate { get; set; }
            public string edate { get; set; }
            public string imei { get; set; }
        }

        public class BudgetResource
        {
            public string comcod { get; set; }
            public string scode { get; set; }
            public string odcode { get; set; }
            public string oddesc { get; set; }
            public string rsircode { get; set; }
            public string rsirdesc { get; set; }
            public string spcfcod { get; set; }
            public string spcfdesc { get; set; }
            public string rsirunit { get; set; }
            public decimal stockqty { get; set; }
            public decimal bgdqty { get; set; }
            public decimal bgdrat { get; set; }
            public decimal bgdam { get; set; }
            public decimal ratio { get; set; }
            public bool selected { get; set; }
        }

        public class BudgetProcess
        {
            public string comcod { get; set; }
            public string processcode { get; set; }
            public string processdesc { get; set; }
            public bool active { get; set; }
        }

        public class BudgetReportProduct
        {
            public string prodcode { get; set; }
            public string proddesc { get; set; }
        }

        public class BudgetReportRow
        {
            public string comcod { get; set; }
            public string grp { get; set; }
            public string grpdesc { get; set; }
            public string procode { get; set; }
            public string prodesc { get; set; }
            public string rsircode { get; set; }
            public string rsirdesc { get; set; }
            public string spcfdesc { get; set; }
            public string rsirunit { get; set; }
            public decimal bgdqty { get; set; }
            public decimal bgdrate { get; set; }
            public decimal bgdam { get; set; }
            public decimal percnt { get; set; }
        }

        public class SaveProductBudgetRequest
        {
            public string type { get; set; }
            public string date { get; set; }
            public string pbnno { get; set; }
            public string sDate { get; set; }
            public string tDate { get; set; }
            public string batchName { get; set; }
            public List<SelectedItemListProductionBudget> items { get; set; } = new();
            public List<ShowProductBudgetLogEntity> log { get; set; } = new();
        }

        public class SaveBudgetResourcesRequest
        {
            public string batchNo { get; set; }
            public List<BudgetResource> resources { get; set; } = new();
            public List<BudgetProcess> processes { get; set; } = new();
        }

        public class ImeiUploadRow
        {
            public string SLNUM { get; set; }
            public string IMEI1 { get; set; }
            public string IMEI2 { get; set; }
            public string IMEI3 { get; set; }
            public string IMEI4 { get; set; }
        }

        public class EngineCodeRow
        {
            public string resdesc { get; set; }
            public string chassis { get; set; }
            public string engine { get; set; }
            public string slnum { get; set; }
            public decimal items { get; set; }
        }

        public class AssignEngineCodesRequest
        {
            public string batchNo { get; set; }
            public List<EngineCodeRow> items { get; set; } = new();
        }

    }
}
