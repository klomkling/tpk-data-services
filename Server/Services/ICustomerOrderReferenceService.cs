using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface ICustomerOrderReferenceService : IServiceBase<CustomerOrderReference>
    {
        int DeleteByCustomerOrderId(int id);
        Task<int> DeleteByCustomerOrderIdAsync(int id);
        int DeleteByInventoryRequestId(int id);
        Task<int> DeleteByInventoryRequestIdAsync(int id);
    }
}