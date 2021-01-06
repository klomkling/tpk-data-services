namespace Tpk.DataServices.Shared.Data.Models
{
    public class TgInventoryTypes : Enumeration
    {
        public static readonly TgInventoryTypes RawMaterials = new TgInventoryTypes(10, "Raw Materials");
        public static readonly TgInventoryTypes FinishedGoods = new TgInventoryTypes(20, "Finished Goods");
        public static readonly TgInventoryTypes TradingGoods = new TgInventoryTypes(30, "Trading Goods");
        public static readonly TgInventoryTypes PackageTypeBoxes = new TgInventoryTypes(40, "Packaging (Box)");
        public static readonly TgInventoryTypes PackageTypeBags = new TgInventoryTypes(50, "Packaging (Bag)");
        public static readonly TgInventoryTypes Stickers = new TgInventoryTypes(60, "Stickers");
        public static readonly TgInventoryTypes Supplies = new TgInventoryTypes(80, "Supplies");
        public static readonly TgInventoryTypes Scrap = new TgInventoryTypes(99, "Scrap");

        private TgInventoryTypes(int value, string displayName) : base(value, displayName)
        {
        }

        public TgInventoryTypes()
        {
        }
    }
}