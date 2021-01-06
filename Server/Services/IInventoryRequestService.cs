using System.Collections.Generic;
using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface IInventoryRequestService : IServiceBase<InventoryRequest>
    {
        bool ConfirmRequest(IEnumerable<int> collection, StringEnumeration transactionType);
        Task<bool> ConfirmRequestAsync(IEnumerable<int> collection, StringEnumeration transactionType);
    }
}