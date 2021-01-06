using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class SupplierOrderWithDetail : SupplierOrder
    {
        public string StatusName { get; set; }
        public int? InventoryRequestNumber { get; set; }
        public bool CanGenerate { get; set; }
    }
}