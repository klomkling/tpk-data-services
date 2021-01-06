using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class SupplierProductPackageWithDetail : SupplierProductPackage
    {
        public string PackageCode { get; set; }
    }
}