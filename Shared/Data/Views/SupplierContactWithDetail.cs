using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class SupplierContactWithDetail : SupplierContact
    {
        public string ContactTypeName { get; set; }
    }
}