using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.ENTITY
{
    public class EnumHelpers
    {
        public enum ModuleType
        {
            Inventory=1,
            Accounts=2,
            HR=3,
            Sales=4
        }

        public enum PageCategory
        {
            OneTimeInput,
            Operational,
            Report
        }

        public enum PagePermissionType
        {
            View,
            Add,
            Edit,
            Delete,
            Print,
            Approval
        }

    }
}
