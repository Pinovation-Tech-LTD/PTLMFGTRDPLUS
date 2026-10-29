using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;
using PTLMFGPLUS.ENTITY;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace PTLMFGPLUS.SERVICE
{
    public interface ICommonService
    {
        string GetComcod();
        string GetEmpId();
        string GetRoleId();
        string GetUserId();
        string GetIPAddress();
        Exception? GetError();   
        string GetUserName();
        string GetTerminalId();
        string GetPostedDate();
        string GetComName();
        string GetComAdd();
        string GetComLogoPath(string fileFolder, string fileName);

    }
    public class CommonService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork) : ICommonService
    {
        private EClass_Login.EUserInfo? LoginUser =>
            GetSessionObject<EClass_Login.EUserInfo>("LoginUser");

        private EClass_Login.CompanyInfo? LoginCompany =>
            GetSessionObject<EClass_Login.CompanyInfo>("LoginCompany");

        private EClass_Login.LoginSessionContext? LoginContext =>
            GetSessionObject<EClass_Login.LoginSessionContext>("LoginContext");

        private T? GetSessionObject<T>(string key)
        {
            string? json = httpContextAccessor.HttpContext?.Session.GetString(key);
            return string.IsNullOrWhiteSpace(json) ? default : JsonConvert.DeserializeObject<T>(json);
        }

        public string GetComcod()
        {
            return GetClaim("Comcod") ?? LoginCompany?.comcod ?? string.Empty;
        }
        public string GetComName()
        {
            return LoginContext?.comnam ?? string.Empty;
        }
        public string GetComAdd()
        {
            return LoginContext?.comadd1 ?? string.Empty;
        }
        public string GetComLogoPath(string fileFolder, string fileName)
        {
            
            fileFolder = Path.GetFileName(fileFolder.TrimEnd('/', '\\'));
            fileName = Path.GetFileName(fileName);

            var fullPath = Path.GetFullPath(Path.Combine(
                LoginContext.envpath, "Images", fileFolder, fileName));

            if (!File.Exists(fullPath))
                return " ";   

            return new Uri(fullPath).AbsoluteUri;
        }

        public string GetUserId()
        {
            return GetClaim("UserId") ?? GetClaim(ClaimTypes.NameIdentifier) ?? LoginUser?.usrid ?? string.Empty;
        }
        public string GetEmpId()
        {
            string empid = GetClaim("EmpId") ?? LoginUser?.empid ?? string.Empty;
            return empid == "" ? "93" : empid;
        }
        public string GetRoleId()
        {
            return GetClaim(ClaimTypes.Role) ?? LoginUser?.userrole ?? string.Empty;
        }
        public Exception? GetError()
        {
            return unitOfWork.SP_Call.GetError();
        }       
      
        public string GetIPAddress()
        {
            return httpContextAccessor?.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
        }
        public string GetUserName()
        {
            return GetClaim(ClaimTypes.Name) ?? LoginUser?.usrname ?? string.Empty;
        }
        public string GetTerminalId()
        {
            return LoginContext?.terminal ?? LoginContext?.compname ?? LoginContext?.trmid ?? string.Empty;
        }
        public string GetPostedDate()
        {
            return LoginContext?.posteddate ?? string.Empty;
        }

        private string? GetClaim(string claimType)
        {
            return httpContextAccessor.HttpContext?.User.FindFirst(claimType)?.Value;
        }
    }
}
