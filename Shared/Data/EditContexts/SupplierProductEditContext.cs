using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class SupplierProductEditContext : EditContextBase<SupplierProduct>
    {
        public SupplierProductEditContext() : base(null)
        {
        }

        public SupplierProductEditContext(SupplierProduct dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            SupplierId = DataItem.SupplierId;
            Code = DataItem.Code;
            Name = DataItem.Name;
            ProductId = DataItem.ProductId;
            ProductUnitId = DataItem.ProductUnitId;
            NormalPrice = DataItem.NormalPrice;
            MoqPrice = DataItem.MoqPrice;
            IsIncludedVat = DataItem.IsIncludedVat;
            MinimumOrder = DataItem.MinimumOrder;
            IsActive = DataItem.IsActive;
        }

        public int SupplierId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? ProductId { get; set; }
        public int ProductUnitId { get; set; }
        public decimal NormalPrice { get; set; }
        public decimal MoqPrice { get; set; }
        public bool IsIncludedVat { get; set; }
        public decimal MinimumOrder { get; set; }
        public bool IsActive { get; set; }

        [NotMapped]
        public string Status
        {
            get => IsActive ? "Active" : "Discontinued";
            set => IsActive = value.Equals("Active", StringComparison.InvariantCultureIgnoreCase);
        }

        [NotMapped]
        public string VatStatus
        {
            get => IsIncludedVat ? "Included" : "Excluded";
            set => IsIncludedVat = value.Equals("Included", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}