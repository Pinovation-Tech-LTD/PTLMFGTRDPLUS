
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using PTLMFGPLUS.SERVICE;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static PTLMFGPLUS.ENTITY.EClass_Login;

namespace PTLMFGPLUS.WEB
{
    public class LicenseService
    {
        private readonly string _password;
        private readonly string _licensePath;
        public LicenseService(IWebHostEnvironment env)
        {
            _licensePath= Path.Combine(env.ContentRootPath, "Data", "license.dat");
            _password = "StrongPassword@123";//config["License:Password"];
        }
        public List<CompanyInfo> GetCompanies()
        {
            if (!File.Exists(_licensePath))
                return new List<CompanyInfo>();

            string encrypted = File.ReadAllText(_licensePath);
            string json = JsonProtector.Decrypt(encrypted, _password);

            var companies = JsonConvert.DeserializeObject<List<CompanyInfo>>(json)
                            ?? new List<CompanyInfo>();

            foreach (var company in companies)
            {
                company.comweb = FirstValue(company.comweb, company.comadd3);
                company.comadd = FirstValue(company.comadd, BuildAddress(company));
            }

            return companies;
        }

        private static string BuildAddress(CompanyInfo company)
        {
            return string.Join("<br />", new[]
            {
                company.comser,
                company.comadd1,
                company.comadd2,
                company.comadd3,
                company.comadd4
            }.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private static string FirstValue(params string[] values) =>
            values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value)) ?? string.Empty;
    }

}
