using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Components.Base
{
    public class ContactTypeComponentBase : TgComponentBase<ContactType, ContactType, ContactTypeEditContext>
    {
        protected ContactType ContactType { get; set; }
        protected bool IsPermanent { get; set; }


        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.ContactTypes, "api/contact-types");
        }

        protected async Task<LoadResult> LoadContactTypes(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            return await LoadDataAsync(ApiUrl, loadOptions, cancellationToken);
        }

        protected override void OnSelectedCollectionChanged(IEnumerable<ContactType> collection)
        {
            var permanentCount = collection.Count(c => c.IsPermanent);
            IsPermanent = SelectedCount == 1 && SelectedItem.IsPermanent || SelectedCount == permanentCount;
        }

        protected override IEnumerable<int> SelectedIdCollection()
        {
            var collection = SelectedCollection.Where(c => c.IsPermanent == false).Select(c => c.Id);
            return collection;
        }
    }
}