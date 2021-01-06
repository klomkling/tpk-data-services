using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class CustomerOrderLineReference : TgMinimalModelBase
    {
        public int CustomerOrderLineId { get; set; }
        public int InventoryRequestLineId { get; set; }
    }
}