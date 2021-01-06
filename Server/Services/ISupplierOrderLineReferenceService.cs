using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface ISupplierOrderLineReferenceService : IServiceBase<SupplierOrderLineReference>
    {
        int DeleteBySupplierOrderLineId(int id);
        Task<int> DeleteBySupplierOrderLineAsync(int id);
        int DeleteByInventoryRequestLineId(int id);
        Task<int> DeleteByInventoryRequestLineIdAsync(int id);
    }
}