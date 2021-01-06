using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class SupplierProductPackage : TgModelBase
    {
        public int SupplierProductId { get; set; }
        public int PackageTypeId { get; set; }
        public decimal Quantity { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public string Remark { get; set; }
    }
}