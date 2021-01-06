using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Components.Base
{
    public class
        MaterialSubTypeComponentBase : TgComponentBase<MaterialSubType, MaterialSubType, MaterialSubTypeEditContext>
    {
        protected MaterialSubType MaterialSubType { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.MaterialSupTypes, "api/material-sub-types");
        }

        protected async Task<LoadResult> LoadMaterialSubTypes(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            return await LoadDataAsync(ApiUrl, loadOptions, cancellationToken);
        }
    }
}