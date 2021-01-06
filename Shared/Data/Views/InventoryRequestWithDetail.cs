using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class InventoryRequestWithDetail : InventoryRequest
    {
        public string RequestTypeName { get; set; }
        public string StatusName { get; set; }
        public int CustomerOrderId { get; set; }
        public int CustomerId { get; set; }
        public int SupplierOrderId { get; set; }
        public int SupplierId { get; set; }
        public bool CanGenerate { get; set; }
    }
}