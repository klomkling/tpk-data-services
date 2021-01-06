using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class SupplierContactEditContext : EditContextBase<SupplierContact>
    {
        public SupplierContactEditContext() : base(null)
        {
        }

        public SupplierContactEditContext(SupplierContact dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            SupplierId = DataItem.SupplierId;
            ContactTypeId = DataItem.ContactTypeId;
            ContactData = DataItem.ContactData;
            Remark = DataItem.Remark;
        }

        public int SupplierId { get; set; }
        public int ContactTypeId { get; set; }
        public string ContactData { get; set; }
        public string Remark { get; set; }
    }
}