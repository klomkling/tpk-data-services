using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface IProductionOrderReferenceService : IServiceBase<ProductionOrderReference>
    {
        int DeleteByProductionOrderId(int id);
        Task<int> DeleteByProductionOrderIdAsync(int id);
        int DeleteByProductionRequestId(int id);
        Task<int> DeleteByProductionRequestIdAsync(int id);
    }
}