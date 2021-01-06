using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class CustomerWithDetail : Customer
    {
        public string OemByCustomerName { get; set; }
    }
}