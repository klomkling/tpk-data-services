using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class CustomerProductPackageEditContext : EditContextBase<CustomerProductPackage>
    {
        public CustomerProductPackageEditContext() : base(null)
        {
        }
        
        public CustomerProductPackageEditContext(CustomerProductPackage dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            CustomerProductId = DataItem.CustomerProductId;
            PackageTypeId = DataItem.PackageTypeId;
            Quantity = DataItem.Quantity;
            NetWeight = DataItem.NetWeight;
            GrossWeight = DataItem.GrossWeight;
            PackagePerLayerOnPallet = DataItem.PackagePerLayerOnPallet;
            MaximumLayerOnPallet = DataItem.MaximumLayerOnPallet;
            Remark = DataItem.Remark;
        }
        
        public int CustomerProductId { get; set; }
        public int PackageTypeId { get; set; }
        public decimal Quantity { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public int PackagePerLayerOnPallet { get; set; }
        public int MaximumLayerOnPallet { get; set; }
        public string Remark { get; set; }
    }
}