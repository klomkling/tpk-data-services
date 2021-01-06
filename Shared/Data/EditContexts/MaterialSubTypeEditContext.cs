using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class MaterialSubTypeEditContext : EditContextBase<MaterialSubType>
    {
        public MaterialSubTypeEditContext() : base(null)
        {
        }

        public MaterialSubTypeEditContext(MaterialSubType dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Code = DataItem.Code;
            Name = DataItem.Name;
        }

        public string Code { get; set; }
        public string Name { get; set; }
    }
}