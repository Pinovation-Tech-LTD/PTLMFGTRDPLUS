using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.ENTITY
{
    public  class FacebookModels
    {
        public class FacebookPage
        {
            public string id { get; set; }
            public string name { get; set; }
            public string access_token { get; set; }
        }

        public class LeadForm
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Status { get; set; }
        }

        public class Lead
        {
            public int Id { get; set; }
            public string LeadId { get; set; }
            public string FormId { get; set; }
            public string PageId { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string RawData { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        }

        public class FacebookIntegration
        {
            public int Id { get; set; }
            public string UserId { get; set; }         // your app's user
            public string PageId { get; set; }
            public string PageName { get; set; }
            public string PageAccessToken { get; set; }
            public string FormId { get; set; }
            public string FormName { get; set; }
            public bool IsActive { get; set; } = true;
            public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
        }
    }
}
