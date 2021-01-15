using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class ImportInventoryEditContext : EditContextBase<ImportInventory>
    {
        public ImportInventoryEditContext() : base(null)
        {
            
        }
        
        public ImportInventoryEditContext(ImportInventory dataItem) : base(dataItem)
        {
            StockLocationId = DataItem.StockLocationId;
            PackageTypeId = DataItem.PackageTypeId;
            PalletNo = DataItem.PalletNo;
            ImportDetails = DataItem.ImportDetails;
        }
        
        public int StockLocationId { get; set; }
        public int? PackageTypeId { get; set; }
        public string PalletNo { get; set; }
        public string ImportDetails { get; set; }
    }
}