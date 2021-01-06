using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class CustomerProductWithDetail : CustomerProduct
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
    }
}