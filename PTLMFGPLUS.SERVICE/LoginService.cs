
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.SERVICE
{
    public interface ILoginService
    {
        
    }
    public class LoginService: ILoginService
    {
        public IUnitOfWork unitofwork { get; set; }

        public LoginService(IUnitOfWork _unitofwork)
        {
            unitofwork = _unitofwork;
        }        
    }
}
