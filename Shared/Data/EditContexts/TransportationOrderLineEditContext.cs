using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Shared.Data.EditContexts
{
    public class TransportationOrderLineEditContext : EditContextBase<TransportationOrderLine>
    {
        public TransportationOrderLineEditContext() : base(null)
        {
        }
        
        public TransportationOrderLineEditContext(TransportationOrderLine dataItem) : base(dataItem)
        {
            Id = DataItem.Id;
            TransportationOrderId = DataItem.TransportationOrderId;
            TransportationRequestId = DataItem.TransportationRequestId;
        }
        
        public int TransportationOrderId { get; set; }
        public int TransportationRequestId { get; set; }
    }
}