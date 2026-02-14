using DocumentFormat.OpenXml.Spreadsheet;
using PTLMFGPLUS.ENTITY;
using PTLMFGPLUS.LIB.ConstantInfo.IConstantInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.EnumHelpers;

namespace PTLMFGPLUS.LIB.ConstantInfo
{
    public class InventoryPageProvider : IConstantInfo.IModulePageProvider
    {
        public ModuleType Module => ModuleType.Inventory;
        //F_19_FGInv
        public IEnumerable<PageDefinition> GetPages()
        {
            var pages = SetupPages().Concat(OperationPages()).Concat(ReportPages());
            return pages;
        }



        private IEnumerable<PageDefinition> SetupPages()
        {
            PageCategory Category = PageCategory.OneTimeInput;

            var items = new List<PageDefinition>();            
            items.Add(UtilityClass.CreatePage("1901001", "FG Inventory Batchwise", "Inventory", "Item", "Create", "Type=Create", Module, Category));

            return items;

        }
        private IEnumerable<PageDefinition> OperationPages()
        {
            PageCategory Category = PageCategory.Operational;

            var items = new List<PageDefinition>();
            items.Add(UtilityClass.CreatePage("1902001", "FG Inventory Batchwise", "Inventory", "Item", "Create", "", Module, Category));



            return items;

        }
        private IEnumerable<PageDefinition> ReportPages()
        {
            PageCategory Category = PageCategory.Report;

            var items = new List<PageDefinition>();
            items.Add(UtilityClass.CreatePage("1903001", "FG Inventory Batchwise", "Inventory", "Item", "Create", "", Module, Category));

            return items;

        }
    }
}
