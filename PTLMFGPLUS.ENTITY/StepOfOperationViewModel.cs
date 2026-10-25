using System.Collections.Generic;

namespace PTLMFGPLUS.ENTITY
{
    public class StepOfOperationViewModel
    {
        public string SelectedModuleId { get; set; }
        public string SelectedModuleName { get; set; }
        public string CompanyName { get; set; }
        public List<EUserModule> Modules { get; set; } = new();
        public List<StepOfOperationSection> Sections { get; set; } = new();
    }

    public class StepOfOperationSection
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Icon { get; set; }
        public List<StepOfOperationItem> Items { get; set; } = new();
    }

    public class StepOfOperationItem
    {
        public string Code { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string FormId { get; set; }
        public bool IsTemplate { get; set; }
    }
}
