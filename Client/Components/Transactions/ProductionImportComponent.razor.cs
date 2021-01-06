using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Blazor;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Components.Transactions
{
    public class ProductionImportComponentBase 
        : TgImportComponentBase<ImportInventory, ImportInventoryEditContext, ImportInventoryLine>
    {
        private InventoryRequestLine _selectedInventoryRequestLine;
        private Stockroom _selectedStockroom;

        protected InventoryRequestLine SelectedInventoryRequestLine
        {
            get => _selectedInventoryRequestLine;
            set
            {
                if (Equals(_selectedInventoryRequestLine, value)) return;
                _selectedInventoryRequestLine = value;
                if (value != null)
                {
                    //LoadProduct(value.ProductId ?? 0);
                    //LoadProductionOrder(value.Id);
                }
                else
                {
                    //SelectedProduct = null;
                    //SelectedProductionOrder = null;
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


        protected override async Task OnInitializedAsync()
        {
            await LoadStockroom("Productions");
        }

        private async Task LoadStockroom(string stockroomName)
        {
            var url = $"api/stockrooms/search?columns=m_name&searchStrings={stockroomName}";
            var collection = await ApiService.GetAllAsync<Stockroom>(url);
            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            SelectedStockroom = collection.FirstOrDefault();
        }

        protected override async Task OnCancelConfirmImport()
        {
            await base.OnCancelConfirmImport();
            DetailCollection = null;
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