namespace Tpk.DataServices.Shared.Data.Models
{
    public class TgInventoryRequestTypes : StringEnumeration
    {
        public static readonly TgInventoryRequestTypes ManualRequest = new TgInventoryRequestTypes("M", "Manual Request");
        public static readonly TgInventoryRequestTypes ManualReturn = new TgInventoryRequestTypes("N", "Manual Return");

        public static readonly TgInventoryRequestTypes PurchaseOrder =
            new TgInventoryRequestTypes("B", "Purchase Order");

        public static readonly TgInventoryRequestTypes ProductionOrder =
            new TgInventoryRequestTypes("P", "Production Order");
        
        public static readonly TgInventoryRequestTypes ProductionRequest =
            new TgInventoryRequestTypes("R", "Production Request");

        public static readonly TgInventoryRequestTypes CustomerOrder =
            new TgInventoryRequestTypes("O", "Customer Order");

        public TgInventoryRequestTypes()
        {
        }

        public TgInventoryRequestTypes(string value, string name) : base(value, name)
        {
        }
    }
}