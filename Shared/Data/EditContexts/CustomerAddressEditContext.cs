using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class CustomerAddressEditContext : EditContextBase<CustomerAddress>
    {
        public CustomerAddressEditContext() : base(null)
        {
        }

        public CustomerAddressEditContext(CustomerAddress dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            CustomerId = DataItem.CustomerId;
            AddressType = DataItem.AddressType;
            IsDefault = DataItem.IsDefault;
            Name = DataItem.Name;
            Recipient = DataItem.Recipient;
            Address = DataItem.Address;
            SubDistrict = DataItem.SubDistrict;
            District = DataItem.District;
            Province = DataItem.Province;
            PostalCode = DataItem.PostalCode;
            Remark = DataItem.Remark;
        }
        
        public int CustomerId { get; set; }
        public string AddressType { get; set; }
        public bool IsDefault { get; set; }
        public string Name { get; set; }
        public string Recipient { get; set; }
        public string Address { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Remark { get; set; }

        [NotMapped]
        public string IsDefaultName
        {
            get => IsDefault ? "Yes" : "No";
            set => IsDefault = value.Equals("Yes", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}