using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class InventoryRequestEditContext : EditContextBase<InventoryRequest>
    {
        private string _requestNumberDisplay;

        public InventoryRequestEditContext() : base(null)
        {
        }

        public InventoryRequestEditContext(InventoryRequest dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            RequestNumber = DataItem.RequestNumber;
            RequestDate = DataItem.RequestDate;
            DueDate = DataItem.DueDate;
            CompletedDate = DataItem.CompletedDate;
            Status = DataItem.Status;
            StatusDate = DataItem.StatusDate;
            RequestType = DataItem.RequestType;
            StockroomId = DataItem.StockroomId;
            RequestedBy = DataItem.RequestedBy;
            RequestApprovedBy = DataItem.RequestApprovedBy;
            StockPerson = DataItem.StockPerson;
            StockApprovedBy = DataItem.StockApprovedBy;
            AccountPerson = DataItem.AccountPerson;
            CheckedBy = DataItem.CheckedBy;
            Remark = DataItem.Remark;
        }

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

        [NotMapped]
        public string RequestNumberDisplay
        {
            get => $"{RequestNumber:000000}";
            set => _requestNumberDisplay = value;
        }

        [NotMapped] public string StockroomDisplay { get; set; }
    }
}