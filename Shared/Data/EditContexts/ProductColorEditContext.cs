using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class ProductColorEditContext : EditContextBase<ProductColor>
    {
        public ProductColorEditContext() : base(null)
        {
        }

        public ProductColorEditContext(ProductColor dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Code = DataItem.Code;
            Description = DataItem.Description;
        }

        public string Code { get; set; }
        public string Description { get; set; }
    }
}