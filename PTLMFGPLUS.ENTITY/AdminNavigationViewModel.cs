using System.Collections.Generic;

namespace PTLMFGPLUS.ENTITY
{
    public class AdminNavigationViewModel
    {
        public List<EUserModule> Modules { get; set; } = new();
        public List<AdminNavigationItem> Interfaces { get; set; } = new();
        public List<AdminNavigationItem> Dashboards { get; set; } = new();
        public List<AdminNavigationItem> Analysis { get; set; } = new();
        public List<AdminNavigationItem> Shortcuts { get; set; } = new();
        public AdminNavigationItem AllInOne { get; set; }
    }

    public class AdminNavigationItem
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
    }
}
