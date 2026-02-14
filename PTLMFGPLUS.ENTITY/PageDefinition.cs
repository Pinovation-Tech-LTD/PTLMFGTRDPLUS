using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.EnumHelpers;

namespace PTLMFGPLUS.ENTITY
{
    public class PageDefinition
    {
        public string PageKey { get; set; }   // Unique Key
        public string PageName { get; set; }   
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string QueryString { get; set; }
        
        public ModuleType Module { get; set; }
        public PageCategory Category { get; set; }
        public string ModuleName => Module.ToString();
        public bool IsActive { get; set; } = true;
    }
   
}
