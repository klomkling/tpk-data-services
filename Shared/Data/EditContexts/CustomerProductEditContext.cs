using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class CustomerProductEditContext : EditContextBase<CustomerProduct>
    {
        public CustomerProductEditContext() : base(null)
        {
        }
        
        public CustomerProductEditContext(CustomerProduct dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            CustomerId = DataItem.CustomerId;
            ProductId = DataItem.ProductId;
            Code = DataItem.Code;
            Name = DataItem.Name;
            ProductUnitId = DataItem.ProductUnitId;
            NormalPrice = DataItem.NormalPrice;
            MoqPrice = DataItem.MoqPrice;
            MinimumOrder = DataItem.MinimumOrder;
        }
        
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int ProductUnitId { get; set; }
        public decimal NormalPrice { get; set; }
        public decimal MoqPrice { get; set; }
        public decimal MinimumOrder { get; set; }
        public bool IsActive { get; set; }
        
        [NotMapped]
        public string Status
        {
            get => IsActive ? "For Sale" : "Discontinued";
            set => IsActive = value.Equals("For Sale", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}