using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PTLMFGPLUS.ENTITY;
using PTLMFGPLUS.LIB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.EClass_Login;
using static PTLMFGPLUS.ENTITY.EnumHelpers;

namespace PTLMFGPLUS.SERVICE
{
    public interface IStepOfOperationService
    {
        Task<StepOfOperationViewModel> GetAsync(string moduleId);
    }

    public class StepOfOperationService(
        IMenuService menuService,
        IHttpContextAccessor httpContextAccessor) : IStepOfOperationService
    {
        public async Task<StepOfOperationViewModel> GetAsync(string moduleId)
        {
            string normalizedModuleId = (moduleId ?? string.Empty).Trim().PadLeft(2, '0');
            var modules = (await menuService.GetModules()).ToList();
            var selectedModule = modules.FirstOrDefault(module =>
                string.Equals(module.moduleid?.Trim(), normalizedModuleId, StringComparison.OrdinalIgnoreCase));

            if (selectedModule == null)
            {
                return null;
            }

            var permissions = ReadSession<List<EUserPagePermission>>("UserPagePermissions")
                ?? new List<EUserPagePermission>();

            return new StepOfOperationViewModel
            {
                SelectedModuleId = normalizedModuleId,
                SelectedModuleName = selectedModule.modulename,
                CompanyName = httpContextAccessor.HttpContext?.User.FindFirst("CompanyName")?.Value ?? string.Empty,
                Modules = modules,
                Sections = BuildSections(GetMenuCatalog(normalizedModuleId), permissions)
            };
        }

        private static List<EMenuTable> GetMenuCatalog(string moduleId)
        {
            if (!int.TryParse(moduleId, out int numericModuleId)
                || !Enum.IsDefined(typeof(ModuleList), numericModuleId))
            {
                return new List<EMenuTable>();
            }

            return ConstantInfo.AllMenuTable((ModuleList)numericModuleId);
        }

        private static List<StepOfOperationSection> BuildSections(
            List<EMenuTable> catalog,
            List<EUserPagePermission> permissions)
        {
            var sectionDefinitions = new[]
            {
                new { Code = "02", Subtitle = "Configure module master data", Icon = "bi-sliders" },
                new { Code = "03", Subtitle = "Run daily operational transactions", Icon = "bi-arrow-left-right" },
                new { Code = "04", Subtitle = "Review reports and analysis", Icon = "bi-bar-chart-line" }
            };

            return sectionDefinitions.Select(definition =>
            {
                var header = catalog.FirstOrDefault(item =>
                    !item.itemslct && item.itemcod?.StartsWith(definition.Code, StringComparison.Ordinal) == true);
                var items = catalog
                    .Where(item => item.itemslct
                        && item.itemcod?.StartsWith(definition.Code, StringComparison.Ordinal) == true)
                    .Select(item => ApplyPermission(item, permissions))
                    .Where(item => item != null)
                    .ToList();

                return new StepOfOperationSection
                {
                    Code = definition.Code,
                    Title = header?.itemdesc ?? definition.Code,
                    Subtitle = definition.Subtitle,
                    Icon = definition.Icon,
                    Items = items
                };
            }).ToList();
        }

        private static StepOfOperationItem ApplyPermission(
            EMenuTable menuItem,
            List<EUserPagePermission> permissions)
        {
            bool isTemplate = string.Equals(menuItem.formid, "TEMPLATE", StringComparison.OrdinalIgnoreCase)
                || string.Equals(menuItem.fbold, "template", StringComparison.OrdinalIgnoreCase);
            if (isTemplate)
            {
                return new StepOfOperationItem
                {
                    Code = menuItem.itemcod,
                    Title = menuItem.itemdesc,
                    Url = string.Empty,
                    FormId = menuItem.formid,
                    IsTemplate = true
                };
            }

            string permissionKey = GetPermissionKey(menuItem);
            var permission = permissions.FirstOrDefault(value =>
                value.chkper
                && !string.IsNullOrWhiteSpace(menuItem.formid)
                && string.Equals(value.frmid, menuItem.formid, StringComparison.OrdinalIgnoreCase))
                ?? permissions.FirstOrDefault(value =>
                    value.chkper
                    && string.Equals(GetPermissionKey(value), permissionKey, StringComparison.OrdinalIgnoreCase));
            if (permission == null)
            {
                return null;
            }

            return new StepOfOperationItem
            {
                Code = menuItem.itemcod,
                Title = string.IsNullOrWhiteSpace(permission.dscrption)
                    ? menuItem.itemdesc
                    : permission.dscrption.Trim(),
                Url = BuildUrl(menuItem),
                FormId = permission.frmid,
                IsTemplate = false
            };
        }

        private static string GetPermissionKey(EMenuTable menuItem)
        {
            return $"{menuItem.pcontroller}{NormalizeQuery(menuItem.proutes)}";
        }

        private static string GetPermissionKey(EUserPagePermission permission)
        {
            return $"{permission.frmname}{NormalizeQuery(permission.qrytype)}"
                .Replace(".aspx", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeQuery(string query)
        {
            return string.IsNullOrWhiteSpace(query)
                ? string.Empty
                : query.Trim().TrimStart('?');
        }

        private static string BuildUrl(EMenuTable item)
        {
            string path = string.Join("/", new[] { item.parea, item.pcontroller, item.paction }
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim('~', '/')));
            string query = NormalizeQuery(item.proutes);
            return string.IsNullOrWhiteSpace(query) ? $"/{path}" : $"/{path}?{query}";
        }

        private T ReadSession<T>(string key)
        {
            string json = httpContextAccessor.HttpContext?.Session.GetString(key);
            return string.IsNullOrWhiteSpace(json) ? default : JsonConvert.DeserializeObject<T>(json);
        }
    }
}
