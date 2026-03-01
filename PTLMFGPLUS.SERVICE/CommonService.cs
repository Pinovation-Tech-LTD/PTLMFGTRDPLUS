using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
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
    }
    public class CommonService(IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork) : ICommonService
    {
        public string GetComcod()
        {
            return httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(claim => claim.Type == "Comcod")?.Value ?? string.Empty;
        }
        public string GetUserId()
        {
            return httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(claim => claim.Type == "UserId")?.Value ?? string.Empty;
        }
        public string GetEmpId()
        {
            string empid = httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(claim => claim.Type == "EmpId")?.Value ?? "";
            return empid == "" ? "93" : empid;
        }
        public string GetRoleId()
        {
            return httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.Role)?.Value ?? string.Empty;
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
            return httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value ?? string.Empty;
        }
    }
}
