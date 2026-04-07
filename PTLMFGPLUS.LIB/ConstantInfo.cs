using PTLMFGPLUS.ENTITY;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PTLMFGPLUS.ENTITY.ControlPanel.ECompanyPage;
using static PTLMFGPLUS.ENTITY.EnumHelpers;

namespace PTLMFGPLUS.LIB
{
    public class ConstantInfo
    {
        public static List<ECompForm> GetPermissionScope()
        {
            List<ECompForm> pagedata = new List<ECompForm>();

            #region GENERAL

            #region Control Panel

            pagedata.Add(new ECompForm { frmgrp = "3202000", frmid = "3502001", frmname = "Budget-Engineering", parea = "BudgetaryControl", pcontroller = "BudgetEngineering", paction = "BudgetEngineering", qrytype = "InputType=BgdSub" });
            pagedata.Add(new ECompForm { frmgrp = "3503000", frmid = "3503995", frmname = "Company Page Permission", parea = "ControlPanel", pcontroller = "ComPermission", paction = "ComPermissionIndex", qrytype = "" });
            pagedata.Add(new ECompForm { frmgrp = "3503000", frmid = "3503998", frmname = "User Page Permission", parea = "ControlPanel", pcontroller = "UserPerm", paction = "UserPermIndex", qrytype = "" });
            pagedata.Add(new ECompForm { frmgrp = "3503000", frmid = "3503346", frmname = "Company Image Upload", parea = "ControlPanel", pcontroller = "CompImg", paction = "CompImgIndex", qrytype = "" });


            #endregion


            #endregion


            return pagedata;
        }
        #region Menu Table
        public static List<EMenuTable> AllMenuTable(ModuleList moduleId)
        {
            List<EMenuTable> menuTables = new List<EMenuTable>();

            switch (moduleId)
            {
                case ModuleList.BudgetPlan:
                    BudgetPlan(menuTables);
                    break;

                case ModuleList.StdCost:
                    StdCost(menuTables);
                    break;

                case ModuleList.RawMatInv:
                    RawMatInv(menuTables);
                    break;

                case ModuleList.LC:
                    LC(menuTables);
                    break;

                case ModuleList.Pro:
                    Pro(menuTables);
                    break;

                case ModuleList.ProdMon:
                    ProdMon(menuTables);
                    break;

                case ModuleList.Account:
                    Account(menuTables);
                    break;

                case ModuleList.Export:
                    Export(menuTables);
                    break;

                case ModuleList.Audit:
                    Audit(menuTables);
                    break;

                case ModuleList.FGInv:
                    FGInv(menuTables);
                    break;

                case ModuleList.Service:
                    Service(menuTables);
                    break;

                case ModuleList.Sales:
                    Sales(menuTables);
                    break;

                case ModuleList.SalesExport:
                    SalesExport(menuTables);
                    break;

                case ModuleList.FixedAsset:
                    FixedAsset(menuTables);
                    break;

                case ModuleList.MIS:
                    MIS(menuTables);
                    break;

                case ModuleList.KPI:
                    KPI(menuTables);
                    break;

                case ModuleList.Doc:
                    Doc(menuTables);
                    break;

                case ModuleList.MgtAccount:
                    MgtAccount(menuTables);
                    break;

                case ModuleList.Mgt:
                    Mgt(menuTables);
                    break;

                case ModuleList.CRM:
                    CRM(menuTables);
                    break;

                case ModuleList.HR:
                    HR(menuTables);
                    break;
            }

            return menuTables;
        }
        #endregion

        #region Module Wise Menu Methods

        public static List<EMenuTable> BudgetPlan(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> StdCost(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> RawMatInv(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> LC(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> Pro(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> ProdMon(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> Account(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> Export(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> Audit(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> FGInv(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> Service(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> Sales(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> SalesExport(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> FixedAsset(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> MIS(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> KPI(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> Doc(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> MgtAccount(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> Mgt(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> CRM(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        public static List<EMenuTable> HR(List<EMenuTable> menuTables)
        {
            menuTables.Add(new EMenuTable("0201000000", "Code Book", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0301000000", "Entry", "", "", "", "", "", false, "header"));
            menuTables.Add(new EMenuTable("0401000000", "Report", "", "", "", "", "", false, "header"));
            return menuTables;
        }

        #endregion

    }
}
