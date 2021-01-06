using System;

namespace Tpk.DataServices.Shared.Data.Models
{
    [Serializable]
    public class TransportationOrder : TgModelBase
    {
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
    }
}