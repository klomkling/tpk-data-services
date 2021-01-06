using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Classes;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Components.Transactions
{
    public class InventoryReceiptImportComponentBase 
        : TgImportComponentBase<ImportInventory, ImportInventoryEditContext, ImportInventoryLine>
    {
        private InventoryRequestLine _selectedInventoryRequestLine;
        private StockLocation _selectedStockLocation;
        private Product _selectedPackageType;
        private Stockroom _selectedStockroom;
        private Product _selectedProduct;
        private SupplierProduct _selectedSupplierProduct;
        private int _inventoryRequestLineId;

        protected Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (Equals(_selectedProduct, value)) return;
                _selectedProduct = value;
                StateHasChanged();
            }
        }

        protected SupplierProduct SelectedSupplierProduct
        {
            get => _selectedSupplierProduct;
            set
            {
                if (Equals(_selectedSupplierProduct, value)) return;
                _selectedSupplierProduct = value;
                StateHasChanged();
            }
        }

        protected InventoryRequestLine SelectedInventoryRequestLine
        {
            get => _selectedInventoryRequestLine;
            set
            {
                if (Equals(_selectedInventoryRequestLine, value)) return;
                _selectedInventoryRequestLine = value;
                if (value != null)
                {
                    LoadProduct(value.ProductId ?? 0);
                    LoadSupplierProduct(value.SupplierProductId ?? 0);
                    LoadProductionOrder(value.Id);
                }
                else
                {
                    SelectedProduct = null;
                    SelectedSupplierProduct = null;
                    SelectedProductionOrder = null;
                }

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
                StateHasChanged();
            }
        }

        protected ProductionOrder SelectedProductionOrder { get; set; }

        [Parameter]
        public int InventoryRequestLineId
        {
            get => _inventoryRequestLineId;
            set
            {
                if (Equals(_inventoryRequestLineId, value)) return;
                _inventoryRequestLineId = value;
                LoadInventoryRequestLine();
            }
        }

        [Parameter] public TgInventoryRequestTypes RequestType { get; set; }

        protected override async Task OnInitializedAsync()
        {
            DataEditContext = new ImportInventoryEditContext();

            await InitComponent(TgClaimTypes.InventoryRequestLineDetails, null);

            ClearModel();
        }

        private void LoadInventoryRequestLine()
        {
            Task.Run(async () =>
            {
                SelectedInventoryRequestLine =
                    await ApiService.GetAsync<InventoryRequestLine>(InventoryRequestLineId,
                        "api/inventory-request-lines");
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);
            });
        }

        private async Task LoadProduct(string productCode)
        {
            var url = $"api/products/search?columns=m_code&searchStrings={productCode}";
            var collection = await ApiService.GetAllAsync<Product>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            SelectedProduct = collection?.FirstOrDefault();
        }

        private void LoadProduct(int id)
        {
            if (id == 0)
            {
                SelectedProduct = null;
                return;
            }

            Task.Run(async () =>
            {
                var url = $"api/products";
                var result = await ApiService.GetAsync<Product>(id, url);
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                SelectedProduct = result;
            });
        }

        private async Task LoadSupplierProduct(string productCode)
        {
            var url = $"api/supplier-products/search?columns=m_code&searchStrings={productCode}";
            var collection = await ApiService.GetAllAsync<SupplierProduct>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            SelectedSupplierProduct = collection?.FirstOrDefault();
        }

        private void LoadSupplierProduct(int id)
        {
            if (id == 0)
            {
                SelectedSupplierProduct = null;
                return;
            }

            Task.Run(async () =>
            {
                var url = "api/supplier-products";
                var result = await ApiService.GetAsync<SupplierProduct>(id, url);
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                SelectedSupplierProduct = result;
            });
        }

        private void LoadProductionOrder(int id)
        {
            if (id == 0)
            {
                SelectedProductionOrder = null;
                return;
            }

            Task.Run(async () =>
            {
                var url = $"api/inventory-request-lines/{id}/production-order";
                var result = await ApiService.GetAsync<ProductionOrder>(url);
                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                SelectedProductionOrder = result;
            });
        }

        private async Task LoadStockroom(string stockroomName)
        {
            var url = $"api/stockrooms/search?columns=m_name&searchStrings={stockroomName}";
            var collection = await ApiService.GetAllAsync<Stockroom>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            SelectedStockroom = collection.FirstOrDefault();
        }

        protected async Task<IEnumerable<Stockroom>> SearchStockrooms(string searchText)
        {
            var url = $"api/stockrooms/search?columns=name&searchStrings={searchText}";
            var result = await ApiService.GetAllAsync<Stockroom>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            return result;
        }

        protected async Task<IEnumerable<StockLocation>> SearchStockLocations(string searchText)
        {
            var url = $"api/stock-locations/search?columns=location&searchStrings={searchText}";
            var result = await ApiService.GetAllAsync<StockLocation>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            return result;
        }

        protected async Task<IEnumerable<Product>> SearchPackageTypes(string searchText)
        {
            var url = $"api/products/search?columns=i_inventoryTypeId&searchStrings={TgInventoryTypes.PackageTypeBags.Value}_{TgInventoryTypes.PackageTypeBoxes.Value}&columns=code&searchStrings={searchText}";

            var result = await ApiService.GetAllAsync<Product>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            return result;
        }

        protected override async Task ProcessingImportData()
        {
            try
            {
                var collection =
                    DataEditContext.ImportDetails.Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList();

                var idx = -1;
                foreach (var source in collection)
                {
                    var i = collection.IndexOf(source);
                    string line;
                    ImportInventoryLine model;

                    // Check if duplicate line
                    if (idx++ == i)
                    {
                        line = $"0;;0;;0;;;0;{source};Duplicated data";

                        model = line.ToModel<ImportInventoryLine>();
                        if (model != null)
                        {
                            DetailCollection ??= new List<ImportInventoryLine>();
                            DetailCollection = DetailCollection.Concat(new List<ImportInventoryLine> {model});
                        }

                        continue;
                    }

                    var detail = source.Split(";");

                    // If both of supplier product and product are not provide just next
                    if (SelectedSupplierProduct == null && SelectedProduct == null) continue;

                    // Check supplier product code or product code is match
                    if (SelectedSupplierProduct?.Code.Equals(detail[0], StringComparison.InvariantCultureIgnoreCase) ==
                        false && SelectedProduct?.Code.Equals(detail[0], StringComparison.InvariantCultureIgnoreCase) ==
                        false)
                    {
                        line = $"0;;0;;0;;;0;{source};Product code does not match current importing";

                        model = line.ToModel<ImportInventoryLine>();
                        if (model != null)
                        {
                            DetailCollection ??= new List<ImportInventoryLine>();
                            DetailCollection = DetailCollection.Concat(new List<ImportInventoryLine> {model});
                        }

                        continue;
                    }

                    // find stockroom
                    if (RequestType.Equals(TgInventoryRequestTypes.ManualRequest) == false &&
                        RequestType.Equals(TgInventoryRequestTypes.ManualReturn) == false)
                    {
                        string stockroomName;
                        if (SelectedProduct != null &&
                            (SelectedProduct.InventoryTypeId.Equals(TgInventoryTypes.FinishedGoods.Value) ||
                             SelectedProduct.InventoryTypeId.Equals(TgInventoryTypes.TradingGoods.Value)))
                        {
                            stockroomName = "finished goods";
                        }
                        else if (SelectedProduct != null &&
                                 SelectedProduct.InventoryTypeId.Equals(TgInventoryTypes.RawMaterials.Value))
                        {
                            stockroomName = "raw materials";
                        }
                        else
                        {
                            stockroomName = "scrap";
                        }

                        await LoadStockroom(stockroomName);
                    }

                    if (SelectedStockroom == null) continue;

                    // Everything is ok
                    line = $"{InventoryRequestLineId};{SelectedStockroom.Id};{SelectedStockroom.Name};" +
                           $"{SelectedStockLocation.Id};{SelectedStockLocation.Location};" +
                           $"{SelectedPackageType.Id};{SelectedPackageType.Code};{DataEditContext.PalletNo};" +
                           $"{SelectedProduct?.Id ?? 0};{source};";

                    model = line.ToModel<ImportInventoryLine>();
                    if (model == null) continue;

                    // Check if lot number is not match with production order
                    if (SelectedProductionOrder?.LotNumber.Equals(model.LotNumber,
                        StringComparison.InvariantCultureIgnoreCase) == false)
                    {
                        model.Reason = "Lot number does not match with production order";
                    }

                    DetailCollection ??= new List<ImportInventoryLine>();

                    DetailCollection = DetailCollection.Concat(new List<ImportInventoryLine> {model});
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected override async Task OnConfirmImport()
        {
            if (DetailCollection == null)
            {
                ToastService.ShowWarning("No data to import");
                await Task.CompletedTask;
            }

            var url = $"api/inventory-request-line-details/confirm-import-receipts";
            var done = await ApiService.PostAsync(url, DetailCollection);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            if (done) ToastService.ShowSuccess("Successfully imported");
        }

        protected override async Task OnCancelConfirmImport()
        {
            await base.OnCancelConfirmImport();
            DetailCollection = null;
            SelectedStockLocation = null;
            SelectedPackageType = null;
        }

        protected override void OnHtmlRowDecoration(DataGridHtmlRowDecorationEventArgs<ImportInventoryLine> args)
        {
            if (args.DataItem.IsValid == false)
            {
                args.CssClass = " invalid-import-data";
            }
        }

        protected async Task<IEnumerable<ImportInventoryLine>> LoadDataAsync(CancellationToken cancellationToken)
        {
            return await Task.FromResult(DetailCollection);
        }
    }
}