using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class StockSummaryWithDetail : StockSummary
    {
        public string ProductName { get; set; }
        public string UnitName { get; set; }
    }
}