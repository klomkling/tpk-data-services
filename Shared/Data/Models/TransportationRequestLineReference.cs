using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class TransportationRequestLineReference : TgMinimalModelBase
    {
        public int TransportationRequestLineId { get; set; }
        public int InventoryRequestLineId { get; set; }
    }
}