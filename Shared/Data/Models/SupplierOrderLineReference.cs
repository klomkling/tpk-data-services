using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class SupplierOrderLineReference : TgMinimalModelBase
    {
        public int SupplierOrderLineId { get; set; }
        public int InventoryRequestLineId { get; set; }
    }
}