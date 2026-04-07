using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTLMFGPLUS.ENTITY
{
    public class EMenuTable
    {
        public string itemcod { get; set; }
        public string itemdesc { get; set; }
        public string parea { get; set; }
        public string pcontroller { get; set; }
        public string paction { get; set; }
        public string proutes { get; set; }
        public string itemtips { get; set; }
        public bool itemslct { get; set; }
        public string fbold { get; set; }
        public string formid { get; set; }

        public EMenuTable()
        {
        }
        public EMenuTable(string itemcod, string itemdesc, string parea, string pcontroller, string paction, string proutes, string itemtips, bool itemslct, string fbold)
        {
            this.itemcod = itemcod;
            this.itemdesc = itemdesc;
            this.parea = parea;
            this.pcontroller = pcontroller;
            this.paction = paction;
            this.proutes = proutes;
            this.itemtips = itemtips;
            this.itemslct = itemslct;
            this.fbold = fbold;
        }

        public EMenuTable(string itemcod, string itemdesc, string parea, string pcontroller, string paction, string proutes, string itemtips, bool itemslct, string fbold, string formid)
        {
            this.itemcod = itemcod;
            this.itemdesc = itemdesc;
            this.parea = parea;
            this.pcontroller = pcontroller;
            this.paction = paction;
            this.proutes = proutes;
            this.itemtips = itemtips;
            this.itemslct = itemslct;
            this.fbold = fbold;
            this.formid = formid;
        }
    }
}
