using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class Customer : TgModelBase
    {
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
    }
}