using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class ProductPackageWithDetail : ProductPackage
    {
        public string PackageCode { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal UsedQuantity { get; set; }
        public decimal BookedQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
    }
}