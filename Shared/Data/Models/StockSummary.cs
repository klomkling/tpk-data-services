using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class StockSummary : TgModelBase
    {
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ProductId { get; set; }
        [NotMapped] public decimal BeginningQuantity { get; set; }
        public decimal IncreasedQuantity { get; set; }
        public decimal DecreasedQuantity { get; set; }
        public decimal LiftedQuantity { get; set; }
        [NotMapped] public decimal BeginningBalance { get; set; }
        public decimal IncreasedBalance { get; set; }
        public decimal DecreasedBalance { get; set; }
        public decimal LiftedBalance { get; set; }
    }
}