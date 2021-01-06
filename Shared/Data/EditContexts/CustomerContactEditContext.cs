using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class CustomerContactEditContext : EditContextBase<CustomerContact>
    {
        public CustomerContactEditContext() : base(null)
        {
        }

        public CustomerContactEditContext(CustomerContact dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            CustomerId = DataItem.CustomerId;
            ContactTypeId = DataItem.ContactTypeId;
            ContactData = DataItem.ContactData;
            Remark = DataItem.Remark;
        }

        public int CustomerId { get; set; }
        public int ContactTypeId { get; set; }
        public string ContactData { get; set; }
        public string Remark { get; set; }
    }
}