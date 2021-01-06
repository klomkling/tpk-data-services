using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class ProductPackage : TgModelBase
    {
        public int ProductId { get; set; }
        public int PackageTypeId { get; set; }
        public decimal Quantity { get; set; }
        public decimal NetWeight { get; set; }
        public decimal GrossWeight { get; set; }
        public int PackagePerLayerOnPallet { get; set; }
        public int MaximumLayerOnPallet { get; set; }
        public string Remark { get; set; }
    }
}