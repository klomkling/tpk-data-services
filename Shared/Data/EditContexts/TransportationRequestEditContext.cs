using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class TransportationRequestEditContext : EditContextBase<TransportationRequest>
    {
        private string _requestNumberDisplay;

        public TransportationRequestEditContext() : base(null)
        {
        }

        public TransportationRequestEditContext(TransportationRequest dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            RequestNumber = DataItem.RequestNumber;
            RequestDate = DataItem.RequestDate;
            DueDate = DataItem.DueDate;
            CompletedDate = DataItem.CompletedDate;
            Status = DataItem.Status;
            StatusDate = DataItem.StatusDate;
            Remark = DataItem.Remark;
            RecipientName = DataItem.RecipientName;
            Address = DataItem.Address;
            SubDistrict = DataItem.SubDistrict;
            District = DataItem.District;
            Province = DataItem.Province;
            PostalCode = DataItem.PostalCode;
            Contact = DataItem.Contact;
        }
        
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

        [NotMapped]
        public string OrderNumberDisplay
        {
            get => $"{RequestNumber:000000}";
            set => _requestNumberDisplay = value;
        }
    }
}