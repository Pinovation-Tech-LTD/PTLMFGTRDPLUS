using PTLMFGPLUS.ENTITY;
using PTLMFGPLUS.LIB.ConstantInfo.IConstantInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.EnumHelpers;

namespace PTLMFGPLUS.SERVICE
{
    public interface IPageRegistry
    {
        IEnumerable<PageDefinition> GetAll();
        IEnumerable<PageDefinition> GetByModule(ModuleType module);

    }
    public class PageRegistry : IPageRegistry
    {
        private readonly IEnumerable<IModulePageProvider> _providers;

        public PageRegistry(IEnumerable<IModulePageProvider> providers)
        {
            _providers = providers;
        }

        public IEnumerable<PageDefinition> GetAll()
        {
            var pages= _providers.SelectMany(p => p.GetPages());
            ValidateUniquePageKeys(pages);
            return pages;
        }

        public IEnumerable<PageDefinition> GetByModule(ModuleType module)
        {
            var pages = _providers
                .Where(p => p.Module == module)
                .SelectMany(p => p.GetPages());
            ValidateUniquePageKeys(pages);
            return pages;
        }
        private void ValidateUniquePageKeys(IEnumerable<PageDefinition> pages)
        {
            var duplicates = pages
                .GroupBy(p => p.PageKey)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Any())
            {
                throw new Exception(
                    $"Duplicate PageKey detected: {string.Join(", ", duplicates)}");
            }
        }

    }

}
