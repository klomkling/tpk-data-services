using System;

namespace Tpk.DataServices.Shared.Interfaces
{
    public interface ITgModelBase : ITgMinimalModelBase
    {
        string CreatedBy { get; set; }
        DateTime? CreatedAt { get; set; }
        string UpdatedBy { get; set; }
        DateTime? UpdatedAt { get; set; }
        string DeletedBy { get; set; }
        DateTime? DeletedAt { get; set; }
        string DeletedStatus { get; }
    }
}