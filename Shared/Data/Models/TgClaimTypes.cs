using System;
using System.Linq;
using System.Security.Claims;
using Tpk.DataServices.Shared.Data.Constants;

namespace Tpk.DataServices.Shared.Data.Models
{
    public class TgClaimTypes : Enumeration
    {
        public static readonly TgClaimTypes SystemOptions = new TgClaimTypes(100, RestrictItems.SystemOptions);

        public static readonly TgClaimTypes Users = new TgClaimTypes(10000, RestrictItems.Users);
        public static readonly TgClaimTypes Companies = new TgClaimTypes(11000, RestrictItems.Companies);
        public static readonly TgClaimTypes Departments = new TgClaimTypes(11100, RestrictItems.Departments);
        public static readonly TgClaimTypes Sections = new TgClaimTypes(11200, RestrictItems.Sections);
        public static readonly TgClaimTypes Units = new TgClaimTypes(11300, RestrictItems.Units);
        public static readonly TgClaimTypes ContactTypes = new TgClaimTypes(12000, RestrictItems.ContactTypes);

        public static readonly TgClaimTypes Stocks = new TgClaimTypes(20000, RestrictItems.Stocks);

        public static readonly TgClaimTypes
            StockTransactions = new TgClaimTypes(20100, RestrictItems.StockTransactions);

        public static readonly TgClaimTypes StockLocations = new TgClaimTypes(21000, RestrictItems.StockLocations);
        public static readonly TgClaimTypes Stockrooms = new TgClaimTypes(21100, RestrictItems.Stockrooms);

        public static readonly TgClaimTypes
            InventoryRequests = new TgClaimTypes(22000, RestrictItems.InventoryRequests);

        public static readonly TgClaimTypes InventoryRequestLines =
            new TgClaimTypes(22010, RestrictItems.InventoryRequestLines);

        public static readonly TgClaimTypes InventoryRequestLineDetails =
            new TgClaimTypes(22020, RestrictItems.InventoryRequestLineDetails);

        public static readonly TgClaimTypes TransportationRequests =
            new TgClaimTypes(23000, RestrictItems.TransportationRequests);

        public static readonly TgClaimTypes TransportationRequestLines =
            new TgClaimTypes(23100, RestrictItems.TransportationRequestLines);

        public static readonly TgClaimTypes TransportationOrders =
            new TgClaimTypes(24000, RestrictItems.TransportationOrders);

        public static readonly TgClaimTypes TransportationOrderLines =
            new TgClaimTypes(24100, RestrictItems.TransportationOrderLines);

        public static readonly TgClaimTypes Products = new TgClaimTypes(30000, RestrictItems.Products);
        public static readonly TgClaimTypes ProductCategories = new TgClaimTypes(30100, RestrictItems.ProductCategories);
        public static readonly TgClaimTypes ProductLines = new TgClaimTypes(30200, RestrictItems.ProductLines);
        public static readonly TgClaimTypes ProductUnits = new TgClaimTypes(30300, RestrictItems.ProductUnits);
        public static readonly TgClaimTypes ProductColors = new TgClaimTypes(30400, RestrictItems.ProductColors);
        public static readonly TgClaimTypes ProductPackages = new TgClaimTypes(30510, RestrictItems.ProductPackages);
        public static readonly TgClaimTypes MaterialTypes = new TgClaimTypes(31000, RestrictItems.MaterialTypes);
        public static readonly TgClaimTypes MaterialSupTypes = new TgClaimTypes(31100, RestrictItems.MaterialSubTypes);
        public static readonly TgClaimTypes MaterialGrades = new TgClaimTypes(31200, RestrictItems.MaterialGrades);

        public static readonly TgClaimTypes Suppliers = new TgClaimTypes(40000, RestrictItems.Suppliers);
        public static readonly TgClaimTypes SupplierContacts = new TgClaimTypes(40100, RestrictItems.SupplierContacts);
        public static readonly TgClaimTypes SupplierProducts = new TgClaimTypes(41000, RestrictItems.SupplierProducts);

        public static readonly TgClaimTypes SupplierProductPackages =
            new TgClaimTypes(41100, RestrictItems.SupplierProductPackages);

        public static readonly TgClaimTypes SupplierOrders = new TgClaimTypes(42000, RestrictItems.SupplierOrders);

        public static readonly TgClaimTypes SupplierOrderLines =
            new TgClaimTypes(42100, RestrictItems.SupplierOrderLines);

        public static readonly TgClaimTypes Customers = new TgClaimTypes(50000, RestrictItems.Customers);
        public static readonly TgClaimTypes CustomerContacts = new TgClaimTypes(50100, RestrictItems.CustomerContacts);
        public static readonly TgClaimTypes CustomerAddresses = new TgClaimTypes(50200, RestrictItems.CustomerAddresses);
        public static readonly TgClaimTypes CustomerProducts = new TgClaimTypes(51000, RestrictItems.CustomerProducts);

        public static readonly TgClaimTypes CustomerProductPackages =
            new TgClaimTypes(51100, RestrictItems.CustomerProductPackages);

        public static readonly TgClaimTypes CustomerOrders = new TgClaimTypes(52000, RestrictItems.CustomerOrders);

        public static readonly TgClaimTypes CustomerOrderLines =
            new TgClaimTypes(52100, RestrictItems.CustomerOrderLines);

        public static readonly TgClaimTypes ProductionOrders = new TgClaimTypes(60000, RestrictItems.ProductionOrders);

        public static readonly TgClaimTypes ProductionRequests =
            new TgClaimTypes(61000, RestrictItems.ProductionRequests);

        private TgClaimTypes(int value, string displayName) : base(value, displayName)
        {
        }

        public TgClaimTypes()
        {
        }

        public static Claim ToClaim(string restrictItemName, string permissionName)
        {
            var item = GetAll<TgClaimTypes>().FirstOrDefault(i =>
                i.DisplayName.Equals(restrictItemName, StringComparison.InvariantCultureIgnoreCase));
            if (item == null) return null;

            var value = FromDisplayName<TgPermissions>(permissionName);
            return value == null ? null : new Claim(item.Value.ToString(), value.Value.ToString());
        }
    }
}