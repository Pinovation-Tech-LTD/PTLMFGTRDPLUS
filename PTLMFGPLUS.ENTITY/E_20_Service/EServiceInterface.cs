using System.Collections.Generic;

namespace PTLMFGPLUS.ENTITY.E_20_Service
{
    public class EServiceInterface
    {
        public class ServiceInterfaceInitialResponse
        {
            public string defaultFromDate { get; set; } = string.Empty;
            public string defaultToDate { get; set; } = string.Empty;
            public List<ServiceCenterOption> serviceCenters { get; set; } = new();
            public List<ServiceTypeOption> serviceTypes { get; set; } = new();
        }

        public class ServiceInterfaceResponse
        {
            public List<ServiceInterfaceRecord> records { get; set; } = new();
            public ServiceInterfaceSummary summary { get; set; } = new();
        }

        public class ServiceCenterOption
        {
            public string actcode { get; set; } = string.Empty;
            public string actdesc { get; set; } = string.Empty;
        }

        public class ServiceTypeOption
        {
            public string gcod { get; set; } = string.Empty;
            public string gdesc { get; set; } = string.Empty;
            public decimal gval { get; set; }
        }

        public class ServiceInterfaceSummary
        {
            public string empid { get; set; } = string.Empty;
            public decimal recv { get; set; }
            public decimal trnsqty { get; set; }
            public decimal engasig { get; set; }
            public decimal engdig { get; set; }
            public decimal hotline { get; set; }
            public decimal approved { get; set; }
            public decimal doadap { get; set; }
            public decimal issue { get; set; }
            public decimal jobinpro { get; set; }
            public decimal qc { get; set; }
            public decimal bill { get; set; }
            public decimal delqty { get; set; }
        }

        public class ServiceInterfaceRecord
        {
            public string centrid { get; set; } = string.Empty;
            public string actdesc { get; set; } = string.Empty;
            public string recvdate { get; set; } = string.Empty;
            public string recvno { get; set; } = string.Empty;
            public string recvno1 { get; set; } = string.Empty;
            public string wrstatus { get; set; } = string.Empty;
            public string wrstatusdesc { get; set; } = string.Empty;
            public string rwrstatusdesc { get; set; } = string.Empty;
            public string bcname { get; set; } = string.Empty;
            public string bcnno { get; set; } = string.Empty;
            public string bcdate { get; set; } = string.Empty;
            public string mimei { get; set; } = string.Empty;
            public string custname { get; set; } = string.Empty;
            public string custmob { get; set; } = string.Empty;
            public string custadd { get; set; } = string.Empty;
            public string prodesc { get; set; } = string.Empty;
            public string probdetails { get; set; } = string.Empty;
            public string problemlist { get; set; } = string.Empty;
            public string deldate { get; set; } = string.Empty;
            public string delvdat { get; set; } = string.Empty;
            public string dcnno { get; set; } = string.Empty;
            public string empid { get; set; } = string.Empty;
            public string empname { get; set; } = string.Empty;
            public string fstatus { get; set; } = string.Empty;
            public string trnstatus { get; set; } = string.Empty;
            public string curstatus { get; set; } = string.Empty;
            public string digstatus { get; set; } = string.Empty;
            public string digndate { get; set; } = string.Empty;
            public string wdone { get; set; } = string.Empty;
            public string custstatus { get; set; } = string.Empty;
            public string retstatus { get; set; } = string.Empty;
            public string approved { get; set; } = string.Empty;
            public string notappved { get; set; } = string.Empty;
            public string rpprocode { get; set; } = string.Empty;
            public string rimei { get; set; } = string.Empty;
            public string appimei { get; set; } = string.Empty;
            public string represdesc { get; set; } = string.Empty;
            public string qcdone { get; set; } = string.Empty;
            public string billst { get; set; } = string.Empty;
            public string custhtmsg { get; set; } = string.Empty;
            public string cutstdesc { get; set; } = string.Empty;
            public decimal balqty { get; set; }
        }
    }
}
