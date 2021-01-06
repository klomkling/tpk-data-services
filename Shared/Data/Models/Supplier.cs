using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class Supplier : TgModelBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string TaxId { get; set; }
    }
}