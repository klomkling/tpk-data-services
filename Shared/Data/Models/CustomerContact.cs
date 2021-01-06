using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class CustomerContact : TgModelBase
    {
        public int CustomerId { get; set; }
        public int ContactTypeId { get; set; }
        public string ContactData { get; set; }
        public string Remark { get; set; }
    }
}