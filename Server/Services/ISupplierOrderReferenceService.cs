using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface ISupplierOrderReferenceService : IServiceBase<SupplierOrderReference>
    {
        int DeleteBySupplierOrderId(int id);
        Task<int> DeleteBySupplierOrderIdAsync(int id);
        int DeleteByInventoryRequestId(int id);
        Task<int> DeleteByInventoryRequestIdAsync(int id);
    }
}