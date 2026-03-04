using Microsoft.AspNetCore.Http;
using PTLMFGPLUS.ENTITY;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.EClass_Login;
using System.Text.Json;     

namespace PTLMFGPLUS.SERVICE
{
    public interface IMenuService
    {
        public Task<IEnumerable<EUserModule>> GetModules();
    }
    public class MenuService(ICommonService _common, IUnitOfWork _unitofwork, IHttpContextAccessor _httpContextAccessor) : IMenuService
    {
        public async Task<IEnumerable<EUserModule>> GetModules()
        {
            var items = await GetAllModules();
            items = items.Where(x => x.usrper && x.moduleid != "AA" && x.flag).ToList();
            _httpContextAccessor.HttpContext?.Session.SetString("UserModules", JsonSerializer.Serialize(items));
            return items;
        }
        private async Task<IEnumerable<EUserModule>> GetAllModules()
        {
            string comcod = _common.GetComcod();
            string userid = _common.GetUserId();
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_UTILITY_LOGIN_MGT";
            parms.Calltype = "GETCOMMODULE";
            parms.Comp1 = comcod;
            parms.Desc01 = userid;
            var results = await _unitofwork.SP_Call.ListAsync<EUserModule>(parms);
            return results;
        }
    }
}
