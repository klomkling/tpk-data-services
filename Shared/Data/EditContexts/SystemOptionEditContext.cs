using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class SystemOptionEditContext : EditContextBase<SystemOption>
    {
        public SystemOptionEditContext() : base(null)
        {
        }

        public SystemOptionEditContext(SystemOption dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Name = DataItem.Name;
            Value = DataItem.Value;
        }

        public string Name { get; set; }
        public string Value { get; set; }
    }
}