namespace Tpk.DataServices.Shared.Data.Models
{
    public class TgAddressTypes : StringEnumeration
    {
        public static readonly TgAddressTypes BillingAddress = new TgAddressTypes("B", "Billing Address");
        public static readonly TgAddressTypes ShippingAddress = new TgAddressTypes("S", "Shipping Address");
        
        private TgAddressTypes(string value, string displayName) : base(value, displayName)
        {
        }

        public TgAddressTypes()
        {
        }
    }
}