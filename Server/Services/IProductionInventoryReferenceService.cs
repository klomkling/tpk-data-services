using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface IProductionInventoryReferenceService : IServiceBase<ProductionInventoryReference>
    {
        int DeleteByProductionOrderId(int id);
        Task<int> DeleteByProductionOrderIdAsync(int id);
        int DeleteByInventoryRequestId(int id);
        Task<int> DeleteByInventoryRequestIdAsync(int id);
    }
}