using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Components.Base
{
    public class ProductColorComponentBase : TgComponentBase<ProductColor, ProductColor, ProductColorEditContext>
    {
        protected ProductColor ProductColor { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.ProductColors, "api/product-colors");
        }

        protected async Task<LoadResult> LoadProductColors(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            return await LoadDataAsync(ApiUrl, loadOptions, cancellationToken);
        }
    }
}