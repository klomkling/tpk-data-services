using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface IStockBookingReferenceService : IServiceBase<StockBookingReference>
    {
        int DeleteByProductId(int id);
        Task<int> DeleteByProductIdAsync(int id);
        int DeleteByInventoryRequestLineId(int id);
        Task<int> DeleteByInventoryRequestLineIdAsync(int id);
    }
}