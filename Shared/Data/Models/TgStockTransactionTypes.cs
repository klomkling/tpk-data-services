namespace Tpk.DataServices.Shared.Data.Models
{
    public class TgStockTransactionTypes : StringEnumeration
    {
        public static readonly TgStockTransactionTypes In = new TgStockTransactionTypes("I", "In");
        public static readonly TgStockTransactionTypes Out = new TgStockTransactionTypes("O", "Out");

        public TgStockTransactionTypes()
        {
        }

        public TgStockTransactionTypes(string value, string name) : base(value, name)
        {
        }
    }
}