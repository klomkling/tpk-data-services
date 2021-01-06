using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class SupplierProduct : TgModelBase
    {
        public int SupplierId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? ProductId { get; set; }
        public int ProductUnitId { get; set; }
        public decimal NormalPrice { get; set; }
        public decimal MoqPrice { get; set; }
        public bool IsIncludedVat { get; set; }
        public decimal MinimumOrder { get; set; }
        public bool IsActive { get; set; }
    }
}