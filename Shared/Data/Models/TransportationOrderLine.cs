using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class TransportationOrderLine : TgModelBase
    {
        public int TransportationOrderId { get; set; }
        public int TransportationRequestId { get; set; }
    }
}