using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class ProductLineEditContext : EditContextBase<ProductLine>
    {
        public ProductLineEditContext() : base(null)
        {
        }

        public ProductLineEditContext(ProductLine dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Name = DataItem.Name;
        }

        public string Name { get; set; }
    }
}