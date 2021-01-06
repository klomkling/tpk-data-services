using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class TransportationRequestLineWithDetail : TransportationRequestLine
    {
        public bool IsFromInventoryRequestLine { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
    }
}