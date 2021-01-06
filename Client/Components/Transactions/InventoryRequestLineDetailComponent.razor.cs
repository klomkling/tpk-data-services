using System;
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
    public class InventoryRequestLineDetailComponentBase : TgComponentBase<InventoryRequestLineDetailWithDetail,
        InventoryRequestLineDetail, InventoryRequestLineDetailEditContext>
    {
        private InventoryRequest _inventoryRequest;
        private InventoryRequestLine _inventoryRequestLine;
        private Product _selectedPackageType;
        private Product _selectedProduct;
        private StockLocation _selectedStockLocation;
        private Stockroom _selectedStockroom;
        protected InventoryRequestLineDetailWithDetail InventoryRequestLineDetailWithDetail { get; set; }

        private InventoryRequest InventoryRequest
        {
            get => _inventoryRequest;
            set
            {
                if (Equals(_inventoryRequest, value)) return;
                _inventoryRequest = value;
                IsPurchaseOrder = value?.RequestType.Equals(TgInventoryRequestTypes.PurchaseOrder.Value,
                    StringComparison.InvariantCultureIgnoreCase) ?? false;
                IsCustomerOrder = value?.RequestType.Equals(TgInventoryRequestTypes.CustomerOrder.Value,
                    StringComparison.InvariantCultureIgnoreCase) ?? false;
                IsProductionOrder = value?.RequestType.Equals(TgInventoryRequestTypes.ProductionOrder.Value,
                    StringComparison.InvariantCultureIgnoreCase) ?? false;
                IsProductionRequest = value?.RequestType.Equals(TgInventoryRequestTypes.ProductionRequest.Value,
                    StringComparison.InvariantCultureIgnoreCase) ?? false;
                IsManual = (value?.RequestType.Equals(TgInventoryRequestTypes.ManualRequest.Value,
                               StringComparison.InvariantCultureIgnoreCase) ?? false) ||
                           (value?.RequestType.Equals(TgInventoryRequestTypes.ManualReturn.Value,
                               StringComparison.InvariantCultureIgnoreCase) ?? false);
                StateHasChanged();
            }
        }

        private InventoryRequestLine InventoryRequestLine
        {
            get => _inventoryRequestLine;
            set
            {
                if (Equals(_inventoryRequestLine, value)) return;
                _inventoryRequestLine = value;
                SupplierProductId = value?.SupplierProductId ?? 0;
                ProductId = value?.ProductId ?? 0;
            }
        }

        protected Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (Equals(_selectedProduct, value)) return;
                _selectedProduct = value;
                DataEditContext.ProductId = value?.Id ?? 0;
                DataEditContext.ProductCode = value?.Code;
                StateHasChanged();
            }
        }

        protected Product SelectedPackageType
        {
            get => _selectedPackageType;
            set
            {
                if (Equals(_selectedPackageType, value)) return;
                _selectedPackageType = value;
                DataEditContext.PackageTypeId = value?.Id ?? 0;
                DataEditContext.PackageTypeCode = value?.Code;
                StateHasChanged();
            }
        }

        protected Stockroom SelectedStockroom
        {
            get => _selectedStockroom;
            set
            {
                if (Equals(_selectedStockroom, value)) return;
                _selectedStockroom = value;
                DataEditContext.StockroomId = value?.Id ?? 0;
                DataEditContext.StockroomName = value?.Name;
                StateHasChanged();
            }
        }

        protected StockLocation SelectedStockLocation
        {
            get => _selectedStockLocation;
            set
            {
                if (Equals(_selectedStockLocation, value)) return;
                _selectedStockLocation = value;
                DataEditContext.StockLocationId = value?.Id ?? 0;
                DataEditContext.StockLocationName = value?.Location;
                StateHasChanged();
            }
        }

        protected bool IsPurchaseOrder { get; set; }
        protected bool IsCustomerOrder { get; set; }
        protected bool IsProductionOrder { get; set; }
        protected bool IsProductionRequest { get; set; }
        protected bool IsManual { get; set; }
        protected int ProductId { get; set; }
        protected int SupplierProductId { get; set; }

        [Parameter] public bool IsBookingMode { get; set; } = true;
        [Parameter] public EventCallback OnSelectionProcess { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.InventoryRequests, "api/inventory-request-line-details");

            await LoadInventoryRequestLine();
            await LoadInventoryRequest(InventoryRequestLine.InventoryRequestId);
        }

        protected override void OnBeforeRowEditing(InventoryRequestLineDetailWithDetail model = default)
        {
            SelectedProduct = null;
            SelectedStockroom = null;
            SelectedPackageType = null;
            SelectedStockLocation = null;

            LoadProduct();

            if (IsPurchaseOrder)
            {
                LoadStockroom("raw materials");
            }
            else if (IsCustomerOrder)
            {
                LoadStockroom("stock out");
            }
            else if (IsProductionOrder)
            {
                LoadStockroom("finished goods");
            }
            else if (IsProductionRequest)
            {
                LoadStockroom("productions");
            }

            if (DataEditContext.DataItem.Id == 0)
            {
                DataEditContext.InventoryRequestLineId = MasterId;
                if (IsManual)
                {
                    SelectedStockroom = null;
                }
                return;
            }

            Task.Run(async () =>
            {
                var result = await ApiService.GetAsync<StockLocation>(DataEditContext.DataItem.StockLocationId,
                    "api/stock-locations");
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);
                SelectedStockLocation = result;
            });

            Task.Run(async () =>
            {
                var result =
                    await ApiService.GetAsync<Product>(DataEditContext.DataItem.PackageTypeId, "api/products");
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);
                SelectedPackageType = result;
            });
        }

        protected async Task<LoadResult> LoadInventoryRequestLineDetails(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var url = $"api/inventory-request-lines/{MasterId}/details";

            return await LoadDataAsync(url, loadOptions, cancellationToken);
        }

        private async Task LoadInventoryRequest(int id)
        {
            var result = await ApiService.GetAsync<InventoryRequest>(id, "api/inventory-requests");
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            InventoryRequest = result;
        }

        private async Task LoadInventoryRequestLine()
        {
            var result = await ApiService.GetAsync<InventoryRequestLine>(MasterId, "api/inventory-request-lines");
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            InventoryRequestLine = result;
        }

        private void LoadProduct()
        {
            Task.Run(async () =>
            {
                var product = await ApiService.GetAsync<Product>(ProductId, "api/products");
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                SelectedProduct = product;
            });
        }

        private void LoadStockroom(string stockroomName)
        {
            Task.Run(async () =>
            {
                if (string.IsNullOrEmpty(stockroomName)) return;

                var url = $"api/stockrooms/search?columns=name&searchStrings={stockroomName}";
                var collection = await ApiService.GetAllAsync<Stockroom>(url);
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                SelectedStockroom = collection.FirstOrDefault();
            });
        }

        protected async Task<IEnumerable<Product>> SearchProducts(string searchText)
        {
            var url = $"api/products/search?columns=code&searchStrings={searchText}";

            return await ApiService.GetAllAsync<Product>(url);
        }

        protected async Task<IEnumerable<Product>> SearchPackageTypes(string searchText)
        {
            var url = $"api/products/search?columns=i_inventoryTypeId&searchStrings={TgInventoryTypes.PackageTypeBags.Value}_{TgInventoryTypes.PackageTypeBoxes.Value}&columns=code&searchStrings={searchText}";

            return await ApiService.GetAllAsync<Product>(url);
        }

        protected async Task<IEnumerable<Stockroom>> SearchStockrooms(string searchText)
        {
            var url = $"api/stockrooms/search?columns=name&searchStrings={searchText}";

            return await ApiService.GetAllAsync<Stockroom>(url);
        }

        protected async Task<IEnumerable<StockLocation>> SearchStockLocations(string searchText)
        {
            var url = $"api/stock-locations/search?columns=location&searchStrings={searchText}";

            return await ApiService.GetAllAsync<StockLocation>(url);
        }
    }
}