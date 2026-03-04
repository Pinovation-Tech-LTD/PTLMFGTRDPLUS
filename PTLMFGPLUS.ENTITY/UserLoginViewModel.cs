using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.ENTITY
{
    public class UserLoginViewModel
    {
        public string SelectedModule { get; set; }
        public List<EUserModule> Modules { get; set; } = new();
    }
}
