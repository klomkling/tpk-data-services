using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class CustomerOrderWithDetail : CustomerOrder
    {
        public string StatusName { get; set; }
        public int InventoryRequestNumber { get; set; }
        public bool CanGenerate { get; set; }
        public string CustomerName { get; set; }
    }
}