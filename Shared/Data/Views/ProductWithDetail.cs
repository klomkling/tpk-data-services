using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class ProductWithDetail : Product
    {
        public string ProductCategoryName { get; set; }
        public string ProductLineName { get; set; }
        public string ProductUnitName { get; set; }
        public string ProductColorName { get; set; }
        public string MaterialTypeCode { get; set; }
        public string MaterialGradeCode { get; set; }
        public string MaterialSubType1Code { get; set; }
        public string MaterialSubType2Code { get; set; }
        public string MaterialSubType3Code { get; set; }
        public string MaterialSubType4Code { get; set; }
        public decimal StockQuantity { get; set; }
        public decimal ProductionQuantity { get; set; }
        public decimal UsedQuantity { get; set; }
        public decimal BookedQuantity { get; set; }
        public decimal AvailableQuantity { get; set; }
    }
}