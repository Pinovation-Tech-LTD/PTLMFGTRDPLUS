using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PTLMFGPLUS.SERVICE;
using System;
using System.Threading.Tasks;

namespace PTLMFGPLUS.WEB.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class PagePermissionAttribute : TypeFilterAttribute
    {
        private const string CurrentRequestQuery = "__CURRENT_REQUEST_QUERY__";

        public PagePermissionAttribute(
            string area = "",
            string controller = "",
            string action = "",
            string queryParams = CurrentRequestQuery,
            PagePermissionAccess access = PagePermissionAccess.View) : base(typeof(PagePermissionFilter))
        {
            Arguments = new object[] { area, controller, action, queryParams, access };
        }
    }

    public class PagePermissionFilter(
        IPagePermissionService pagePermissionService,
        string area,
        string controller,
        string action,
        string queryParams,
        PagePermissionAccess access) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string requestedArea = ValueOrRoute(area, context, "area");
            string requestedController = ValueOrRoute(controller, context, "controller");
            string requestedAction = ValueOrRoute(action, context, "action");
            string requestedQuery = queryParams == "__CURRENT_REQUEST_QUERY__"
                ? context.HttpContext.Request.QueryString.Value ?? string.Empty
                : queryParams;

            if (!pagePermissionService.HasPermission(
                requestedArea,
                requestedController,
                requestedAction,
                requestedQuery,
                access))
            {
                string returnUrl = context.HttpContext.Request.PathBase
                    + context.HttpContext.Request.Path
                    + context.HttpContext.Request.QueryString;
                context.Result = new RedirectToActionResult(
                    "AccessDenied",
                    "Home",
                    new { area="", returnUrl });
                return;
            }

            await next();
        }

        private static string ValueOrRoute(string supplied, ActionExecutingContext context, string routeKey)
        {
            return string.IsNullOrWhiteSpace(supplied)
                ? Convert.ToString(context.RouteData.Values[routeKey]) ?? string.Empty
                : supplied;
        }
    }
}
