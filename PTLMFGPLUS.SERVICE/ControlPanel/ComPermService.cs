
using PTLMFGPLUS.ENTITY;
using PTLMFGPLUS.LIB;
using PTLMFGPLUS.LIB.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.ControlPanel.ECompanyPage;

namespace PTLMFGPLUS.SERVICE.ControlPanel
{
    public interface Icomperm
    {
        Exception? GetError();
        Task<DataSet> ShowUserInfo(string filtername = "%%");     
        Task<DataSet> ShowPagePer();
        Task<IEnumerable<ECompanyPagePermission>> ShowData(string moduleid);
        Task<IEnumerable<ECompanyPagePermission>> ShowAllData(string moduleid);
        Task<bool> SavePermissions(SaveCompanyPagePermissionRequest request);

    }


    public class compermService(ICommonService common, IUnitOfWork unitOfWork, IMenuService _menuService) : Icomperm
    {
        public Exception? GetError() => unitOfWork.SP_Call.GetError();

        public async Task<DataSet> ShowUserInfo(string filtername = "%%")
        {
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_UTILITY_LOGIN_MGT";
            parms.Calltype = "SHOWUSER";
            parms.Comp1 = common.GetComcod();
            parms.Desc01 = "%" + filtername + "%";
            var items = await unitOfWork.SP_Call.DataSetAsync(parms);
            return items;
        }
        public async Task<DataSet> ShowPagePer()
        {
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_UTILITY_LOGIN_MGT";
            parms.Calltype = "SHOWUSERPERFORMASITUSER";
            parms.Comp1 = common.GetComcod();
            var items = await unitOfWork.SP_Call.DataSetAsync(parms);

            return items;
        }


        public async Task<IEnumerable<ECompanyPagePermission>> ShowData(string moduleid)
        {
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_UTILITY_LOGIN_MGT";
            parms.Calltype = "SHOWUSERPERFORMASITUSER";
            parms.Comp1 = common.GetComcod();
            var items = await unitOfWork.SP_Call.ListAsync<ECompanyPagePermission>(parms);
            return moduleid == "AA"
                ? items.OrderBy(x => x.frmid)
                : items.Where(x => x.frmid.StartsWith(moduleid, StringComparison.Ordinal)).OrderBy(x => x.frmid);
          

        }

        public async Task<IEnumerable<ECompanyPagePermission>> ShowAllData(string moduleid)
        {
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_UTILITY_LOGIN_MGT";
            parms.Calltype = "SHOWUSERPERFORMASITUSER";
            parms.Comp1 = common.GetComcod();
            var lst = await unitOfWork.SP_Call.ListAsync<ECompanyPagePermission>(parms);           
            List<ECompForm> lstconsinfo = ConstantInfo.GetPermissionScope();
            List<ECompanyPagePermission> lstall = new List<ECompanyPagePermission>();
            List<ECompForm> lstconsinfo1 = new List<ECompForm>();
            IEnumerable<EUserModule> lstmodule = await _menuService.GetModules();


            if (moduleid == "%")
            {    
                lstall = (from p in lstconsinfo
                            join t in lstmodule on p.frmid.Substring(0,2) equals t.moduleid
                            select new ECompanyPagePermission
                            {
                                frmgrp = p.frmgrp,
                                frmid = p.frmid,
                                pagelink = p.parea + "/" + p.pcontroller + "/" + p.paction,
                                qrytype = p.qrytype,
                                dscrption = p.frmname,
                                chkper = p.chkper,
                                modulename = t.modulename,
                                fmgrpdesc = "",
                                alias = p.frmname
                            }).ToList();
            }
            else
            {

             var lstcall = lstconsinfo.FindAll(l => l.frmid.Substring(0, 2) == moduleid.Substring(0,2));
                lstall = (from p in lstcall
                            join t in lstmodule on p.frmid.Substring(0, 2) equals t.moduleid
                            select new ECompanyPagePermission
                            {
                                frmgrp = p.frmgrp,
                                frmid = p.frmid,
                                pagelink = p.parea + "/" + p.pcontroller + "/" + p.paction,
                                qrytype = p.qrytype,
                                dscrption = p.frmname,
                                chkper = p.chkper,
                                modulename = t.modulename,
                                fmgrpdesc = "",
                                alias = p.frmname
                            }).ToList();

            }





            foreach (var lstitem in lstall)
            {
                string frmgrp = lstitem.frmgrp;
                lstitem.fmgrpdesc= (frmgrp.Substring(2, 2) == "01") ? "One Time Input" : (frmgrp.Substring(2, 2) == "02") ? "Entry"
                    : (frmgrp.Substring(2, 2) == "51") ? "Interface" : (frmgrp.Substring(2, 2) == "91") || (frmgrp.Substring(2, 2) == "61") || (frmgrp.Substring(2, 2) == "62") ? "Dashboard" : "Reports";
            }



            //left join 
            var lstfinal = (from a in lstall
                            join b in lst on new { a.frmid, a.qrytype } equals new { b.frmid, b.qrytype } into gj
                            from b in gj.DefaultIfEmpty()
                            select new ECompanyPagePermission
                            {
                                frmgrp = a.frmgrp,
                                frmid = a.frmid,
                                pagelink = a.pagelink,
                                qrytype = a.qrytype,
                                dscrption = a.dscrption,
                                chkper = b?.chkper ?? a.chkper,
                                modulename = a.modulename,
                                fmgrpdesc = a.fmgrpdesc,
                                alias = string.IsNullOrWhiteSpace(b?.alias) ? a.alias : b.alias,
                                entry = b?.entry ?? false,
                                printable = b?.printable ?? false,
                                deleteck = b?.deleteck ?? false

                            }).ToList().OrderBy(l=>l.frmid);

        


            return lstfinal;

            

        }

