
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
        Task<IEnumerable<ECompanyPagePermission>> ShowData();
        Task<IEnumerable<ECompanyPagePermission>> ShowAllData(string moduleid);

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


        public async Task<IEnumerable<ECompanyPagePermission>> ShowData()
        {
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_UTILITY_COMPAUSER_PERM";
            parms.Calltype = "SHOWCOMPANYPAGEPERMISSION";
            parms.Comp1 = common.GetComcod();
            var items = await unitOfWork.SP_Call.ListAsync<ECompanyPagePermission>(parms);
            return items;
          

        }

        public async Task<IEnumerable<ECompanyPagePermission>> ShowAllData(string moduleid)
        {
            ClassProAccessParams parms = new ClassProAccessParams();
            parms.StoredProcedure = "SP_UTILITY_COMPAUSER_PERM";
            parms.Calltype = "SHOWCOMPANYPAGEPERMISSION";
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
                                fmgrpdesc = ""
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
                                fmgrpdesc = ""
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
                            join b in lst on a.frmid equals b.frmid into gj
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
                                fmgrpdesc = a.fmgrpdesc

                            }).ToList().OrderBy(l=>l.frmid);

        


            return lstfinal;

            

        }
    }




}



