using PTLMFGPLUS.ENTITY;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using static PTLMFGPLUS.ENTITY.ControlPanel.EUserPermission;

namespace PTLMFGPLUS.SERVICE.ControlPanel
{
    public interface IUserPermissionService
    {
        Exception? GetError();
        Task<IEnumerable<UserRow>> GetUsers(string search);
        Task<IEnumerable<EmployeeOption>> GetEmployees();
        Task<IEnumerable<RoleOption>> GetRoles();
        Task<IEnumerable<EUserModule>> GetModules();
        Task<IEnumerable<PermissionRow>> GetPermissions(string userId, string moduleId, bool showAll);
        Task<bool> SaveUser(SaveUserRequest request);
        Task<bool> SavePermissions(SavePermissionsRequest request);
        Task<bool> CopyPermissions(CopyPermissionRequest request);
    }

    public class UserPermissionService(ICommonService common, IUnitOfWork unitOfWork) : IUserPermissionService
    {
        public Exception? GetError() => unitOfWork.SP_Call.GetError();

        public Task<IEnumerable<UserRow>> GetUsers(string search)
        {
            return unitOfWork.SP_Call.ListAsync<UserRow>(Params("SP_UTILITY_LOGIN_MGT", "SHOWUSER", $"%{search?.Trim()}%"));
        }

        public Task<IEnumerable<EmployeeOption>> GetEmployees()
        {
            return unitOfWork.SP_Call.ListAsync<EmployeeOption>(Params("SP_ENTRY_CODEBOOK", "GETEMPTIDNAME", "%"));
        }

        public Task<IEnumerable<RoleOption>> GetRoles()
        {
            return unitOfWork.SP_Call.ListAsync<RoleOption>(Params("SP_ENTRY_CODEBOOK02", "GETUROLECODE", "%"));
        }

        public Task<IEnumerable<EUserModule>> GetModules()
        {
            return unitOfWork.SP_Call.ListAsync<EUserModule>(Params("SP_UTILITY_LOGIN_MGT", "GETCOMMODULEUPDATE"));
        }

        public async Task<IEnumerable<PermissionRow>> GetPermissions(string userId, string moduleId, bool showAll)
        {
            string moduleFilter = (moduleId == "AA" ? "" : moduleId) + "%";
            await EnsureUserExists(userId);

            if (!showAll)
            {
                return await unitOfWork.SP_Call.ListAsync<PermissionRow>(
                    Params("SP_UTILITY_LOGIN_MGT", "SHOWMODWISEFORM", userId, moduleFilter));
            }

            List<PermissionRow> companyPermissions = (await unitOfWork.SP_Call.ListAsync<PermissionRow>(
                Params("SP_UTILITY_LOGIN_MGT", "SHOWUSERPERFORMASIT", common.GetUserId())))
                .Where(x => moduleId == "AA" || x.frmid.StartsWith(moduleId, StringComparison.Ordinal))
                .ToList();
            List<PermissionRow> userPermissions = (await unitOfWork.SP_Call.ListAsync<PermissionRow>(
                Params("SP_UTILITY_LOGIN_MGT", "SHOWUSERPERFORM", userId, moduleFilter))).ToList();
            var userMap = userPermissions
                .GroupBy(x => $"{x.frmname}|{x.qrytype}", StringComparer.Ordinal)
                .ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);

            return companyPermissions.Select(companyPage =>
            {
                userMap.TryGetValue($"{companyPage.frmname}|{companyPage.qrytype}", out PermissionRow userPage);
                return new PermissionRow
                {
                    frmid1 = companyPage.frmid1,
                    frmid = companyPage.frmid,
                    frmname = companyPage.frmname,
                    qrytype = companyPage.qrytype,
                    dscrption = companyPage.dscrption,
                    modulename = companyPage.modulename,
                    frmdesc = companyPage.frmdesc,
                    chkper = userPage?.chkper ?? false,
                    entry = userPage?.entry ?? false,
                    printable = userPage?.printable ?? false,
                    deleteck = userPage?.deleteck ?? false
                };
            }).OrderBy(x => x.frmid);
        }

