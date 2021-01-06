using System.Threading.Tasks;
using DevExpress.Blazor;
using Microsoft.AspNetCore.Components;
using Tpk.DataServices.Client.Services;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Client.Components
{
    public class DataGridToolbarBase : ComponentBase
    {
        private bool _addButtonVisible = true;
        private string _cssClass = "mb-2";
        private bool _deleteButtonVisible = true;
        private bool _editButtonVisible = true;

        private bool _isAdmin;
        private bool _isBusy;
        private bool _isShowColumnChooser = true;
        private bool _neededToRefresh;
        private TgClaimTypes _requiredClaimType;
        private bool _restoreButtonVisible = true;
        private int _selectedCount;

        protected bool IsAddButtonVisible;
        protected bool IsDeleteButtonVisible;
        protected bool IsEditButtonVisible;
        protected bool IsRestoreButtonVisible;
        [Inject] private IUserService UserService { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public EventCallback<bool> IsBusyChanged { get; set; }
        [Parameter] public EventCallback<TgClaimTypes> RequiredClaimTypeChanged { get; set; }
        [Parameter] public EventCallback<int> SelectedCountChanged { get; set; }
        [Parameter] public EventCallback<bool> IsShowColumnChooserChanged { get; set; }
        [Parameter] public EventCallback<TgDataGridToolbarClickEventArgs> OnToolbarClick { get; set; }
        [Parameter] public EventCallback<string> CssClassChanged { get; set; }
        [Parameter] public EventCallback<bool> NeededToRefreshChanged { get; set; }

        [Parameter]
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (Equals(_isBusy, value)) return;
                _isBusy = value;
                IsBusyChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        [Parameter]
        public TgClaimTypes RequiredClaimType
        {
            get => _requiredClaimType;
            set
            {
                if (Equals(_requiredClaimType, value)) return;
                _requiredClaimType = value;
                RequiredClaimTypeChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        [Parameter]
        public int SelectedCount
        {
            get => _selectedCount;
            set
            {
                if (Equals(_selectedCount, value)) return;
                _selectedCount = value;
                SelectedCountChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        [Parameter]
        public bool IsShowColumnChooser
        {
            get => _isShowColumnChooser;
            set
            {
                if (Equals(_isShowColumnChooser, value)) return;
                _isShowColumnChooser = value;
                IsShowColumnChooserChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        [Parameter]
        public string CssClass
        {
            get => _cssClass;
            set
            {
                if (Equals(_cssClass, value)) return;
                _cssClass = value;
                CssClassChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        [Parameter]
        public bool NeededToRefresh
        {
            get => _neededToRefresh;
            set
            {
                if (Equals(_neededToRefresh, value)) return;
                _neededToRefresh = value;
                if (_neededToRefresh) StateHasChanged();

                NeededToRefreshChanged.InvokeAsync(false);
            }
        }

        [Parameter]
        public bool AddButtonVisible
        {
            get => _addButtonVisible;
            set
            {
                if (Equals(_addButtonVisible, value)) return;
                _addButtonVisible = value;
                StateHasChanged();
            }
        }

        [Parameter]
        public bool EditButtonVisible
        {
            get => _editButtonVisible;
            set
            {
                if (Equals(_editButtonVisible, value)) return;
                _editButtonVisible = value;
                StateHasChanged();
            }
        }

        [Parameter]
        public bool DeleteButtonVisible
        {
            get => _deleteButtonVisible;
            set
            {
                if (Equals(_deleteButtonVisible, value)) return;
                _deleteButtonVisible = value;
                StateHasChanged();
            }
        }

        [Parameter]
        public bool RestoreButtonVisible
        {
            get => _restoreButtonVisible;
            set
            {
                if (Equals(_restoreButtonVisible, value)) return;
                _restoreButtonVisible = value;
                StateHasChanged();
            }
        }

        [Parameter] public bool ReadOnly { get; set; }
        [Parameter] public bool IsPermanent { get; set; }

        [Parameter] public bool CanCreate { get; set; } = true;
        [Parameter] public bool CanEdit { get; set; } = true;
        [Parameter] public bool CanDelete { get; set; } = true;
        [Parameter] public bool CanRestore { get; set; } = true;

        protected override async Task OnInitializedAsync()
        {
            await UserService.LoadAsync();
            _isAdmin = UserService.IsAdminLevel;

            var permission = await UserService.GetPermissionAsync(RequiredClaimType);

            IsAddButtonVisible = _isAdmin || permission.Value >= TgPermissions.Create.Value;
            IsEditButtonVisible = _isAdmin || permission.Value >= TgPermissions.Update.Value;
            IsDeleteButtonVisible = _isAdmin || permission.Value >= TgPermissions.Delete.Value;
            IsRestoreButtonVisible = _isAdmin;
        }

        protected void OnAddButtonClick(ToolbarItemClickEventArgs args)
        {
            var eventArgs = new TgDataGridToolbarClickEventArgs
            {
                Name = "Add",
                MouseEventArgs = args.MouseEventArgs
            };
            OnToolbarClick.InvokeAsync(eventArgs);
        }

        protected void OnEditButtonClick(ToolbarItemClickEventArgs args)
        {
            var eventArgs = new TgDataGridToolbarClickEventArgs
            {
                Name = "Edit",
                MouseEventArgs = args.MouseEventArgs
            };
            OnToolbarClick.InvokeAsync(eventArgs);
        }

        protected void OnDeleteButtonClick(ToolbarItemClickEventArgs args)
        {
            var eventArgs = new TgDataGridToolbarClickEventArgs
            {
                Name = "Delete",
                MouseEventArgs = args.MouseEventArgs
            };
            OnToolbarClick.InvokeAsync(eventArgs);
        }

        protected void OnRestoreButtonClick(ToolbarItemClickEventArgs args)
        {
            var eventArgs = new TgDataGridToolbarClickEventArgs
            {
                Name = "Restore",
                MouseEventArgs = args.MouseEventArgs
            };
            OnToolbarClick.InvokeAsync(eventArgs);
        }

        protected void OnFilterButtonClick(ToolbarItemClickEventArgs args)
        {
            var eventArgs = new TgDataGridToolbarClickEventArgs
            {
                Name = "Filter",
                MouseEventArgs = args.MouseEventArgs
            };
            OnToolbarClick.InvokeAsync(eventArgs);
        }
    }
}