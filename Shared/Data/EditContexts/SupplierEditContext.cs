using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class SupplierEditContext : EditContextBase<Supplier>
    {
        public SupplierEditContext() : base(null)
        {
        }

        public SupplierEditContext(Supplier dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Code = DataItem.Code;
            Name = DataItem.Name;
            Address = DataItem.Address;
            SubDistrict = DataItem.SubDistrict;
            District = DataItem.District;
            Province = DataItem.Province;
            PostalCode = DataItem.PostalCode;
            TaxId = DataItem.TaxId;
        }

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