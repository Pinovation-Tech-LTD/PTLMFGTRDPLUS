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
            public string comcod { get; set; }
            [Required(ErrorMessage = "Please Enter Username")]
            public string username { get; set; }
            [Required(ErrorMessage = "Please Enter Password")]
            public string userpass { get; set; }
        }
        public class CompanyInfo
        {
            public string comcod { get; set; }
            public string comname { get; set; }
            public string comsname { get; set; }
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

    }
}
