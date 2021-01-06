using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class ValidationResponse
    {
        public bool IsUnique { get; set; }
    }
}