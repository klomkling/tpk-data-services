using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class ProductionRequestReference : TgMinimalModelBase
    {
        public int CustomerOrderId { get; set; }
        public int ProductionRequestId { get; set; }
    }
}