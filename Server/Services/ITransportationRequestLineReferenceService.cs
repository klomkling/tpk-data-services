using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface ITransportationRequestLineReferenceService : IServiceBase<TransportationRequestLineReference>
    {
        int DeleteByTransportationRequestLineId(int id);
        Task<int> DeleteByTransportationRequestLineIdAsync(int id);
        int DeleteByInventoryRequestLineId(int id);
        Task<int> DeleteByInventoryRequestLineIdAsync(int id);
    }
}