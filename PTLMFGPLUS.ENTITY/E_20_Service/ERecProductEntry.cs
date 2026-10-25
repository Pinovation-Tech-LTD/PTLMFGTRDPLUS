using System;
using System.Collections.Generic;

namespace PTLMFGPLUS.ENTITY.E_20_Service
{
    public class ERecProductEntry
    {
        public class LookupItem
        {
            public string code { get; set; } = "";
            public string description { get; set; } = "";
            public double value { get; set; }
        }

        public class CustomerOption
        {
            public string sircode { get; set; } = "";
            public string sirdesc { get; set; } = "";
            public string phone { get; set; } = "";
            public string addre { get; set; } = "";
            public string email { get; set; } = "";
        }

        public class ProductInfo
        {
            public string sactcode { get; set; } = "";
            public string sdelno { get; set; } = "";
            public string mimei { get; set; } = "";
            public string simei { get; set; } = "";
            public string sdeldate { get; set; } = "";
            public string custid { get; set; } = "";
            public string rescod { get; set; } = "";
            public string actdesc { get; set; } = "";
            public string custname { get; set; } = "";
            public string proddesc { get; set; } = "";
            public string distadd { get; set; } = "";
            public string activedate { get; set; } = "";
            public string bounceset { get; set; } = "";
            public string rwarststus { get; set; } = "";
            public string batchcode { get; set; } = "";
            public string actualowner { get; set; } = "";
            public string actualowneradd { get; set; } = "";
            public string actualownerphone { get; set; } = "";
            public string nextservtype { get; set; } = "";
            public double scount { get; set; }
            public double odometer { get; set; }
            public string regno { get; set; } = "";
            public string scustid { get; set; } = "";
        }

        public class ReceiveHeader
        {
            public string recvno { get; set; } = "";
            public string centrid { get; set; } = "";
            public string mimei { get; set; } = "";
            public string simei { get; set; } = "";
            public string recvdate { get; set; } = "";
            public string custname { get; set; } = "";
            public string custmob { get; set; } = "";
            public string custmob2 { get; set; } = "";
            public string custmail { get; set; } = "";
            public string custadd { get; set; } = "";
            public string custop { get; set; } = "";
            public string deldate { get; set; } = "";
            public string wrcode { get; set; } = "";
            public string hsmissing { get; set; } = "";
            public string bcsircode { get; set; } = "";
            public string bcnno { get; set; } = "";
            public string bcdate { get; set; } = "";
            public string custid { get; set; } = "";
            public string odometer { get; set; } = "";
            public string regno { get; set; } = "";
            public string empid { get; set; } = "";
            public string advance { get; set; } = "";
        }

        public class ReceiveLine
        {
            public string grp { get; set; } = "";
            public string gcod { get; set; } = "";
            public string gdesc { get; set; } = "";
            public string remarks { get; set; } = "";
        }

        public class InitialResponse
        {
            public string companyCode { get; set; } = "";
            public string entryDate { get; set; } = "";
            public string probableDeliveryDate { get; set; } = "";
            public string activationDate { get; set; } = "";
            public string bookingDate { get; set; } = "";
            public List<LookupItem> serviceCenters { get; set; } = new();
            public List<LookupItem> serviceTypes { get; set; } = new();
            public List<LookupItem> problems { get; set; } = new();
            public List<LookupItem> accessories { get; set; } = new();
            public List<LookupItem> transports { get; set; } = new();
            public List<LookupItem> mechanics { get; set; } = new();
            public List<CustomerOption> customers { get; set; } = new();
        }

        public class EditResponse
        {
            public ReceiveHeader header { get; set; } = new();
            public ProductInfo product { get; set; } = new();
            public List<ReceiveLine> lines { get; set; } = new();
        }

        public class ProductResponse
        {
            public ProductInfo product { get; set; } = new();
            public List<object> history { get; set; } = new();
        }

        public class SaveRequest
        {
            public string type { get; set; } = "Entry";
            public string previousReceiveNo { get; set; } = "";
            public string receiveNo { get; set; } = "";
            public string centerId { get; set; } = "";
            public string imei { get; set; } = "";
            public string receiveDate { get; set; } = "";
            public string probableDeliveryDate { get; set; } = "";
            public string activationDate { get; set; } = "";
            public string bookingDate { get; set; } = "";
            public string serviceType { get; set; } = "";
            public string customerId { get; set; } = "";
            public string customerName { get; set; } = "";
            public string customerMobile { get; set; } = "";
            public string customerMobile2 { get; set; } = "";
            public string customerEmail { get; set; } = "";
            public string customerAddress { get; set; } = "";
            public string customerComplaint { get; set; } = "";
            public string missingItems { get; set; } = "";
            public string transportCode { get; set; } = "";
            public string consignmentNo { get; set; } = "";
            public string odometer { get; set; } = "";
            public string registrationNo { get; set; } = "";
            public string mechanicId { get; set; } = "";
            public decimal advance { get; set; }
            public List<ReceiveLine> lines { get; set; } = new();
        }
    }
}
