using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class ProductUnitEditContext : EditContextBase<ProductUnit>
    {
        public ProductUnitEditContext() : base(null)
        {
        }

        public ProductUnitEditContext(ProductUnit dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Code = DataItem.Code;
            Name = DataItem.Name;
            IsPermanent = DataItem.IsPermanent;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsPermanent { get; set; }
    }
}