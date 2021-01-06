using System;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.Views
{
    [Serializable]
    public class CustomerContactWithDetail : CustomerContact
    {
        public string ContactTypeName { get; set; }
    }
}