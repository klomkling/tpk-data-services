using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class ImportInventory
    {
        public int StockLocationId { get; set; }
        public int PackageTypeId { get; set; }
        public string PalletNo { get; set; }
        public string ImportDetails { get; set; }
    }
}