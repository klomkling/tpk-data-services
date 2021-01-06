using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class ProductionOrderReference : TgMinimalModelBase
    {
        public int ProductionOrderId { get; set; }
        public int ProductionRequestId { get; set; }
    }
}