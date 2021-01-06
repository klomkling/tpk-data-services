using System;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class CurrentStock
    {
        public int ProductId { get; set; }
        public int StockroomId { get; set; }
        public int PackageTypeId { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalUsedQuantity { get; set; }
        public decimal TotalBookedQuantity { get; set; }
        public decimal TotalProductionQuantity { get; set; }

        public decimal AvailableQuantity =>
            TotalQuantity + TotalProductionQuantity - TotalUsedQuantity - TotalBookedQuantity;
    }
}