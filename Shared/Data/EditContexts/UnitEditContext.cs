using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class UnitEditContext : EditContextBase<Unit>
    {
        public UnitEditContext() : base(null)
        {
        }

        public UnitEditContext(Unit dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Name = DataItem.Name;
            SectionId = DataItem.SectionId;
        }

        public string Name { get; set; }
        public int SectionId { get; set; }
    }
}