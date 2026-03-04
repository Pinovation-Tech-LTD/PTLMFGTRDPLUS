using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.SERVICE;
using System.Threading.Tasks;

namespace PTLMFGPLUS.WEB.Components
{
    public class ModuleViewComponent(IMenuService _menuService) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = await _menuService.GetModules();
            return View(items);
        }

    }
}
