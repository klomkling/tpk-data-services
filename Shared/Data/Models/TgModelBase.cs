using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Interfaces;

namespace Tpk.DataServices.Shared.Data.Models
{
    public class TgModelBase : ITgModelBase
    {
        public int Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }

        [NotMapped]
        public string DeletedStatus => string.IsNullOrEmpty(DeletedBy) && Equals(DeletedAt, null) ? null : "Deleted";
    }
}