using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class CompanyEditContext : EditContextBase<Company>
    {
        public CompanyEditContext() : base(null)
        {
        }

        public CompanyEditContext(Company dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Name = DataItem.Name;
            Address = DataItem.Address;
            SubDistrict = DataItem.SubDistrict;
            District = DataItem.District;
            Province = DataItem.Province;
            PostalCode = DataItem.PostalCode;
            Phone = DataItem.Phone;
            Fax = DataItem.Fax;
            TaxId = DataItem.TaxId;
        }

        public string Name { get; set; }
        public string Address { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string TaxId { get; set; }
    }
}