using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class ContactTypeEditContext : EditContextBase<ContactType>
    {
        public ContactTypeEditContext() : base(null)
        {
        }

        public ContactTypeEditContext(ContactType dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            Name = DataItem.Name;
            Description = DataItem.Description;
            IsPermanent = DataItem.IsPermanent;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsPermanent { get; set; }
    }
}