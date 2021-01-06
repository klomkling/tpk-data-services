using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface IStockBookingDetailReferenceService : IServiceBase<StockBookingDetailReference>
    {
        int DeleteByStockId(int id);
        Task<int> DeleteByStockIdAsync(int id);
        int DeleteByInventoryRequestLineDetailId(int id);
        Task<int> DeleteByInventoryRequestLineDetailIdAsync(int id);
    }
}