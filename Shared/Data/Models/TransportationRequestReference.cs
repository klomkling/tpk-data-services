using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class TransportationRequestReference : TgMinimalModelBase
    {
        public int TransportationRequestId { get; set; }
        public int InventoryRequestId { get; set; }
    }
}