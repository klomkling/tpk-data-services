using System;
using System.Collections.Generic;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class ValidationRequest
    {
        public IEnumerable<ValidationRequestColumn> ValidateColumns { get; set; }
        public ValidationRequestColumn KeyColumn { get; set; }
    }
}