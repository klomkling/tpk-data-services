using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class InventoryRequest : TgModelBase
    {
        public int RequestNumber { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public string RequestType { get; set; }
        public int StockroomId { get; set; }
        public string RequestedBy { get; set; }
        public string RequestApprovedBy { get; set; }
        public string StockPerson { get; set; }
        public string StockApprovedBy { get; set; }
        public string AccountPerson { get; set; }
        public string CheckedBy { get; set; }
        public string Remark { get; set; }
    }
}