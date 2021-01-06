using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class CustomerAddress : TgModelBase
    {
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
    }
}