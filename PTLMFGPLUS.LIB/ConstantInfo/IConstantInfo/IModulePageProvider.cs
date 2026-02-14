using PTLMFGPLUS.ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.EnumHelpers;

namespace PTLMFGPLUS.LIB.ConstantInfo.IConstantInfo
{
    public interface IModulePageProvider
    {
        ModuleType Module { get; }
        IEnumerable<PageDefinition> GetPages();
    }
}