        public async Task<bool> SavePermissions(SaveCompanyPagePermissionRequest request)
        {
            string modulePrefix = request.moduleId == "AA" ? "" : request.moduleId;
            List<ECompForm> allowedPages = ConstantInfo.GetPermissionScope()
                .Where(x => modulePrefix.Length == 0 || x.frmid.StartsWith(modulePrefix, StringComparison.Ordinal))
                .ToList();
            var allowedKeys = allowedPages.Select(x => $"{x.frmid}|{x.qrytype}")
                .ToHashSet(StringComparer.Ordinal);
            var selected = request.permissions
                .Where(x => x.chkper && allowedKeys.Contains($"{x.frmid}|{x.qrytype}"))
                .GroupBy(x => $"{x.frmid}|{x.qrytype}", StringComparer.Ordinal)
                .Select(x => x.First())
                .ToList();

            var delete = new ClassProAccessParams
            {
                Comp1 = common.GetComcod(),
                StoredProcedure = "SP_UTILITY_LOGIN_MGT",
                Calltype = "DELETECOMP",
                Desc01 = common.GetComcod() + "001",
                Desc02 = modulePrefix + "%",
                UserID = common.GetUserId()
            };
            if (!await unitOfWork.SP_Call.ExecuteAsync(delete))
            {
                return false;
            }
            if (selected.Count == 0)
            {
                return true;
            }

            DataTable table = new DataTable("tbl1");
            table.Columns.Add("frmid1", typeof(string));
            table.Columns.Add("frmid", typeof(string));
            table.Columns.Add("frmname", typeof(string));
            table.Columns.Add("qrytype", typeof(string));
            table.Columns.Add("frmdesc", typeof(string));
            table.Columns.Add("dscrption", typeof(string));
            table.Columns.Add("modulename", typeof(string));
            table.Columns.Add("chkper", typeof(bool));
            table.Columns.Add("entry", typeof(bool));
            table.Columns.Add("printable", typeof(bool));
            table.Columns.Add("deleteck", typeof(bool));
            table.Columns.Add("alias", typeof(string));

            foreach (CompanyPagePermissionRow row in selected)
            {
                ECompForm page = allowedPages.First(x => x.frmid == row.frmid && x.qrytype == row.qrytype);
                table.Rows.Add(page.frmgrp, page.frmid, page.frmname, page.qrytype, "", page.frmname, "", true,
                    page.entry, page.printable, false, string.IsNullOrWhiteSpace(row.alias) ? page.frmname : row.alias.Trim());
            }

            DataSet data = new DataSet("ds1");
            data.Tables.Add(table);
            var insert = new ClassProAccessParams
            {
                Comp1 = common.GetComcod(),
                StoredProcedure = "SP_UTILITY_LOGIN_MGT",
                Calltype = "INSERTCOMPPER",
                Dxml01 = data,
                UserID = common.GetUserId()
            };
            return await unitOfWork.SP_Call.ExecuteAsync(insert);
        }
    }




}



