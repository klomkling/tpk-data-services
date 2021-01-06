using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface IProductionRequestReferenceService : IServiceBase<ProductionRequestReference>
    {
        int DeleteByCustomerOrderId(int id);
        Task<int> DeleteByCustomerOrderIdAsync(int id);
        int DeleteByProductionRequestId(int id);
        Task<int> DeleteByProductionRequestIdAsync(int id);
    }
}