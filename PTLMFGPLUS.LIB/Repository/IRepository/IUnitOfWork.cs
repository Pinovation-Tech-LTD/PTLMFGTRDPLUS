using System;
using System.Collections.Generic;
using System.Text;

namespace PTLMFGPLUS.LIB.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ISP_Call SP_Call { get; }//Property
    }
}
