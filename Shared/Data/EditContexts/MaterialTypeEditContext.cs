using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class MaterialTypeEditContext : EditContextBase<MaterialType>
    {
        public MaterialTypeEditContext() : base(null)
        {
        }

        public MaterialTypeEditContext(MaterialType dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Code = DataItem.Code;
            Name = DataItem.Name;
        }

        public string Code { get; set; }
        public string Name { get; set; }
    }
}