using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Components;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Client.Components.Transactions
{
    public class CustomerOrderLineComponentBase
        : TgComponentBase<CustomerOrderLineWithDetail, CustomerOrderLine, CustomerOrderLineEditContext>
    {
        private CustomerProduct _selectedCustomerProduct;

        protected CustomerOrderLineWithDetail CustomerOrderLineWithDetail { get; set; }

        protected bool IsPermanent { get; set; }
        protected string UnitName { get; set; }

        protected CustomerProduct SelectedCustomerProduct
        {
            get => _selectedCustomerProduct;
            set
            {
                if (Equals(_selectedCustomerProduct, value)) return;
                _selectedCustomerProduct = value;
                DataEditContext.CustomerProductId = value?.Id ?? 0;
                var isProductChanged = DataEditContext.CustomerProductId != DataEditContext.DataItem.CustomerProductId;
                DataEditContext.ProductId = isProductChanged ? value?.ProductId ?? 0 : DataEditContext.DataItem.ProductId;
                DataEditContext.Description = isProductChanged ? value?.Name : DataEditContext.DataItem.Description;
                DataEditContext.Price = isProductChanged ? value?.NormalPrice ?? 0m : DataEditContext.DataItem.Price;
                LoadProductUnit(value?.ProductUnitId ?? 0);

                StateHasChanged();
            }
        }

        [Parameter] public int CustomerId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.CustomerOrders, "api/customer-order-lines");
        }

        protected override void OnSelectedCollectionChanged(IEnumerable<CustomerOrderLineWithDetail> collection)
        {
            var permanentCount = collection.Count(c => c.InventoryRequestLineId > 0);
            IsPermanent = SelectedCount == 1 && SelectedItem.InventoryRequestLineId > 0 ||
                          SelectedCount == permanentCount;
        }

        protected override bool OnValidateRowDoubleClick(CustomerOrderLineWithDetail args)
        {
            return args.InventoryRequestLineId == 0;
        }

        protected override void OnBeforeRowEditing(CustomerOrderLineWithDetail model = default)
        {
            DataEditContext.CustomerOrderId = MasterId;
            SelectedCustomerProduct = null;

            if (DataEditContext.Id == 0) return;

            Task.Run(async () =>
            {
                var result = await ApiService.GetAsync<CustomerProduct>(DataEditContext.DataItem.CustomerProductId,
                    "api/customer-products");
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                SelectedCustomerProduct = result;
            });
        }

        protected async Task<LoadResult> LoadCustomerOrderLines(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/customer-orders/{MasterId}/lines";

            var result = await LoadDataAsync(url, loadOptions, cancellationToken);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            return result;
        }

        protected async Task<IEnumerable<CustomerProduct>> SearchProducts(string searchText)
        {
            var url =
                $"api/customer-products/search?columns=customerId&searchStrings={CustomerId}&columns=code&searchStrings={searchText}";

            var result = await ApiService.GetAllAsync<CustomerProduct>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            return result;
        }

        protected void OnQuantityChanged(decimal newQuantity)
        {
            if (SelectedCustomerProduct.MinimumOrder > 0m && SelectedCustomerProduct.MoqPrice > 0m)
            {
                DataEditContext.Price = newQuantity >= SelectedCustomerProduct.MinimumOrder
                    ? SelectedCustomerProduct.MoqPrice
                    : SelectedCustomerProduct.NormalPrice;
            }
            DataEditContext.EditQuantity = newQuantity;
        }

        private void LoadProductUnit(int id)
        {
            if (id == 0)
            {
                UnitName = null;
                return;
            }

            Task.Run(async () =>
            {
                var productUnit = await ApiService.GetAsync<ProductUnit>(id, "api/product-units");
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                UnitName = productUnit == null ? null : $" ({productUnit.Code})";
                StateHasChanged();
            });
        }
    }
}