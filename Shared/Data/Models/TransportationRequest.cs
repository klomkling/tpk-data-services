using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class TransportationRequest : TgModelBase
    {
        public int RequestNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Remark { get; set; }
        public string RecipientName { get; set; }
        public string Address { get; set; }
        public string SubDistrict { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Contact { get; set; }
    }
}