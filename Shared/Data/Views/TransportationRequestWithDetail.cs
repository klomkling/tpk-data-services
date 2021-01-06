using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class TransportationRequestWithDetail : TransportationRequest
    {
        public bool IsFromInventoryRequest { get; set; }
        public bool IsCompleted { get; set; }
        public string StatusName { get; set; }
    }
}