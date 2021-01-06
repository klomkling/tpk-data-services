using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Components
{
    public class WarningModalBase : ComponentBase
    {
        private bool _isShowWarning;
        private string _warningMessage;

        [Parameter]
        public bool IsShowWarning
        {
            get => _isShowWarning;
            set
            {
                if (Equals(_isShowWarning, value)) return;
                _isShowWarning = value;
                IsShowWarningChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        [Parameter] public EventCallback<bool> IsShowWarningChanged { get; set; }

        [Parameter]
        public string WarningMessage
        {
            get => _warningMessage;
            set
            {
                if (Equals(_warningMessage, value)) return;
                _warningMessage = value;
                WarningMessageChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        [Parameter] public EventCallback<string> WarningMessageChanged { get; set; }

        [Parameter] public EventCallback<MouseEventArgs> CancelWarningClick { get; set; }
        [Parameter] public EventCallback<MouseEventArgs> ConfirmWarningClick { get; set; }
        [Parameter] public EventCallback<TgWarningModalEventArgs> WarningResponse { get; set; }

        protected void OnCancelWarningButtonClick(MouseEventArgs args)
        {
            CancelWarningClick.InvokeAsync(args);
            WarningResponse.InvokeAsync(new TgWarningModalEventArgs
            {
                IsConfirm = false,
                MouseEventArgs = args
            });
            IsShowWarning = false;
        }

        protected void OnConfirmWarningButtonClick(MouseEventArgs args)
        {
            ConfirmWarningClick.InvokeAsync(args);
            WarningResponse.InvokeAsync(new TgWarningModalEventArgs
            {
                IsConfirm = true,
                MouseEventArgs = args
            });
            IsShowWarning = false;
        }
    }
}