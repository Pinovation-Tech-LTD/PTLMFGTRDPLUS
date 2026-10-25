using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PTLMFGPLUS.LIB;
using System;
using System.Collections.Generic;
using System.Linq;
using static PTLMFGPLUS.ENTITY.ControlPanel.ECompanyPage;
using static PTLMFGPLUS.ENTITY.EClass_Login;

namespace PTLMFGPLUS.SERVICE
{
    public enum PagePermissionAccess
    {
        View,
        Entry,
        Delete,
        Print
    }

    public interface IPagePermissionService
    {
        bool HasPermission(
            string area,
            string controller,
            string action,
            string queryParams = "",
            PagePermissionAccess access = PagePermissionAccess.View);
        bool CanEnter(string area, string controller, string action, string queryParams = "");
        bool CanDelete(string area, string controller, string action, string queryParams = "");
        bool CanPrint(string area, string controller, string action, string queryParams = "");
    }

    public class PagePermissionService(IHttpContextAccessor httpContextAccessor) : IPagePermissionService
    {
        public bool HasPermission(
            string area,
            string controller,
            string action,
            string queryParams = "",
            PagePermissionAccess access = PagePermissionAccess.View)
        {
            List<EUserPagePermission> permissions = ReadPermissions();
            string requestedQuery = NormalizeQuery(queryParams);
            ECompForm configuredPage = ConstantInfo.GetPermissionScope().FirstOrDefault(page =>
                EqualsValue(page.parea, area)
                && EqualsValue(page.pcontroller, controller)
                && EqualsValue(page.paction, action)
                && EqualsValue(NormalizeQuery(page.qrytype), requestedQuery));

            if (configuredPage != null)
            {
                EUserPagePermission permission = permissions.FirstOrDefault(item =>
                    EqualsValue(item.frmid, configuredPage.frmid));
                return Allows(permission, access);
            }

            string requestedPath = NormalizePath(area, controller, action);
            EUserPagePermission routePermission = permissions.FirstOrDefault(permission =>
                PermissionMatchesRoute(permission, requestedPath, controller, requestedQuery));
            return Allows(routePermission, access);
        }

        public bool CanEnter(string area, string controller, string action, string queryParams = "")
        {
            return HasPermission(area, controller, action, queryParams, PagePermissionAccess.Entry);
        }

        public bool CanDelete(string area, string controller, string action, string queryParams = "")
        {
            return HasPermission(area, controller, action, queryParams, PagePermissionAccess.Delete);
        }

        public bool CanPrint(string area, string controller, string action, string queryParams = "")
        {
            return HasPermission(area, controller, action, queryParams, PagePermissionAccess.Print);
        }

        private List<EUserPagePermission> ReadPermissions()
        {
            string json = httpContextAccessor.HttpContext?.Session.GetString("UserPagePermissions");
            return string.IsNullOrWhiteSpace(json)
                ? new List<EUserPagePermission>()
                : JsonConvert.DeserializeObject<List<EUserPagePermission>>(json) ?? new List<EUserPagePermission>();
        }

        private static bool PermissionMatchesRoute(
            EUserPagePermission permission,
            string requestedPath,
            string controller,
            string requestedQuery)
        {
            string storedPath = NormalizeStoredPath(permission.floc, permission.urlinf);
            bool routeMatches = EqualsValue(storedPath, requestedPath)
                || storedPath.EndsWith($"/{requestedPath}", StringComparison.OrdinalIgnoreCase)
                || EqualsValue(RemoveExtension(permission.frmname), controller);
            return routeMatches && EqualsValue(NormalizeQuery(permission.qrytype), requestedQuery);
        }

        private static bool Allows(EUserPagePermission permission, PagePermissionAccess access)
        {
            if (permission == null || !permission.chkper)
            {
                return true;
            }

            return access switch
            {
                PagePermissionAccess.Entry => permission.entry,
                PagePermissionAccess.Delete => permission.delete,
                PagePermissionAccess.Print => permission.printable,
                _ => true
            };
        }

        private static string NormalizeStoredPath(string location, string url)
        {
            string combined = string.Join("/", new[] { location, url }
                .Where(value => !string.IsNullOrWhiteSpace(value)));
            string path = combined.Split('?', 2)[0];
            return path.Trim().Trim('~', '/').Replace("\\", "/");
        }

        private static string NormalizePath(params string[] segments)
        {
            return string.Join("/", segments
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => value.Trim().Trim('~', '/')));
        }

        private static string NormalizeQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return string.Empty;
            }

            string value = query.Trim().TrimStart('?');
            return string.Join("&", value.Split('&', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Split('=', 2))
                .Select(parts => new
                {
                    Key = Uri.UnescapeDataString(parts[0]).Trim(),
                    Value = parts.Length > 1 ? Uri.UnescapeDataString(parts[1]).Trim() : string.Empty
                })
                .OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
                .ThenBy(item => item.Value, StringComparer.OrdinalIgnoreCase)
                .Select(item => $"{item.Key}={item.Value}"));
        }

        private static string RemoveExtension(string value)
        {
            return (value ?? string.Empty).Trim().Replace(".aspx", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        private static bool EqualsValue(string left, string right)
        {
            return string.Equals(left?.Trim(), right?.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
