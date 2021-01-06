using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Tpk.DataServices.Shared.Data.Enums;
using Tpk.DataServices.Shared.Interfaces;

namespace Tpk.DataServices.Client.Classes.Impl
{
    public class TgImportComponentBase<TModel, TContext, TDetail> : TgMinimalComponentBase
    {
        public IEnumerable<TDetail> DetailCollection { get; set; }
        protected TContext DataEditContext { get; set; }
        protected bool ImportClicked { get; set; }
        protected bool ConfirmClicked { get; set; }
        protected ComponentMode ComponentMode = ComponentMode.Add;

        [Parameter] public EventCallback<MouseEventArgs> ImportCancelled { get; set; }
        [Parameter] public EventCallback<bool> ImportCompleted { get; set; }

        protected void CancelImportClick(MouseEventArgs args)
        {
            ClearModel();
            ImportCancelled.InvokeAsync(args);
        }

        protected virtual Task HandleInvalidSubmit(EditContext editContext)
        {
            Console.WriteLine("Invalid Submit");
            var messages = editContext.GetValidationMessages();
            foreach (var message in messages) Console.WriteLine(message);
            return Task.CompletedTask;
        }

        protected async Task HandleValidSubmit(EditContext editContext)
        {
            if (ImportClicked) return;
            
            ImportClicked = true;

            await ProcessingImportData();

            ComponentMode = ComponentMode.List;
            
            ImportClicked = false;
        }

        protected virtual Task ProcessingImportData()
        {
            return Task.CompletedTask;
        }

        protected virtual Task ConfirmImport()
        {
            if (ConfirmClicked) return Task.CompletedTask;
            ConfirmClicked = true;
            
            OnConfirmImport();
            ImportCompleted.InvokeAsync(true);
            ClearModel();

            ConfirmClicked = false;
            
            return Task.CompletedTask;
        }

        protected virtual Task OnConfirmImport()
        {
            return Task.CompletedTask;
        }

        protected virtual Task CancelConfirmImport()
        {
            OnCancelConfirmImport();
            ClearModel();
            ComponentMode = ComponentMode.Add;
            
            return Task.CompletedTask;
        }

        protected virtual Task OnCancelConfirmImport()
        {
            return Task.CompletedTask;
        }

        protected virtual void ClearModel()
        {
            DataEditContext = (TContext) Activator.CreateInstance(typeof(TContext));
            ((IEditContextBase<TModel>) DataEditContext).StateHasChanged += () => { InvokeAsync(StateHasChanged); };
            DetailCollection = default;

            ImportClicked = false;

            StateHasChanged();
        }

        protected virtual void OnHtmlRowDecoration(DataGridHtmlRowDecorationEventArgs<TDetail> args)
        {
        }
    }
}