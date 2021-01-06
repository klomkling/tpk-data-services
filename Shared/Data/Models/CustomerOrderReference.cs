using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class CustomerOrderReference : TgMinimalModelBase
    {
        public int CustomerOrderId { get; set; }
        public int InventoryRequestId { get; set; }
    }
}