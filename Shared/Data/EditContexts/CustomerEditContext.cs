using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class CustomerEditContext : EditContextBase<Customer>
    {
        public CustomerEditContext() : base(null)
        {
        }

        public CustomerEditContext(Customer dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Code = DataItem.Code;
            Name = DataItem.Name;
            Branch = dataItem.Branch;
            Address = DataItem.Address;
            SubDistrict = DataItem.SubDistrict;
            District = DataItem.District;
            Province = DataItem.Province;
            PostalCode = DataItem.PostalCode;
            TaxId = DataItem.TaxId;
            OemByCustomerId = DataItem.OemByCustomerId;
            IsAffiliatedCompany = DataItem.IsAffiliatedCompany;
            Comment = DataItem.Comment;
        }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Branch { get; set; }
        public string Address { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string TaxId { get; set; }
        public int? OemByCustomerId { get; set; }
        public bool IsAffiliatedCompany { get; set; }
        public string Comment { get; set; }
        
        [NotMapped]
        public string AffiliatedCompany
        {
            get => IsAffiliatedCompany ? "Yes" : "No";
            set => IsAffiliatedCompany = value.Equals("Yes", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}