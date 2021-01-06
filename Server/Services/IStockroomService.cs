using System.Threading.Tasks;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services
{
    public interface IStockroomService : IServiceBase<Stockroom>
    {
        Stockroom GetByName(string name, bool isActive = true);
        Task<Stockroom> GetByNameAsync(string name, bool isActive = true);
    }
}