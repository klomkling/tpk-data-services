using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class SupplierProductPackageEditContext : EditContextBase<SupplierProductPackage>
    {
        public SupplierProductPackageEditContext() : base(null)
        {
        }

        public SupplierProductPackageEditContext(SupplierProductPackage dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            SupplierProductId = DataItem.SupplierProductId;
            PackageTypeId = DataItem.PackageTypeId;
            Quantity = DataItem.Quantity;
            NetWeight = DataItem.NetWeight;
            GrossWeight = DataItem.GrossWeight;
            Remark = DataItem.Remark;
        }

        public int SupplierProductId { get; set; }
        public int PackageTypeId { get; set; }
        public decimal Quantity { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string Remark { get; set; }
    }
}