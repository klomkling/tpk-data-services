using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Client.Components.Base
{
    public class CustomerAddressComponentBase : TgComponentBase<CustomerAddress, CustomerAddress, CustomerAddressEditContext>
    {
        protected CustomerAddress CustomerAddressWithDetail { get; set; }
        protected IEnumerable<TgAddressTypes> AddressTypeCollection { get; set; }
        protected readonly IEnumerable<string> DefaultDropdown = new List<string>
            {"Yes", "No"};

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.CustomerAddresses, "api/customer-addresses");

            AddressTypeCollection = StringEnumeration.GetAll<TgAddressTypes>();
        }

        protected override void OnBeforeRowEditing(CustomerAddress model = default)
        {
            DataEditContext.CustomerId = MasterId;

            if (DataEditContext.Id != 0) return;
            
            DataEditContext.AddressType = TgAddressTypes.ShippingAddress.Value;
        }

        protected async Task<LoadResult> LoadCustomerAddresses(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/customers/{MasterId}/addresses";

            return await LoadDataAsync(url, loadOptions, cancellationToken);
        }

        protected async Task<IEnumerable<TgAddressTypes>> GetAddressTypeAsync(CancellationToken cancellationToken)
        {
            return await Task.FromResult(AddressTypeCollection);
        }
    }
}