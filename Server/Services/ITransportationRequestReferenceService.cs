using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface ITransportationRequestReferenceService : IServiceBase<TransportationRequestReference>
    {
        int DeleteByTransportationRequestId(int id);
        Task<int> DeleteByTransportationRequestIdAsync(int id);
        int DeleteByInventoryRequestId(int id);
        Task<int> DeleteByInventoryRequestIdAsync(int id);
    }
}