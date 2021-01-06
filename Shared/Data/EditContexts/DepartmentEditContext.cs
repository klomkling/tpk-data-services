using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class DepartmentEditContext : EditContextBase<Department>
    {
        public DepartmentEditContext() : base(null)
        {
        }

        public DepartmentEditContext(Department dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Name = DataItem.Name;
        }

        public string Name { get; set; }
    }
}