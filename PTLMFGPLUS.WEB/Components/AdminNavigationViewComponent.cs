using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PTLMFGPLUS.ENTITY;
using PTLMFGPLUS.SERVICE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.EClass_Login;

namespace PTLMFGPLUS.WEB.Components
{
    public class AdminNavigationViewComponent : ViewComponent
    {
        private readonly IMenuService _menuService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AdminNavigationViewComponent(
            IMenuService menuService,
            IHttpContextAccessor httpContextAccessor)
        {
            _menuService = menuService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new AdminNavigationViewModel();

            try
            {
                model.Modules = (await _menuService.GetModules()).ToList();
            }
            catch
            {
                model.Modules = ReadSession<List<EUserModule>>("UserModules") ?? new List<EUserModule>();
            }

            var permissions = ReadSession<List<EUserPagePermission>>("UserPagePermissions")
                ?? new List<EUserPagePermission>();

            model.Interfaces = BuildPermissionGroup(permissions, "I");
            model.Dashboards = BuildPermissionGroup(permissions, "D");
            model.Analysis = BuildPermissionGroup(permissions, "A");
            model.AllInOne = BuildPermissionGroup(permissions, "M").FirstOrDefault();
            model.Shortcuts = BuildShortcuts(permissions);

            return View(model);
        }

        private static List<AdminNavigationItem> BuildPermissionGroup(
            List<EUserPagePermission> permissions,
            string marker)
        {
            


            return permissions
                .Where(permission => string.Equals(permission.@interface?.Trim(), marker, StringComparison.OrdinalIgnoreCase))
                .Select(ToNavigationItem)
                //.Where(item => item != null)
                .GroupBy(item => item.Url, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .ToList();
        }

        private List<AdminNavigationItem> BuildShortcuts(List<EUserPagePermission> permissions)
        {
            var resultSets = ReadSession<List<List<Dictionary<string, string>>>>("tblusrlog")
                ?? new List<List<Dictionary<string, string>>>();
            var shortcutRows = resultSets.ElementAtOrDefault(5)
                ?? new List<Dictionary<string, string>>();
            var shortcutIds = shortcutRows
                .Select(row => Value(row, "formid", "frmid"))
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return permissions
                .Where(permission => permission.chkper && shortcutIds.Contains(permission.frmid ?? string.Empty))
                .Select(ToNavigationItem)
                .Where(item => item != null)
                .GroupBy(item => item.Url, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .Take(12)
                .ToList();
        }

        private static AdminNavigationItem ToNavigationItem(EUserPagePermission permission)
        {
            string title = FirstValue(permission.dscrption, permission.frmname, permission.frmid);
            string url = GetMigratedRoute(title);
            if (string.IsNullOrWhiteSpace(url))
            {
                url = NormalizeUrl(permission.floc, permission.urlinf);
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            return new AdminNavigationItem
            {
                Id = permission.frmid,
                Title = title,
                Url = url,
                Icon = "bi-grid"
            };
        }

        private static string GetMigratedRoute(string title)
        {
            if (title.Contains("materials transfer interface", StringComparison.OrdinalIgnoreCase)
                || title.Contains("raw material interface", StringComparison.OrdinalIgnoreCase))
            {
                return "/F_07_RM/RawMattInterface/InterfaceIndex";
            }

            if (title.Contains("production interface", StringComparison.OrdinalIgnoreCase))
            {
                return "/F_13_ProdMon/ProductionInterface/ProductionInterfaceIndex?type=FG";
            }

            if (title.Contains("service interface", StringComparison.OrdinalIgnoreCase))
            {
                return "/F_20_Service/RptServInterface?Type=All";
            }

            if (title.Contains("production requisition", StringComparison.OrdinalIgnoreCase))
            {
                return "/F_11_Pro/ProdReq";
            }

            if (title.Contains("production budget", StringComparison.OrdinalIgnoreCase))
            {
                return "/F_13_ProdMon/ProdBudget/ProdBudgetIndex";
            }

            if (title.Contains("material issue", StringComparison.OrdinalIgnoreCase))
            {
                return "/F_07_Inv/PBMatIssueSingle";
            }

            return string.Empty;
        }

        private static string NormalizeUrl(string folder, string page)
        {
            string cleanFolder = folder?.Trim().Trim('~', '/') ?? string.Empty;
            string cleanPage = page?.Trim().Trim('~', '/') ?? string.Empty;
            string value = string.Join("/", new[] { cleanFolder, cleanPage }
                .Where(part => !string.IsNullOrWhiteSpace(part)));
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            if (Uri.TryCreate(value, UriKind.Absolute, out _))
            {
                return value;
            }

            return value.StartsWith('/') ? value : $"/{value.TrimStart('~', '/')}";
        }

        private T ReadSession<T>(string key)
        {
            string json = _httpContextAccessor.HttpContext?.Session.GetString(key);
            return string.IsNullOrWhiteSpace(json) ? default : JsonConvert.DeserializeObject<T>(json);
        }

        private static string Value(Dictionary<string, string> row, params string[] keys)
        {
            foreach (string key in keys)
            {
                if (row.TryGetValue(key, out string value))
                {
                    return value;
                }
            }

            return string.Empty;
        }

        private static string FirstValue(params string[] values)
        {
            return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
        }
    }
}
