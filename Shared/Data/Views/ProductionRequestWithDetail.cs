using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class ProductionRequestWithDetail : ProductionRequest
    {
        public string StatusName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public bool IsGenerated { get; set; }
        public bool IsCustomerOrder { get; set; }
    }
}