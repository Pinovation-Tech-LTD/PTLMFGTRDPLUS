
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.LIB.Repository.IRepository
{
    public interface ISP_Call
    {
        Task<DataSet> DataSetAsync(ClassProAccessParams parm, string conn = "");
        Task<bool> ExecuteAsync(ClassProAccessParams parm, string conn = "");
        Task<T> FirstRowAsync<T>(ClassProAccessParams parm);
        Exception? GetError();
        Task<IEnumerable<T>> ListAsync<T>(ClassProAccessParams parm);
        Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> ListAsync<T1, T2>(ClassProAccessParams parm);
        Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> ListAsync<T1, T2, T3>(ClassProAccessParams parm);
        Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>> ListAsync<T1, T2, T3, T4>(ClassProAccessParams parm);
        Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>?>> ListAsync<T1, T2, T3, T4, T5>(ClassProAccessParams parm);
        Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>> ListAsync<T1, T2, T3, T4, T5, T6>(ClassProAccessParams parm);
        Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>> ListAsync<T1, T2, T3, T4, T5, T6, T7>(ClassProAccessParams parm);
        Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>, IEnumerable<T8>, IEnumerable<T9>)> ListAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ClassProAccessParams parm);
        Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>, IEnumerable<T8>, IEnumerable<T9>, IEnumerable<T10>)> ListAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ClassProAccessParams parm);
        
    }
}
