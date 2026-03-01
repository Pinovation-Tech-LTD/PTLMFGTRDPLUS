
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.EClass_Login;

namespace PTLMFGPLUS.SERVICE
{
    public interface ILoginService
    {
        public Task<Tuple<IEnumerable<EUserInfo>, IEnumerable<ECompanyStandard>>> GetValidUser(string comcod, string username, string userpass);
    }
    public class LoginService(ICommonService _common, IUnitOfWork _unitofwork) : ILoginService
    {
        private string EncodePassword(string username, string password)
        {
            return UtilityClass.EncodePassword(username + password.Trim());
        }
        public async Task<Tuple<IEnumerable<EUserInfo>, IEnumerable<ECompanyStandard>>> GetValidUser(string comcod, string username, string userpass)
        {
            string ipaddress = _common.GetIPAddress();
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_UTILITY_LOGIN_MGT";
            parms.Calltype = "LOGINUSERCORE";
            parms.Comp1 = comcod;
            parms.Desc01 = username;
            parms.Desc02 = EncodePassword(username, userpass.Trim());
            var results= await _unitofwork.SP_Call.ListAsync<EUserInfo, ECompanyStandard>(parms);
            return results;
        }

       
    }
}
