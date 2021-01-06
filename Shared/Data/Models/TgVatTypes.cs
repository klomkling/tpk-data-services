namespace Tpk.DataServices.Shared.Data.Models
{
    public class TgVatTypes : Enumeration
    {
        public static readonly TgVatTypes RawMaterials = new TgVatTypes(1, "Included VAT");
        public static readonly TgVatTypes FinishedGoods = new TgVatTypes(2, "Excluded VAT");
        public static readonly TgVatTypes TradingGoods = new TgVatTypes(3, "Zero VAT");
        public static readonly TgVatTypes Scrap = new TgVatTypes(9, "Non VAT");

        private TgVatTypes(int value, string displayName) : base(value, displayName)
        {
        }

        public TgVatTypes()
        {
        }
    }
}