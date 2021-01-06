using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tpk.DataServices.Server.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Classes
{
    public interface ITgControllerBase<in T>
    {
        bool IsError { get; }
        string ErrorMessage { get; }

        Task<IActionResult> SearchAsync([FromQuery] string[] columns, [FromQuery] string[] searchStrings,
            [FromQuery] string[] orderColumns = null);

        Task<IActionResult> GetAllAsync([FromQuery] DataSourceLoadOptions loadOptions);

        Task<IActionResult> GetByIdAsync(int id);
        Task<IActionResult> InsertAsync([FromBody] T model);
        Task<IActionResult> UpdateAsync([FromBody] T model);
        Task<IActionResult> DeleteAsync(int id);
        Task<IActionResult> BulkDeleteAsync([FromBody] IReadOnlyList<int> collection);
        Task<IActionResult> RestoreAsync(int id);
        Task<IActionResult> BulkRestoreAsync([FromBody] IReadOnlyList<int> collection);
        Task<IActionResult> IsUniqueAsync([FromBody] ValidationRequest model);
    }
}