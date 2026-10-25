using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PTLMFGPLUS.ENTITY.ControlPanel
{
    public class EUserPermission
    {
        public class UserRow
        {
            public string usrid { get; set; }
            public string usrsname { get; set; }
            public string usrname { get; set; }
            public string usrdesig { get; set; }
            public bool usractive { get; set; }
            public string usrrmrk { get; set; }
            public string empid { get; set; }
            public string empname { get; set; }
            public string urole { get; set; }
            public string roledesc { get; set; }
        }

        public class EmployeeOption
        {
            public string empid { get; set; }
            public string empname { get; set; }
        }

        public class RoleOption
        {
            public string roleid { get; set; }
            public string roledesc { get; set; }
        }

        public class PermissionRow
        {
            public string frmid1 { get; set; }
            public string frmid { get; set; }
            public string frmname { get; set; }
            public string qrytype { get; set; }
            public string dscrption { get; set; }
            public string modulename { get; set; }
            public string frmdesc { get; set; }
            public bool chkper { get; set; }
            public bool entry { get; set; }
            public bool printable { get; set; }
            public bool deleteck { get; set; }
        }

        public class SaveUserRequest
        {
            [Required]
            [StringLength(20)]
            public string usrid { get; set; }
            [Required]
            [StringLength(100)]
            public string usrsname { get; set; }
            [Required]
            [StringLength(150)]
            public string usrname { get; set; }
            [StringLength(100)]
            public string usrdesig { get; set; }
            [StringLength(100)]
            public string password { get; set; }
            [StringLength(250)]
            public string usrrmrk { get; set; }
            public bool active { get; set; }
            public string empid { get; set; }
            public string roleid { get; set; }
        }

        public class SavePermissionsRequest
        {
            [Required]
            public string userId { get; set; }
            [Required]
            public string moduleId { get; set; }
            public List<PermissionRow> permissions { get; set; } = new List<PermissionRow>();
        }

        public class CopyPermissionRequest
        {
            [Required]
            public string fromUserId { get; set; }
            [Required]
            public string toUserId { get; set; }
        }
    }
}
