using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.ENTITY
{
    public class EClass_Login
    {
        public class Login_VM
        {
            [Required(ErrorMessage = "Please Select Company")]
            public string comcod { get; set; }
            [Required(ErrorMessage = "Please Enter Username")]
            public string username { get; set; }
            [Required(ErrorMessage = "Please Enter Password")]
            public string userpass { get; set; }
        }
        public class CompanyInfo
        {
            public int slnum { get; set; }
            public string comcod { get; set; }
            public string comname { get; set; }
            public string comsname { get; set; }
            public string comser { get; set; }
            public string comadd { get; set; }
            public string comadd1 { get; set; }
            public string comadd2 { get; set; }
            public string comadd3 { get; set; }
            public string comadd4 { get; set; }
            public string compcod { get; set; }
            public string combranch { get; set; }
            public string comphone { get; set; }
            public string comemail { get; set; }
            public string comweb { get; set; }
            public string comfax { get; set; }
            public DateTime ExpiryDate { get; set; }
            public bool IsActive { get; set; }
        }

        public class EUserInfo
        {
            public string usrid { get; set; }
            public string usrsname { get; set; }
            public string usrname { get; set; }
            public string usrdesig { get; set; }
            public string empid { get; set; }
            public string deptcode { get; set; }
            public string deptname { get; set; }            
            public string userrole { get; set; }
            public string usrimg { get; set; }
            public string circle { get; set; }
            public string area { get; set; }
            public string region { get; set; }
            public string territory { get; set; }
            public string sroute { get; set; }
            public string location { get; set; }
            public string urole { get; set; }
            public bool compsms { get; set; }
            public bool compmail { get; set; }
            public string salestype { get; set; }
            public bool issostockcheck { get; set; }
            public string apiurl { get; set; }
        }

        public class ECompanyStandard
        {
            public string comcod { get; set; }
            public string compbin { get; set; }
            public bool autovou { get; set; }
            public bool compsms { get; set; }
            public bool compmail { get; set; }
            public string salestype { get; set; }     
            public bool oversale { get; set; }
        }

        public class EUserPagePermission
        {
            public string frmid { get; set; }
            public string frmname { get; set; }
            public string qrytype { get; set; }
            public string dscrption { get; set; }
            public string floc { get; set; }
            public string urlinf { get; set; }
            public string @interface { get; set; }
            public bool chkper { get; set; }
            public bool entry { get; set; }
            public bool printable { get; set; }
            public bool delete { get; set; }
        }

        public class ELoginCompanyLink
        {
            public string comcod { get; set; }
            public string mcomcod { get; set; }
            public string scomcod { get; set; }
        }

        public class ELoginHrPeriod
        {
            public string hrsdate { get; set; }
            public string hredate { get; set; }
        }

        public class LoginDatabaseData
        {
            public EUserInfo User { get; set; }
            public List<EUserPagePermission> PagePermissions { get; set; } = new();
            public ELoginCompanyLink CompanyLink { get; set; } = new();
            public ELoginHrPeriod HrPeriod { get; set; } = new();
            public List<List<Dictionary<string, string>>> ResultSets { get; set; } = new();
        }

        public class LoginSessionContext
        {
            public string comcod { get; set; }
            public string envpath { get; set; }
            public string comnam { get; set; }
            public string comsnam { get; set; }
            public string comadd { get; set; }
            public string comadd1 { get; set; }
            public string comadd2 { get; set; }
            public string comadd3 { get; set; }
            public string comadd4 { get; set; }
            public string comweb { get; set; }
            public string comphone { get; set; }
            public string comemail { get; set; }
            public string comfax { get; set; }
            public string combranch { get; set; }
            public string compcod { get; set; }
            public string deptcode { get; set; }
            public string ldeptcode { get; set; }
            public string deptname { get; set; }
            public string modulenam { get; set; }
            public string username { get; set; }
            public string usrname { get; set; }
            public string usrdesig { get; set; }
            public string compname { get; set; }
            public string terminal { get; set; }
            public string posteddate { get; set; }
            public string usrid { get; set; }
            public string session { get; set; }
            public string trmid { get; set; }
            public string commod { get; set; }
            public string empid { get; set; }
            public string mcomcod { get; set; }
            public string scomcod { get; set; }
            public string circle { get; set; }
            public string area { get; set; }
            public string region { get; set; }
            public string territory { get; set; }
            public string sroute { get; set; }
            public string location { get; set; }
            public string urole { get; set; }
            public string userrole { get; set; }
            public bool compsms { get; set; }
            public bool mailstus { get; set; }
            public string salestype { get; set; }
            public string hrsdate { get; set; }
            public string hredate { get; set; }
            public bool issostockcheck { get; set; }
            public string apiurl { get; set; }
        }

    }
}
