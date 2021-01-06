using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class SectionEditContext : EditContextBase<Section>
    {
        public SectionEditContext() : base(null)
        {
        }

        public SectionEditContext(Section dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Name = DataItem.Name;
            DepartmentId = DataItem.DepartmentId;
        }

        public string Name { get; set; }
        public int DepartmentId { get; set; }
    }
}