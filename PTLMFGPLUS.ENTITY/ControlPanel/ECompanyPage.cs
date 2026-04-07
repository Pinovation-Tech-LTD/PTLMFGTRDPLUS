using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.ENTITY.ControlPanel
{
    public class ECompanyPage
    {
        
        public class ECompForm
        {
            public string comcod { get; set; }
            public string frmgrp { get; set; }
            public string frmid { get; set; }
            public string frmname { get; set; }
            public string parea { get; set; }
            public string pcontroller { get; set; }
            public string paction { get; set; }
            public string qrytype { get; set; }
            public bool chkper { get; set; } = false;
            public bool entry { get; set; } = false;
            public bool printable { get; set; } = false;
            public bool defaultper { get; set; } = false;
        }

        public class ECompanyPagePermission
        {
            public string frmgrp { get; set; }
            public string frmid { get; set; }
            public string pagelink { get; set; }

            public string qrytype { get; set; }
            public string dscrption { get; set; }
            public bool   chkper { get; set; } = false;
            public string modulename { get; set; }
            public string fmgrpdesc { get; set; }
        }
    }
}
