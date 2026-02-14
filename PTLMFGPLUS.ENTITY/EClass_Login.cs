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
    }
}
