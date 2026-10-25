
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using static PTLMFGPLUS.ENTITY.EClass_Login;

namespace PTLMFGPLUS.SERVICE
{
    public interface ILoginService
    {
        public Task<Tuple<IEnumerable<EUserInfo>, IEnumerable<ECompanyStandard>>> GetValidUser(string comcod, string username, string userpass);
        public Task<LoginDatabaseData> GetLoginDatabaseData(string comcod, string username, string userpass);
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

        public async Task<LoginDatabaseData> GetLoginDatabaseData(string comcod, string username, string userpass)
        {
            ClassProAccessParams parms = new ClassProAccessParams
            {
                StoredProcedure = "SP_UTILITY_LOGIN_MGT",
                Calltype = "LOGINUSER",
                Comp1 = comcod,
                Desc01 = username,
                Desc02 = EncodePassword(username, userpass.Trim()),
                Desc03 = "AA",
                Desc04 = string.Empty
            };

            DataSet result = await _unitofwork.SP_Call.DataSetAsync(parms);
            var loginData = new LoginDatabaseData();
            if (result == null)
            {
                return loginData;
            }

            if (result.Tables.Count > 0)
                loginData.User = result.Tables[0].DataTableToList<EUserInfo>().FirstOrDefault();
            if (result.Tables.Count > 1)
                loginData.PagePermissions = result.Tables[1].DataTableToList<EUserPagePermission>();
            if (result.Tables.Count > 3)
                loginData.CompanyLink = result.Tables[3].DataTableToList<ELoginCompanyLink>().FirstOrDefault() ?? new ELoginCompanyLink();
            if (result.Tables.Count > 4)
                loginData.HrPeriod = result.Tables[4].DataTableToList<ELoginHrPeriod>().FirstOrDefault() ?? new ELoginHrPeriod();

            loginData.ResultSets = result.Tables.Cast<DataTable>()
                .Select(table => table.Rows.Cast<DataRow>()
                    .Select(row => table.Columns.Cast<DataColumn>()
                        .ToDictionary(column => column.ColumnName,
                            column => row[column] == DBNull.Value ? string.Empty : Convert.ToString(row[column]) ?? string.Empty,
                            StringComparer.OrdinalIgnoreCase))
                    .ToList())
                .ToList();

            return loginData;
        }

       
    }
}
