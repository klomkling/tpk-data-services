using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class CustomerProduct : TgModelBase
    {
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ProductUnitId { get; set; }
        public decimal NormalPrice { get; set; }
        public decimal MoqPrice { get; set; }
        public decimal MinimumOrder { get; set; }
        public bool IsActive { get; set; }
    }
}