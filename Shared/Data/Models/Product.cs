using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class Product : TgModelBase
    {
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