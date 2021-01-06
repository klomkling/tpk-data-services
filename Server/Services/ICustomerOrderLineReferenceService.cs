using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface ICustomerOrderLineReferenceService : IServiceBase<CustomerOrderLineReference>
    {
        int DeleteByCustomerOrderLineId(int id);
        Task<int> DeleteByCustomerOrderLineIdAsync(int id);
        int DeleteByInventoryRequestLineId(int id);
        Task<int> DeleteByInventoryRequestLineIdAsync(int id);
    }
}