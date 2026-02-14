
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using PTLMFGPLUS.ENTITY.Exceptions;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.LIB.Repository
{
    public class SP_Call : ISP_Call
    {
        private static string ConnectionString = "";
        private static string AddLogRecordConnectionString = "";
        private Exception? m_Erroobj;
        public SP_Call(IConfiguration db)
        {
            ConnectionString = db.GetConnectionString("DefaultConnection") ?? "";
            AddLogRecordConnectionString = db.GetConnectionString("AddLogRecordConnection") ?? "";
            m_Erroobj = null;
        }
        private Exception? ErrorObject
        {
            get
            {
                return m_Erroobj;
            }
        }
        private void ClearErrors()
        {
            m_Erroobj = null;
        }
        private void SetError(Exception ex)
        {
            m_Erroobj = ex;
        }

        public Exception? GetError()
        {
            return ErrorObject;
        }


        public async Task<DataSet> GetDataSet(SqlCommand cmd, string connection = "")
        {
            DataSet ds = new DataSet();
            try
            {
                ClearErrors();
                using (SqlConnection conn = new SqlConnection(connection == "" ? ConnectionString : connection))
                {
                    await conn.OpenAsync(); // Open connection asynchronously
                    using (SqlDataAdapter adp = new SqlDataAdapter())
                    {
                        cmd.Connection = conn;
                        cmd.CommandTimeout = 0; // Set a reasonable timeout                       
                        adp.SelectCommand = cmd;
                        await Task.Run(() => adp.Fill(ds));// Run Fill on a background thread
                    }
                }
                return ds;
            }
            catch (SqlException sqlEx)
            {
                SetError(sqlEx);
                //Console.WriteLine($"SQL Error: {sqlEx.Message}");
                throw; //new DatabaseConnectionException;
            }
            catch (TimeoutException timeoutEx)
            {
                // Handle timeout exceptions
                SetError(timeoutEx);
                throw;
            }
            catch (Exception ex)
            {
                SetError(ex);
                //Console.WriteLine($"Unexpected Error: {ex.Message}");
                throw;
            }

        }




        public async Task<DataSet> DataSetAsync(ClassProAccessParams parm, string conn = "")
        {
            try
            {
                using (SqlCommand param = new SqlCommand(parm.StoredProcedure))
                {
                    this.ClearErrors();
                    param.CommandType = CommandType.StoredProcedure;
                    param.Parameters.Add(new SqlParameter("@Comp1", parm.Comp1));
                    param.Parameters.Add(new SqlParameter("@CallType", parm.Calltype));
                    param.Parameters.Add("@Dxml01", SqlDbType.Xml).Value = parm.Dxml01?.GetXml();
                    param.Parameters.Add("@Dxml02", SqlDbType.Xml).Value = parm.Dxml02?.GetXml();
                    var props = parm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in props)
                    {
                        string propName = prop.Name;

                        var value = prop.GetValue(parm) as string;
                        if (propName.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                        {
                            string sqlPropName = "@Desc" + Convert.ToInt16(UtilityClass.Right(propName, 2));
                            if (value != null)
                            {
                                param.Parameters.Add(new SqlParameter(sqlPropName, prop.GetValue(parm) ?? ""));
                            }
                        }
                    }
                    param.Parameters.Add(new SqlParameter("@UserID", parm.UserID ?? ""));
                    DataSet result = await GetDataSet(param, conn);
                    return result;
                }
            }
            catch (Exception ex)
            {
                this.SetError(ex);
                Console.WriteLine($"Error in TableListServiceResponse: {ex.Message}");
                throw;
            }
        }

        //-------Execute Async

        public async Task<bool> ExecuteAsync(ClassProAccessParams parm, string conn = "")
        {
            conn = conn == "Log" ? AddLogRecordConnectionString : "";
            using (SqlConnection sqlCon = new SqlConnection(conn == "" ? ConnectionString : conn))
            {
                DynamicParameters param = new DynamicParameters();
                try
                {
                    ClearErrors();
                    param.Add("@Comp1", parm.Comp1);
                    param.Add("@CallType", parm.Calltype);
                    param.Add("@Dxml01", parm.Dxml01?.GetXml());
                    param.Add("@Dxml02", parm.Dxml02?.GetXml());
                    var props = parm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in props)
                    {
                        string propName = prop.Name;
                        var value = prop.GetValue(parm) as string;
                        if (propName.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                        {
                            string sqlPropName = "@Desc" + Convert.ToInt16(UtilityClass.Right(propName, 2));
                            if (value != null)
                            {
                                param.Add(sqlPropName, prop.GetValue(parm) ?? "");
                            }
                        }
                    }
                    param.Add("@UserID", parm.UserID ?? "");

                    await sqlCon.OpenAsync();
                    await sqlCon.ExecuteAsync(parm.StoredProcedure, param, commandType: CommandType.StoredProcedure, commandTimeout: 0);
                    return true;
                }
                catch (SqlException sqlEx)
                {
                    SetError(sqlEx);
                    throw new DataAccessException("Database error: " + sqlEx.Message, parm.Calltype,
                        parm.StoredProcedure, param, sqlEx);
                }
                catch (TimeoutException timeoutEx)
                {
                    // Handle timeout exceptions
                    SetError(timeoutEx);
                    throw new DataAccessException("Time Out error", parm.Calltype,
                        parm.StoredProcedure, param, timeoutEx);
                    //return false;
                }
                catch (Exception ex)
                {
                    SetError(ex);
                    throw new DataAccessException("Exception error", parm.Calltype,
                        parm.StoredProcedure, param, ex);
                    //return false;
                }
            }
        }

        

        //--------List Async

        public async Task<IEnumerable<T>> ListAsyncDynamic<T>(ClassProAccessParams parm)
        {
            DynamicParameters param = new DynamicParameters();
            try
            {
                await using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {
                    ClearErrors();
                    param.Add("@Comp1", parm.Comp1 ?? "");
                    param.Add("@CallType", parm.Calltype);
                    param.Add("@Dxml01", parm.Dxml01?.GetXml());
                    param.Add("@Dxml02", parm.Dxml02?.GetXml());
                    param.Add("@UserID", parm.UserID ?? "");
                    var props = parm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in props)
                    {
                        string propName = prop.Name;

                        var value = prop.GetValue(parm) as string;
                        if (propName.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                        {
                            string sqlPropName = "@Desc" + Convert.ToInt16(UtilityClass.Right(propName, 2));
                            if (value != null)
                            {
                                param.Add(sqlPropName, prop.GetValue(parm) ?? "");
                            }
                        }
                    }

                    await sqlCon.OpenAsync();
                    return await sqlCon.QueryAsync<T>(parm.StoredProcedure, param, commandType: CommandType.StoredProcedure);

                }
            }
            catch (SqlException sqlEx)
            {
                SetError(sqlEx);
                throw new DataAccessException("Database error", parm.Calltype,
                    parm.StoredProcedure, param, sqlEx);
            }
            catch (TimeoutException timeoutEx)
            {
                // Handle timeout exceptions
                SetError(timeoutEx);
                throw new DataAccessException("Time Out error", parm.Calltype,
                    parm.StoredProcedure, param, timeoutEx);
                //return false;
            }
            catch (Exception ex)
            {
                SetError(ex);
                throw new DataAccessException("Exception error", parm.Calltype,
                    parm.StoredProcedure, param, ex);
                //return false;
            }
        }




        public async Task<IEnumerable<T>> ListAsync<T>(ClassProAccessParams parm)
        {
            DynamicParameters param = new DynamicParameters();
            try
            {
                await using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {
                    ClearErrors();
                    param.Add("@Comp1", parm.Comp1 ?? "");
                    param.Add("@CallType", parm.Calltype);
                    param.Add("@Dxml01", parm.Dxml01?.GetXml());
                    param.Add("@Dxml02", parm.Dxml02?.GetXml());

                    var props = parm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in props)
                    {
                        string propName = prop.Name;

                        var value = prop.GetValue(parm) as string;
                        if (propName.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                        {
                            string sqlPropName = "@Desc" + Convert.ToInt16(UtilityClass.Right(propName, 2));
                            if (value != null)
                            {
                                param.Add(sqlPropName, prop.GetValue(parm) ?? "");
                            }
                        }
                    }
                    await sqlCon.OpenAsync();
                    return await sqlCon.QueryAsync<T>(parm.StoredProcedure, param, commandType: CommandType.StoredProcedure);
                }
            }
            catch (SqlException sqlEx)
            {
                SetError(sqlEx);
                throw new DataAccessException(sqlEx.Message, parm.Calltype,
                    parm.StoredProcedure, param, sqlEx);
            }
            catch (TimeoutException timeoutEx)
            {
                // Handle timeout exceptions
                SetError(timeoutEx);
                throw new DataAccessException(timeoutEx.Message, parm.Calltype,
                    parm.StoredProcedure, param, timeoutEx);
                //return false;
            }
            catch (Exception ex)
            {
                SetError(ex);
                throw new DataAccessException(ex.Message, parm.Calltype,
                    parm.StoredProcedure, param, ex);
                //return false;
            }
        }

        

        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> ListAsync<T1, T2>(ClassProAccessParams parm)
        {
            DynamicParameters param = new DynamicParameters();
            try
            {

                await using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {
                    ClearErrors();
                    param.Add("@Comp1", parm.Comp1 ?? "");
                    param.Add("@CallType", parm.Calltype);
                    param.Add("@Dxml01", parm.Dxml01?.GetXml());
                    param.Add("@Dxml02", parm.Dxml02?.GetXml());
                    param.Add("@UserID", parm.UserID ?? "");
                    var props = parm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in props)
                    {
                        string propName = prop.Name;

                        var value = prop.GetValue(parm) as string;
                        if (propName.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                        {
                            string sqlPropName = "@Desc" + Convert.ToInt16(UtilityClass.Right(propName, 2));
                            if (value != null)
                            {
                                param.Add(sqlPropName, prop.GetValue(parm) ?? "");
                            }
                        }
                    }


                    await sqlCon.OpenAsync();
                    var result = await SqlMapper.QueryMultipleAsync(sqlCon, parm.StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure);
                    var item1 = await result.ReadAsync<T1>() ?? Enumerable.Empty<T1>();
                    var item2 = await result.ReadAsync<T2>() ?? Enumerable.Empty<T2>();
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
                }

            }
            catch (SqlException sqlEx)
            {
                SetError(sqlEx);
                throw new DataAccessException("Database error", parm.Calltype,
                    parm.StoredProcedure, param, sqlEx);
            }
            catch (TimeoutException timeoutEx)
            {
                // Handle timeout exceptions
                SetError(timeoutEx);
                throw new DataAccessException("Time Out error", parm.Calltype,
                    parm.StoredProcedure, param, timeoutEx);
                //return false;
            }
            catch (Exception ex)
            {
                SetError(ex);
                throw new DataAccessException("Exception error", parm.Calltype,
                    parm.StoredProcedure, param, ex);
                //return false;
            }
        }
       
        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>> ListAsync<T1, T2, T3>(ClassProAccessParams parm)
        {
            try
            {

                await using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {
                    ClearErrors();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Comp1", parm.Comp1 ?? "");
                    param.Add("@CallType", parm.Calltype);
                    param.Add("@Dxml01", parm.Dxml01?.GetXml());
                    param.Add("@Dxml02", parm.Dxml02?.GetXml());
                    var props = parm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in props)
                    {
                        string propName = prop.Name;

                        var value = prop.GetValue(parm) as string;
                        if (propName.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                        {
                            string sqlPropName = "@Desc" + Convert.ToInt16(UtilityClass.Right(propName, 2));
                            if (value != null)
                            {
                                param.Add(sqlPropName, prop.GetValue(parm) ?? "");
                            }
                        }
                    }
                    param.Add("@UserID", parm.UserID ?? "");

                    await sqlCon.OpenAsync();
                    var result = await SqlMapper.QueryMultipleAsync(sqlCon, parm.StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure);
                    var item1 = await result.ReadAsync<T1>() ?? Enumerable.Empty<T1>();
                    var item2 = await result.ReadAsync<T2>() ?? Enumerable.Empty<T2>();
                    var item3 = await result.ReadAsync<T3>() ?? Enumerable.Empty<T3>();
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>(item1, item2, item3);
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle database-specific errors
                SetError(sqlEx);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>
                    (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>());// Return empty list to prevent null reference issues
            }
            catch (TimeoutException timeoutEx)
            {
                // Handle timeout exceptions
                SetError(timeoutEx);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>
                   (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>());// Return empty list to prevent null reference issues
            }
            catch (Exception ex)
            {
                SetError(ex);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>>
                   (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>());// Return empty list to prevent null reference issues
            }
        }

        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>> ListAsync<T1, T2, T3, T4>(ClassProAccessParams parm)
        {
            try
            {
                await using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {
                    ClearErrors();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Comp1", parm.Comp1 ?? "");
                    param.Add("@CallType", parm.Calltype);
                    param.Add("@Dxml01", parm.Dxml01?.GetXml());
                    param.Add("@Dxml02", parm.Dxml02?.GetXml());
                    var props = parm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in props)
                    {
                        string propName = prop.Name;

                        var value = prop.GetValue(parm) as string;
                        if (propName.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                        {
                            string sqlPropName = "@Desc" + Convert.ToInt16(UtilityClass.Right(propName, 2));
                            if (value != null)
                            {
                                param.Add(sqlPropName, prop.GetValue(parm) ?? "");
                            }
                        }
                    }
                    param.Add("@UserID", parm.UserID ?? "");

                    await sqlCon.OpenAsync();
                    var result = await SqlMapper.QueryMultipleAsync(sqlCon, parm.StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure);
                    var item1 = await result.ReadAsync<T1>() ?? Enumerable.Empty<T1>();
                    var item2 = await result.ReadAsync<T2>() ?? Enumerable.Empty<T2>();
                    var item3 = await result.ReadAsync<T3>() ?? Enumerable.Empty<T3>();
                    var item4 = await result.ReadAsync<T4>() ?? Enumerable.Empty<T4>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>(item1, item2, item3, item4);
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle database-specific errors
                SetError(sqlEx);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
                    (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>());// Return empty list to prevent null reference issues
            }
            catch (TimeoutException timeoutEx)
            {
                // Handle timeout exceptions
                SetError(timeoutEx);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
                   (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>());// Return empty list to prevent null reference issues
            }
            catch (Exception ex)
            {
                SetError(ex);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>>
                   (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>());// Return empty list to prevent null reference issues
            }
        }

        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>?>> ListAsync<T1, T2, T3, T4, T5>(ClassProAccessParams parm)
        {
            try
            {
                await using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {
                    ClearErrors();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Comp1", parm.Comp1 ?? "");
                    param.Add("@CallType", parm.Calltype);
                    param.Add("@Dxml01", parm.Dxml01?.GetXml());
                    param.Add("@Dxml02", parm.Dxml02?.GetXml());
                    var props = parm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in props)
                    {
                        string propName = prop.Name;

                        var value = prop.GetValue(parm) as string;
                        if (propName.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                        {
                            string sqlPropName = "@Desc" + Convert.ToInt16(UtilityClass.Right(propName, 2));
                            if (value != null)
                            {
                                param.Add(sqlPropName, prop.GetValue(parm) ?? "");
                            }
                        }
                    }
                    param.Add("@UserID", parm.UserID ?? "");

                    await sqlCon.OpenAsync();
                    var result = await SqlMapper.QueryMultipleAsync(sqlCon, parm.StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure);
                    var item1 = await result.ReadAsync<T1>() ?? Enumerable.Empty<T1>();
                    var item2 = await result.ReadAsync<T2>() ?? Enumerable.Empty<T2>();
                    var item3 = await result.ReadAsync<T3>() ?? Enumerable.Empty<T3>();
                    var item4 = await result.ReadAsync<T4>() ?? Enumerable.Empty<T4>();
                    var item5 = result.IsConsumed ? null : await result.ReadAsync<T5>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>?>(item1, item2, item3, item4, item5);
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle database-specific errors
                SetError(sqlEx);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>?>
                    (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>());// Return empty list to prevent null reference issues
            }
            catch (TimeoutException timeoutEx)
            {
                // Handle timeout exceptions
                SetError(timeoutEx);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>?>
                   (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>());// Return empty list to prevent null reference issues
            }
            catch (Exception ex)
            {
                SetError(ex);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>?>
                   (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>());// Return empty list to prevent null reference issues
            }
        }

        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>> ListAsync<T1, T2, T3, T4, T5, T6>(ClassProAccessParams parm)
        {
            try
            {
                await using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {
                    ClearErrors();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Comp1", parm.Comp1 ?? "");
                    param.Add("@CallType", parm.Calltype);
                    param.Add("@Dxml01", parm.Dxml01?.GetXml());
                    param.Add("@Dxml02", parm.Dxml02?.GetXml());
                    var props = parm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in props)
                    {
                        string propName = prop.Name;

                        var value = prop.GetValue(parm) as string;
                        if (propName.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                        {
                            string sqlPropName = "@Desc" + Convert.ToInt16(UtilityClass.Right(propName, 2));
                            if (value != null)
                            {
                                param.Add(sqlPropName, prop.GetValue(parm) ?? "");
                            }
                        }
                    }

                    param.Add("@UserID", parm.UserID ?? "");

                    await sqlCon.OpenAsync();
                    var result = await SqlMapper.QueryMultipleAsync(sqlCon, parm.StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure);
                    var item1 = await result.ReadAsync<T1>() ?? Enumerable.Empty<T1>();
                    var item2 = await result.ReadAsync<T2>() ?? Enumerable.Empty<T2>();
                    var item3 = await result.ReadAsync<T3>() ?? Enumerable.Empty<T3>();
                    var item4 = await result.ReadAsync<T4>() ?? Enumerable.Empty<T4>();
                    var item5 = await result.ReadAsync<T5>() ?? Enumerable.Empty<T5>();
                    var item6 = await result.ReadAsync<T6>() ?? Enumerable.Empty<T6>();
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>(item1, item2, item3, item4, item5, item6);
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle database-specific errors
                SetError(sqlEx);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>
                    (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>(), Enumerable.Empty<T6>());// Return empty list to prevent null reference issues
            }
            catch (TimeoutException timeoutEx)
            {
                // Handle timeout exceptions
                SetError(timeoutEx);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>
                    (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>(), Enumerable.Empty<T6>());// Return empty list to prevent null reference issues
            }
            catch (Exception ex)
            {
                SetError(ex);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>>
                    (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>(), Enumerable.Empty<T6>());// Return empty list to prevent null reference issues
            }
        }

        public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>> ListAsync<T1, T2, T3, T4, T5, T6, T7>(ClassProAccessParams parm)
        {
            try
            {
                await using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {
                    ClearErrors();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Comp1", parm.Comp1 ?? "");
                    param.Add("@CallType", parm.Calltype);
                    param.Add("@Dxml01", parm.Dxml01?.GetXml());
                    param.Add("@Dxml02", parm.Dxml02?.GetXml());
                    var props = parm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in props)
                    {
                        string propName = prop.Name;

                        var value = prop.GetValue(parm) as string;
                        if (propName.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                        {
                            string sqlPropName = "@Desc" + Convert.ToInt16(UtilityClass.Right(propName, 2));
                            if (value != null)
                            {
                                param.Add(sqlPropName, prop.GetValue(parm) ?? "");
                            }
                        }
                    }

                    param.Add("@UserID", parm.UserID ?? "");

                    await sqlCon.OpenAsync();
                    var result = await SqlMapper.QueryMultipleAsync(sqlCon, parm.StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure);
                    var item1 = await result.ReadAsync<T1>() ?? Enumerable.Empty<T1>();
                    var item2 = await result.ReadAsync<T2>() ?? Enumerable.Empty<T2>();
                    var item3 = await result.ReadAsync<T3>() ?? Enumerable.Empty<T3>();
                    var item4 = await result.ReadAsync<T4>() ?? Enumerable.Empty<T4>();
                    var item5 = await result.ReadAsync<T5>() ?? Enumerable.Empty<T5>();
                    var item6 = await result.ReadAsync<T6>() ?? Enumerable.Empty<T6>();
                    var item7 = await result.ReadAsync<T7>() ?? Enumerable.Empty<T7>();

                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>(item1, item2, item3, item4, item5, item6, item7);
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle database-specific errors
                SetError(sqlEx);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>
                    (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>(), Enumerable.Empty<T6>(), Enumerable.Empty<T7>());// Return empty list to prevent null reference issues
            }
            catch (TimeoutException timeoutEx)
            {
                // Handle timeout exceptions
                SetError(timeoutEx);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>
                    (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>(), Enumerable.Empty<T6>(), Enumerable.Empty<T7>());// Return empty list to prevent null reference issues
            }
            catch (Exception ex)
            {
                SetError(ex);
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>>
                    (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>(), Enumerable.Empty<T6>(), Enumerable.Empty<T7>());// Return empty list to prevent null reference issues
            }
        }

        public async Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>, IEnumerable<T8>, IEnumerable<T9>)> ListAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ClassProAccessParams parm)
        {
            try
            {
                await using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {
                    ClearErrors();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Comp1", parm.Comp1 ?? "");
                    param.Add("@CallType", parm.Calltype);
                    param.Add("@Dxml01", parm.Dxml01?.GetXml());
                    param.Add("@Dxml02", parm.Dxml02?.GetXml());
                    var props = parm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in props)
                    {
                        string propName = prop.Name;

                        var value = prop.GetValue(parm) as string;
                        if (propName.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                        {
                            string sqlPropName = "@Desc" + Convert.ToInt16(UtilityClass.Right(propName, 2));
                            if (value != null)
                            {
                                param.Add(sqlPropName, prop.GetValue(parm) ?? "");
                            }
                        }
                    }

                    param.Add("@UserID", parm.UserID ?? "");

                    await sqlCon.OpenAsync();
                    var result = await SqlMapper.QueryMultipleAsync(sqlCon, parm.StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure);
                    var item1 = await result.ReadAsync<T1>() ?? Enumerable.Empty<T1>();
                    var item2 = await result.ReadAsync<T2>() ?? Enumerable.Empty<T2>();
                    var item3 = await result.ReadAsync<T3>() ?? Enumerable.Empty<T3>();
                    var item4 = await result.ReadAsync<T4>() ?? Enumerable.Empty<T4>();
                    var item5 = await result.ReadAsync<T5>() ?? Enumerable.Empty<T5>();
                    var item6 = await result.ReadAsync<T6>() ?? Enumerable.Empty<T6>();
                    var item7 = await result.ReadAsync<T7>() ?? Enumerable.Empty<T7>();
                    var item8 = await result.ReadAsync<T8>() ?? Enumerable.Empty<T8>();
                    var item9 = await result.ReadAsync<T9>() ?? Enumerable.Empty<T9>();

                    return (item1, item2, item3, item4, item5, item6, item7, item8, item9);
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle database-specific errors
                SetError(sqlEx);
                return (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>(), Enumerable.Empty<T6>(), Enumerable.Empty<T7>(), Enumerable.Empty<T8>(), Enumerable.Empty<T9>());
            }
            catch (TimeoutException timeoutEx)
            {
                // Handle timeout exceptions
                SetError(timeoutEx);
                return (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>(), Enumerable.Empty<T6>(), Enumerable.Empty<T7>(), Enumerable.Empty<T8>(), Enumerable.Empty<T9>());
            }
            catch (Exception ex)
            {
                SetError(ex);
                return (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>(), Enumerable.Empty<T6>(), Enumerable.Empty<T7>(), Enumerable.Empty<T8>(), Enumerable.Empty<T9>());
            }
        }
        public async Task<(IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>, IEnumerable<T5>, IEnumerable<T6>, IEnumerable<T7>, IEnumerable<T8>, IEnumerable<T9>, IEnumerable<T10>)> ListAsync<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ClassProAccessParams parm)
        {
            try
            {
                await using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {
                    ClearErrors();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Comp1", parm.Comp1 ?? "");
                    param.Add("@CallType", parm.Calltype);
                    param.Add("@Dxml01", parm.Dxml01?.GetXml());
                    param.Add("@Dxml02", parm.Dxml02?.GetXml());
                    var props = parm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in props)
                    {
                        string propName = prop.Name;

                        var value = prop.GetValue(parm) as string;
                        if (propName.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                        {
                            string sqlPropName = "@Desc" + Convert.ToInt16(UtilityClass.Right(propName, 2));
                            if (value != null)
                            {
                                param.Add(sqlPropName, prop.GetValue(parm) ?? "");
                            }
                        }
                    }

                    param.Add("@UserID", parm.UserID ?? "");

                    await sqlCon.OpenAsync();
                    var result = await SqlMapper.QueryMultipleAsync(sqlCon, parm.StoredProcedure, param, commandType: System.Data.CommandType.StoredProcedure);
                    var item1 = await result.ReadAsync<T1>() ?? Enumerable.Empty<T1>();
                    var item2 = await result.ReadAsync<T2>() ?? Enumerable.Empty<T2>();
                    var item3 = await result.ReadAsync<T3>() ?? Enumerable.Empty<T3>();
                    var item4 = await result.ReadAsync<T4>() ?? Enumerable.Empty<T4>();
                    var item5 = await result.ReadAsync<T5>() ?? Enumerable.Empty<T5>();
                    var item6 = await result.ReadAsync<T6>() ?? Enumerable.Empty<T6>();
                    var item7 = await result.ReadAsync<T7>() ?? Enumerable.Empty<T7>();
                    var item8 = await result.ReadAsync<T8>() ?? Enumerable.Empty<T8>();
                    var item9 = await result.ReadAsync<T9>() ?? Enumerable.Empty<T9>();
                    var item10 = await result.ReadAsync<T10>() ?? Enumerable.Empty<T10>();

                    return (item1, item2, item3, item4, item5, item6, item7, item8, item9, item10);
                }
            }
            catch (SqlException sqlEx)
            {
                // Handle database-specific errors
                SetError(sqlEx);
                return (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>(), Enumerable.Empty<T6>(), Enumerable.Empty<T7>(), Enumerable.Empty<T8>(), Enumerable.Empty<T9>(), Enumerable.Empty<T10>());
            }
            catch (TimeoutException timeoutEx)
            {
                // Handle timeout exceptions
                SetError(timeoutEx);
                return (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>(), Enumerable.Empty<T6>(), Enumerable.Empty<T7>(), Enumerable.Empty<T8>(), Enumerable.Empty<T9>(), Enumerable.Empty<T10>());
            }
            catch (Exception ex)
            {
                SetError(ex);
                return (Enumerable.Empty<T1>(), Enumerable.Empty<T2>(), Enumerable.Empty<T3>(), Enumerable.Empty<T4>(), Enumerable.Empty<T5>(), Enumerable.Empty<T6>(), Enumerable.Empty<T7>(), Enumerable.Empty<T8>(), Enumerable.Empty<T9>(), Enumerable.Empty<T10>());
            }
        }
        public async Task<T> FirstRowAsync<T>(ClassProAccessParams parm)
        {
            try
            {

                await using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
                {
                    ClearErrors();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Comp1", parm.Comp1 ?? "");
                    param.Add("@CallType", parm.Calltype);
                    param.Add("@Dxml01", parm.Dxml01?.GetXml());
                    param.Add("@Dxml02", parm.Dxml02?.GetXml());
                    var props = parm.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var prop in props)
                    {
                        string propName = prop.Name;

                        var value = prop.GetValue(parm) as string;
                        if (propName.StartsWith("Desc", StringComparison.OrdinalIgnoreCase))
                        {
                            string sqlPropName = "@Desc" + Convert.ToInt16(UtilityClass.Right(propName, 2));
                            if (value != null)
                            {
                                param.Add(sqlPropName, prop.GetValue(parm) ?? "");
                            }
                        }
                    }
                    param.Add("@UserID", parm.UserID ?? "");

                    await sqlCon.OpenAsync();
                    return await sqlCon.QuerySingleOrDefaultAsync<T>(parm.StoredProcedure, param, commandType: CommandType.StoredProcedure);

                }
            }
            catch (SqlException sqlEx)
            {
                // Handle database-specific errors
                SetError(sqlEx);
                return default!;
            }
            catch (TimeoutException timeoutEx)
            {
                // Handle timeout exceptions
                SetError(timeoutEx);
                return default!;
            }
            catch (Exception ex)
            {
                SetError(ex);
                return default!;
            }
        }


    }
}
