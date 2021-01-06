using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class SupplierContact : TgModelBase
    {
        public int SupplierId { get; set; }
        public int ContactTypeId { get; set; }
        public string ContactData { get; set; }
        public string Remark { get; set; }
    }
}