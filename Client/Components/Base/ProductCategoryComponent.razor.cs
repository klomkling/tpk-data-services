using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Components.Base
{
    public class ProductCategoryComponentBase : TgComponentBase<ProductCategory, ProductCategory, ProductCategoryEditContext>
    {
        protected ProductCategory ProductCategory { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.ProductCategories, "api/product-categories");
        }

        protected async Task<LoadResult> LoadProductCategories(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            return await LoadDataAsync(ApiUrl, loadOptions, cancellationToken);
        }
    }
}