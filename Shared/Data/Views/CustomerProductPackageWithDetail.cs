using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class CustomerProductPackageWithDetail : CustomerProductPackage
    {
        public string PackageCode { get; set; }
    }
}