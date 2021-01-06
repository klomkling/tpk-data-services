using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class ProductEditContext : EditContextBase<Product>
    {
        public ProductEditContext() : base(null)
        {
        }

        public ProductEditContext(Product dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Code = DataItem.Code;
            Name = DataItem.Name;
            CommonName = DataItem.CommonName;
            InventoryTypeId = DataItem.InventoryTypeId;
            ProductCategoryId = DataItem.ProductCategoryId;
            ProductLineId = DataItem.ProductLineId;
            ProductUnitId = DataItem.ProductUnitId;
            MaterialTypeId = DataItem.MaterialTypeId;
            MaterialSubType1Id = DataItem.MaterialSubType1Id;
            MaterialGradeId = DataItem.MaterialGradeId;
            ProductColorId = DataItem.ProductColorId;
            CoreWeight = DataItem.CoreWeight;
            StandardWeight = DataItem.StandardWeight;
            Width = DataItem.Width;
            Length = DataItem.Length;
            Height = DataItem.Height;
            Thickness = DataItem.Thickness;
            Diameter = DataItem.Diameter;
            NormalPrice = DataItem.NormalPrice;
            MoqPrice = DataItem.MoqPrice;
            InternalNormalPrice = DataItem.InternalNormalPrice;
            InternalMoqPrice = DataItem.InternalMoqPrice;
            StandardCost = DataItem.StandardCost;
            UnitCost = DataItem.UnitCost;
            MinimumOrder = DataItem.MinimumOrder;
            MinimumStock = DataItem.MinimumStock;
            Remark = DataItem.Remark;
            Picture = DataItem.Picture;
            IsForSale = DataItem.IsForSale;
            IsActive = DataItem.IsActive;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string CommonName { get; set; }
        public int InventoryTypeId { get; set; }
        public int? ProductCategoryId { get; set; }
        public int? ProductLineId { get; set; }
        public int ProductUnitId { get; set; }
        public int? MaterialTypeId { get; set; }
        public int? MaterialGradeId { get; set; }
        public int? MaterialSubType1Id { get; set; }
        public int? MaterialSubType2Id { get; set; }
        public int? MaterialSubType3Id { get; set; }
        public int? MaterialSubType4Id { get; set; }
        public int? ProductColorId { get; set; }
        public decimal CoreWeight { get; set; }
        public decimal StandardWeight { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Height { get; set; }
        public decimal Thickness { get; set; }
        public decimal Diameter { get; set; }
        public decimal NormalPrice { get; set; }
        public decimal MoqPrice { get; set; }
        public decimal InternalNormalPrice { get; set; }
        public decimal InternalMoqPrice { get; set; }
        public decimal StandardCost { get; set; }
        public decimal UnitCost { get; set; }
        public decimal MinimumOrder { get; set; }
        public decimal MinimumStock { get; set; }
        public string Remark { get; set; }
        public string Picture { get; set; }
        public bool IsForSale { get; set; }
        public bool IsActive { get; set; }
        
        [NotMapped]
        public string SaleStatus
        {
            get => IsForSale ? "For Sale" : "Not For Sale";
            set => IsForSale = value.Equals("For Sale", StringComparison.InvariantCultureIgnoreCase);
        }

        [NotMapped]
        public string ActiveStatus
        {
            get => IsActive ? "Active" : "Discontinued";
            set => IsActive = value.Equals("Active", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}