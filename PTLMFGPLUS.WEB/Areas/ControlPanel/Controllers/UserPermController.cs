using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTLMFGPLUS.ENTITY;
using PTLMFGPLUS.SERVICE;
using PTLMFGPLUS.SERVICE.ControlPanel;
using PTLMFGPLUS.WEB.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.ControlPanel.EUserPermission;

namespace PTLMFGPLUS.WEB.Areas.ControlPanel.Controllers
{
    [Authorize]
    [PagePermission("ControlPanel", "UserPerm", "UserPermIndex", "")]
    [Area("ControlPanel")]
    public class UserPermController(IUserPermissionService userPermissionService) : Controller
    {
        [HttpGet]
        public IActionResult UserPermIndex()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetInitialData(string search = "")
        {
            try
            {
                var users = await userPermissionService.GetUsers(search);
                var employees = await userPermissionService.GetEmployees();
                var roles = await userPermissionService.GetRoles();
                var modules = await userPermissionService.GetModules();
                return Ok(new { users, employees, roles, modules });
            }
            catch
            {
                return BadRequest(new { objError = "User permission data could not be loaded." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(string search = "")
        {
            try
            {
                return Ok(await userPermissionService.GetUsers(search));
            }
            catch
            {
                return BadRequest(new { objError = "Users could not be loaded." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPermissions(string userId, string moduleId = "AA", bool showAll = false)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(moduleId))
            {
                return BadRequest(new { objError = "Select a user and module." });
            }
            try
            {
                var modules = await userPermissionService.GetModules();
                if (moduleId != "AA" && !modules.Any(x => x.moduleid == moduleId))
                {
                    return BadRequest(new { objError = "The selected module is not available." });
                }
                return Ok(await userPermissionService.GetPermissions(userId, moduleId, showAll));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { objError = ex.Message });
            }
            catch
            {
                return BadRequest(new { objError = "User permissions could not be loaded." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PagePermission(
            "ControlPanel",
            "UserPerm",
            "UserPermIndex",
            "",
            PagePermissionAccess.Entry)]
        public async Task<IActionResult> SaveUser([FromBody] SaveUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { objError = "User ID, short name and full name are required." });
            }
            try
            {
                bool saved = await userPermissionService.SaveUser(request);
                return saved
                    ? Ok(new { message = "User information updated successfully." })
                    : BadRequest(new { objError = "User information could not be updated." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { objError = ex.Message });
            }
            catch
            {
                return BadRequest(new { objError = "User information could not be updated." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PagePermission(
            "ControlPanel",
            "UserPerm",
            "UserPermIndex",
            "",
            PagePermissionAccess.Entry)]
        public async Task<IActionResult> SavePermissions([FromBody] SavePermissionsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { objError = "Select a user and module before saving." });
            }
            try
            {
                bool saved = await userPermissionService.SavePermissions(request);
                return saved
                    ? Ok(new { message = "User page permissions updated successfully." })
                    : BadRequest(new { objError = "User page permissions could not be updated." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { objError = ex.Message });
            }
            catch
            {
                return BadRequest(new { objError = "User page permissions could not be updated." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PagePermission(
            "ControlPanel",
            "UserPerm",
            "UserPermIndex",
            "",
            PagePermissionAccess.Entry)]
        public async Task<IActionResult> CopyPermissions([FromBody] CopyPermissionRequest request)
        {
            if (!ModelState.IsValid || request.fromUserId == request.toUserId)
            {
                return BadRequest(new { objError = "Select two different users." });
            }
            try
            {
                bool copied = await userPermissionService.CopyPermissions(request);
                return copied
                    ? Ok(new { message = "User privileges copied successfully." })
                    : BadRequest(new { objError = "User privileges could not be copied." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { objError = ex.Message });
            }
            catch
            {
                return BadRequest(new { objError = "User privileges could not be copied." });
            }
        }

    }
}
