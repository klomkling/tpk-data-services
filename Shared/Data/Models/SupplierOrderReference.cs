using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class SupplierOrderReference : TgMinimalModelBase
    {
        public int SupplierOrderId { get; set; }
        public int InventoryRequestId { get; set; }
    }
}