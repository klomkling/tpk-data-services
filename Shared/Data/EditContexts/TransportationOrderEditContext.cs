using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class TransportationOrderEditContext : EditContextBase<TransportationOrder>
    {
        private string _orderNumberDisplay;

        public TransportationOrderEditContext() : base(null)
        {
        }

        public TransportationOrderEditContext(TransportationOrder dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            OrderNumber = DataItem.OrderNumber;
            OrderDate = DataItem.OrderDate;
            DueDate = DataItem.DueDate;
            CompletedDate = DataItem.CompletedDate;
            Status = DataItem.Status;
            StatusDate = DataItem.StatusDate;
            TruckLicensePlate = DataItem.TruckLicensePlate;
            DriverName = DataItem.DriverName;
            DriverLicenseCard = DataItem.DriverLicenseCard;
            CoDriver1Name = DataItem.CoDriver1Name;
            CoDriver2Name = DataItem.CoDriver2Name;
            StartMileGauge = DataItem.StartMileGauge;
            EndMileGauge = DataItem.EndMileGauge;
            Remark = DataItem.Remark;
        }
        
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public DateTime StatusDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string TruckLicensePlate { get; set; }
        public string DriverName { get; set; }
        public string DriverLicenseCard { get; set; }
        public string CoDriver1Name { get; set; }
        public string CoDriver2Name { get; set; }
        public int StartMileGauge { get; set; }
        public int EndMileGauge { get; set; }
        public string Remark { get; set; }
        
        [NotMapped]
        public string OrderNumberDisplay
        {
            get => $"{OrderNumber:000000}";
            set => _orderNumberDisplay = value;
        }
    }
}