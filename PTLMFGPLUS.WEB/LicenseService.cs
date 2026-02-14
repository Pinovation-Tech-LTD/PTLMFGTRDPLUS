
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using PTLMFGPLUS.SERVICE;
using System.Collections.Generic;
using System.IO;
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

            return JsonConvert.DeserializeObject<List<CompanyInfo>>(json)
                   ?? new List<CompanyInfo>();
        }
    }

}
