using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class ProductCategoryEditContext : EditContextBase<ProductCategory>
    {
        public ProductCategoryEditContext() : base(null)
        {
        }

        public ProductCategoryEditContext(ProductCategory dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Name = DataItem.Name;
        }

        public string Name { get; set; }
    }
}