        public async Task<bool> SaveUser(SaveUserRequest request)
        {
            if (!request.usrid.StartsWith(common.GetComcod(), StringComparison.Ordinal))
            {
                throw new InvalidOperationException("The user ID must belong to the current company.");
            }
            if (!string.IsNullOrWhiteSpace(request.empid) && !(await GetEmployees()).Any(x => x.empid == request.empid))
            {
                throw new InvalidOperationException("The selected employee is not available for this company.");
            }
            if (!string.IsNullOrWhiteSpace(request.roleid) && !(await GetRoles()).Any(x => x.roleid == request.roleid))
            {
                throw new InvalidOperationException("The selected role is not available for this company.");
            }
            string encodedPassword = string.IsNullOrWhiteSpace(request.password)
                ? ""
                : UtilityClass.EncodePassword(request.usrsname.Trim() + request.password.Trim());
            ClassProAccessParams parameters = Params(
                "SP_UTILITY_LOGIN_MGT",
                "INSORUPDATEUSR",
                request.usrid.Trim(),
                request.usrsname.Trim(),
                request.usrname.Trim(),
                request.usrdesig?.Trim() ?? "",
                encodedPassword,
                request.usrrmrk?.Trim() ?? "",
                request.active ? "1" : "0",
                request.empid ?? "",
                request.roleid ?? "");
            parameters.UserID = common.GetUserId();
            return await unitOfWork.SP_Call.ExecuteAsync(parameters);
        }

        public async Task<bool> SavePermissions(SavePermissionsRequest request)
        {
            string modulePrefix = request.moduleId == "AA" ? "" : request.moduleId;
            List<PermissionRow> allowed = (await GetPermissions(request.userId, request.moduleId, true)).ToList();
            var allowedKeys = allowed.Select(x => $"{x.frmid}|{x.qrytype}").ToHashSet(StringComparer.Ordinal);
            List<PermissionRow> selected = request.permissions
                .Where(x => x.chkper && allowedKeys.Contains($"{x.frmid}|{x.qrytype}"))
                .GroupBy(x => $"{x.frmid}|{x.qrytype}", StringComparer.Ordinal)
                .Select(x => x.First())
                .ToList();

            if (!await unitOfWork.SP_Call.ExecuteAsync(Params(
                "SP_UTILITY_LOGIN_MGT", "DELETEUSER", request.userId, modulePrefix + "%")))
            {
                return false;
            }
            if (selected.Count == 0)
            {
                return true;
            }

            XElement tableRows = new XElement("ds1",
                selected.Select(row => new XElement("tbl1",
                    new XElement("frmid1", row.frmid1 ?? ""),
                    new XElement("frmid", row.frmid ?? ""),
                    new XElement("frmname", row.frmname ?? ""),
                    new XElement("qrytype", row.qrytype ?? ""),
                    new XElement("frmdesc", row.frmdesc ?? ""),
                    new XElement("dscrption", row.dscrption ?? ""),
                    new XElement("modulename", row.modulename ?? ""),
                    new XElement("chkper", row.chkper),
                    new XElement("entry", row.entry),
                    new XElement("printable", row.printable),
                    new XElement("deleteCk", row.deleteck))));
            ClassProAccessParams insert = Params("SP_UTILITY_LOGIN_MGT", "INSERTUSRPER", request.userId);
            insert.Xml01 = tableRows.ToString(SaveOptions.DisableFormatting);
            insert.UserID = common.GetUserId();
            return await unitOfWork.SP_Call.ExecuteAsync(insert);
        }

        public async Task<bool> CopyPermissions(CopyPermissionRequest request)
        {
            await EnsureUserExists(request.fromUserId);
            await EnsureUserExists(request.toUserId);
            return await unitOfWork.SP_Call.ExecuteAsync(Params(
                "SP_ENTRY_CODEBOOK", "COPY_USERTOUSER_PRIVILEGE", request.fromUserId, request.toUserId));
        }

        private async Task EnsureUserExists(string userId)
        {
            bool exists = (await GetUsers("")).Any(x => x.usrid == userId);
            if (!exists)
            {
                throw new InvalidOperationException("The selected user is not available for this company.");
            }
        }

        private ClassProAccessParams Params(string procedure, string callType, params string[] descriptions)
        {
            ClassProAccessParams parameters = new ClassProAccessParams
            {
                Comp1 = common.GetComcod(),
                StoredProcedure = procedure,
                Calltype = callType
            };
            for (int index = 0; index < descriptions.Length && index < 40; index++)
            {
                parameters.GetType().GetProperty($"Desc{index + 1:00}")?.SetValue(parameters, descriptions[index] ?? "");
            }
            return parameters;
        }
    }
}
