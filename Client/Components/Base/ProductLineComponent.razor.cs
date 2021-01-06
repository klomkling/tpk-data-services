using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Components.Base
{
    public class ProductLineComponentBase : TgComponentBase<ProductLine, ProductLine, ProductLineEditContext>
    {
        protected ProductLine ProductLine { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.ProductLines, "api/product-lines");
        }

        protected async Task<LoadResult> LoadProductLines(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            return await LoadDataAsync(ApiUrl, loadOptions, cancellationToken);
        }
    }
}