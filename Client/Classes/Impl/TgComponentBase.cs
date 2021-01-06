using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Blazor;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using Tpk.DataServices.Shared.Classes;
using Tpk.DataServices.Shared.Data.Enums;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Interfaces;

namespace Tpk.DataServices.Client.Classes.Impl
{
    public class TgComponentBase<TView, TModel, TContext> : TgMinimalComponentBase
    {
        #region Private properties

        protected static string SavedCollectionName
        {
            get
            {
                var list = new List<TModel>();
                return $"{list.GetType().GetGenericArguments()[0].Name}_SavedCollection";
            }
        }

        #endregion

        #region Private fields

        private bool _isMasterBusy;
        private bool _isDetailBusy;
        private bool _isBusy;
        private bool _isRestore;
        private bool _firstRender;
        private int _masterId;
        private int _selectedId;
        private string _toolbarCssClass = "";
        private string _cssClass = "";
        private bool _isComponentBusy;
        private IEnumerable<TView> _selectedCollection;
        private int _selectedCount;
        private bool _refreshDataGrid;

        #endregion

        #region Protected constants

        protected const string KeyFieldName = "Id";
        protected const DataGridSelectAllMode SelectAllMode = DataGridSelectAllMode.Page;
        protected const DataGridSelectionMode SelectionMode = DataGridSelectionMode.MultipleSelectedDataRows;
        protected const int PageSize = 10;

        #endregion

        #region Protected fields

        protected string SavedLayout;
        protected DxDataGrid<TView> Grid;
        protected bool IsShowFilterRow;
        protected bool IsShowWarning;
        protected string WarningMessage;
        protected TContext DataEditContext;
        protected int PageIndex;
        protected int NewInsertedOrderId;
        protected readonly List<int> AllowedPageSizes = new List<int> {5, 10, 20, 50};
        protected ComponentMode ComponentMode = ComponentMode.List;
        private TView _selectedItem;

        #endregion

        #region Protected properties

        protected bool SubmitClick { get; set; }
        protected bool SaveAndClose { get; set; }

        #endregion

        #region Parameter properties

        [Parameter]
        public TView SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (Equals(_selectedItem, value)) return;
                _selectedItem = value;
                SelectedItemChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        [Parameter]
        public bool RefreshDataGrid
        {
            get => _refreshDataGrid;
            set
            {
                if (Equals(_refreshDataGrid, value)) return;
                _refreshDataGrid = value;
                if (_refreshDataGrid)
                {
                    Grid.Refresh();
                    _refreshDataGrid = false;
                }

                RefreshDataGridChanged.InvokeAsync(_refreshDataGrid);
                StateHasChanged();
            }
        }

        [Parameter] public bool ShowFormWithTable { get; set; }

        [Parameter]
        public bool FirstRender
        {
            get => _firstRender;
            set
            {
                if (Equals(_firstRender, value)) return;
                _firstRender = value;
                FirstRenderChanged.InvokeAsync(value);
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
        public IEnumerable<TView> SelectedCollection
        {
            get => _selectedCollection;
            set
            {
                if (Equals(_selectedCollection, value)) return;
                _selectedCollection = value;

                var selectedCollection = _selectedCollection?.ToList() ?? new List<TView>();
                SelectedCount = selectedCollection.Count;
                CanRestore = false;
                if (SelectedCount == 1)
                {
                    var element = selectedCollection.FirstOrDefault();
                    SelectedItem = element;
                    SelectedId = ((ITgModelBase) element)?.Id ?? 0;
                    CanRestore = string.IsNullOrEmpty(((ITgModelBase) element)?.DeletedStatus) == false;
                }
                else
                {
                    if (SelectedCount == 0)
                    {
                        Grid.ClearSelection();
                        Grid.Refresh();
                    }
                    else
                    {
                        var deletedCount =
                            SelectedCollection?.Count(c =>
                                string.IsNullOrEmpty(((ITgModelBase) c).DeletedStatus) == false) ?? 0;
                        CanRestore = deletedCount > 0;
                    }

                    SelectedItem = default;
                    SelectedId = 0;
                }

                var selection = JsonConvert.SerializeObject(SelectedCollection);
                LocalStorageService.SetItemAsync(SavedCollectionName, selection);

                OnSelectedCollectionChanged(value);

                SelectedCollectionChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        [Parameter]
        public bool IsComponentBusy
        {
            get => _isComponentBusy;
            set
            {
                if (Equals(_isComponentBusy, value)) return;
                _isComponentBusy = value;
                IsComponentBusyChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        [Parameter]
        public bool IsMasterBusy
        {
            get => _isMasterBusy;
            set
            {
                if (Equals(_isMasterBusy, value)) return;
                _isMasterBusy = value;
                IsMasterBusyChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        [Parameter]
        public bool IsDetailBusy
        {
            get => _isDetailBusy;
            set
            {
                if (Equals(_isDetailBusy, value)) return;
                _isDetailBusy = value;
                IsDetailBusyChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

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
        public int MasterId
        {
            get => _masterId;
            set
            {
                if (Equals(_masterId, value)) return;
                _masterId = value;
                MasterIdChanged.InvokeAsync(value);
                Grid?.Refresh();
                StateHasChanged();
            }
        }

        [Parameter]
        public int SelectedId
        {
            get => _selectedId;
            set
            {
                if (Equals(_selectedId, value)) return;
                _selectedId = value;
                SelectedIdChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        [Parameter]
        public string ToolbarCssClass
        {
            get => _toolbarCssClass;
            set
            {
                if (Equals(_toolbarCssClass, value)) return;
                _toolbarCssClass = value;
                ToolbarCssClassChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        [Parameter]
        public string CssClass
        {
            get => string.IsNullOrEmpty(_cssClass) ? "tg-grid" : $"{_cssClass} tg-grid";
            set
            {
                if (Equals(_cssClass, value)) return;
                _cssClass = value;
                CssClassChanged.InvokeAsync(value);
                StateHasChanged();
            }
        }

        #endregion

        #region Parameter events

        [Parameter] public EventCallback<bool> IsMasterBusyChanged { get; set; }
        [Parameter] public EventCallback<bool> IsDetailBusyChanged { get; set; }
        [Parameter] public EventCallback<bool> IsBusyChanged { get; set; }
        [Parameter] public EventCallback<int> MasterIdChanged { get; set; }
        [Parameter] public EventCallback<int> SelectedIdChanged { get; set; }
        [Parameter] public EventCallback<TView> SelectedItemChanged { get; set; }
        [Parameter] public EventCallback<string> ToolbarCssClassChanged { get; set; }
        [Parameter] public EventCallback<string> CssClassChanged { get; set; }
        [Parameter] public EventCallback<bool> IsComponentBusyChanged { get; set; }
        [Parameter] public EventCallback<IEnumerable<TView>> SelectedCollectionChanged { get; set; }
        [Parameter] public EventCallback<int> SelectedCountChanged { get; set; }
        [Parameter] public EventCallback<bool> FirstRenderChanged { get; set; }
        [Parameter] public EventCallback<bool> RefreshDataGridChanged { get; set; }
        [Parameter] public EventCallback<bool> DataGridChanged { get; set; }
        [Parameter] public EventCallback ToolbarAddButtonClicked { get; set; }
        [Parameter] public EventCallback ToolbarEditButtonClicked { get; set; }
        [Parameter] public EventCallback ToolbarDeleteButtonClicked { get; set; }
        [Parameter] public EventCallback ToolbarRestoreButtonClicked { get; set; }

        #endregion

        #region Protected methods

        protected override async Task InitComponent(TgClaimTypes requireClaimType, string apiUrl)
        {
            await base.InitComponent(requireClaimType, apiUrl);

            await CancelUpdateClick();
        }

        protected virtual void OnSelectedCollectionChanged(IEnumerable<TView> collection)
        {
        }

        protected virtual void OnLayoutChanging(IDataGridLayout dataGridLayout)
        {
            FirstRender = true;
        }

        protected virtual void OnLayoutRestoring(IDataGridLayout dataGridLayout)
        {
            if (string.IsNullOrEmpty(SavedLayout) == false) dataGridLayout.LoadLayout(SavedLayout);

            Grid?.ClearSelection();

            var name = SavedCollectionName;
            if (FirstRender)
            {
                FirstRender = false;
                Task.Run(async () => { await LocalStorageService.RemoveItemAsync(name); });
                return;
            }

            Task.Run(async () =>
            {
                var collection = await LocalStorageService.GetItemAsync<IEnumerable<TView>>(name);
                SelectedCollection = collection.AsEnumerable();
                await LocalStorageService.RemoveItemAsync(name);

                await Grid.Refresh();
            });
        }

        protected async Task HandleToolbarResponse(TgDataGridToolbarClickEventArgs args)
        {
            switch (args.Name.ToLower())
            {
                case "add":
                    ToggleProcessSpinner(true);
                    Grid.ClearSelection();
                    SavedLayout = Grid.SaveLayout();

                    if (ToolbarAddButtonClicked.HasDelegate)
                    {
                        await ToolbarAddButtonClicked.InvokeAsync(default);
                    }
                    else
                    {
                        DataEditContext = CreateEditContext(default);
                        OnBeforeRowEditing();

                        ComponentMode = ComponentMode.Add;
                        IsBusy = true;
                    }

                    ToggleProcessSpinner(false);
                    break;

                case "edit":
                    ToggleProcessSpinner(true);
                    SavedLayout = Grid.SaveLayout();

                    if (ToolbarEditButtonClicked.HasDelegate)
                    {
                        await ToolbarEditButtonClicked.InvokeAsync(default);
                    }
                    else
                    {
                        var model = SelectedCollection.FirstOrDefault();
                        DataEditContext = CreateEditContext(model);
                        OnBeforeRowEditing(model);

                        ComponentMode = ComponentMode.Edit;
                        IsBusy = true;
                    }

                    ToggleProcessSpinner(false);
                    break;

                case "delete":
                    if (ToolbarDeleteButtonClicked.HasDelegate)
                    {
                        await ToolbarDeleteButtonClicked.InvokeAsync(default);
                    }
                    else
                    {
                        _isRestore = false;
                        WarningMessage =
                            $"Are you sure to delete selected row{(SelectedCount > 1 ? "s" : string.Empty)}?";
                        IsShowWarning = true;
                        IsBusy = true;
                    }

                    break;

                case "restore":
                    if (ToolbarRestoreButtonClicked.HasDelegate)
                    {
                        await ToolbarRestoreButtonClicked.InvokeAsync(default);
                    }
                    else
                    {
                        _isRestore = true;
                        WarningMessage =
                            $"Are you sure to restore selected row{(SelectedCount > 1 ? "s" : string.Empty)}?";
                        IsShowWarning = true;
                        IsBusy = true;
                    }

                    break;

                case "filter":
                    IsShowFilterRow = !IsShowFilterRow;
                    break;
            }

            await Task.CompletedTask;
        }

        protected async Task HandleWarningResponse(TgWarningModalEventArgs args)
        {
            ToggleProcessSpinner(true);
            SavedLayout = Grid.SaveLayout();

            if (args.IsConfirm)
            {
                var collection = SelectedIdCollection().ToList();
                if (collection.Count == 0) return;

                if (collection.Count > 1)
                {
                    if (_isRestore)
                        await OnBulkRestoreAsync(collection);
                    else
                        await OnBulkDeleteAsync(collection);
                }
                else
                {
                    var model = SelectedCollection.FirstOrDefault();
                    if (model != null)
                    {
                        var propertyInfo = model.GetType().GetProperty(KeyFieldName);
                        if (propertyInfo != null)
                        {
                            var id = (int) propertyInfo.GetValue(model);

                            if (_isRestore)
                                await OnRestoreAsync(id);
                            else
                                await OnDeleteAsync(id);
                        }
                    }
                }

                if (ApiService.IsSessionExpired) RedirectToLoginPage();
                if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

                if (_isRestore)
                    ToastService.ShowSuccess("Successfully restore", "Restored");
                else
                    ToastService.ShowInfo("Successfully deleted", "Deleted");

                // SelectedCollection = default;
                Grid.ClearSelection();
                await Grid.Refresh();

                SelectedCount = 0;
                PageIndex = 0;
            }

            IsBusy = false;
            ToggleProcessSpinner(false);
            StateHasChanged();
        }

        protected void OnRowClick(DataGridRowClickEventArgs<TView> args)
        {
            // Handle for double click only
            if (args.MouseEventArgs.Detail != 2) return;

            // If Master or Detail is busy
            if (IsMasterBusy || IsDetailBusy) return;

            // if cannot edit
            if (CanEdit == false) return;

            // if not pass validation
            if (OnValidateRowDoubleClick(args.DataItem) == false) return;

            ToggleProcessSpinner(true);
            StateHasChanged();

            SavedLayout = Grid.SaveLayout();
            DataEditContext = CreateEditContext(args.DataItem);
            SelectedItem = args.DataItem;
            OnBeforeRowEditing(args.DataItem);

            ComponentMode = ComponentMode.Edit;
            IsBusy = true;

            ToggleProcessSpinner(false);
            StateHasChanged();
        }

        protected virtual bool OnValidateRowDoubleClick(TView args)
        {
            return true;
        }

        protected virtual async Task<TModel> OnUpdateAsync()
        {
            var model = await ApiService.UpdateAsync(((IEditContextBase<TModel>) DataEditContext).DataItem);
            if (DataGridChanged.HasDelegate) await DataGridChanged.InvokeAsync(true);

            return model;
        }

        protected virtual async Task<TModel> OnInsertAsync()
        {
            var model = await ApiService.InsertAsync(((IEditContextBase<TModel>) DataEditContext).DataItem);
            if (DataGridChanged.HasDelegate) await DataGridChanged.InvokeAsync(true);

            return model;
        }

        protected virtual async Task OnDeleteAsync(int id)
        {
            await ApiService.DeleteAsync(id);
            if (DataGridChanged.HasDelegate) await DataGridChanged.InvokeAsync(true);
        }

        protected virtual async Task OnRestoreAsync(int id)
        {
            await ApiService.RestoreAsync(id);
            if (DataGridChanged.HasDelegate) await DataGridChanged.InvokeAsync(true);
        }

        protected virtual async Task OnBulkDeleteAsync(IEnumerable<int> collection)
        {
            await ApiService.BulkDeleteAsync(collection);
            if (DataGridChanged.HasDelegate) await DataGridChanged.InvokeAsync(true);
        }

        protected virtual async Task OnBulkRestoreAsync(IEnumerable<int> collection)
        {
            await ApiService.BulkRestoreAsync(collection);
            if (DataGridChanged.HasDelegate) await DataGridChanged.InvokeAsync(true);
        }

        protected virtual Task HandleInvalidSubmit(EditContext editContext)
        {
            Console.WriteLine("Invalid Submit");
            var messages = editContext.GetValidationMessages();
            foreach (var message in messages) Console.WriteLine(message);
            return Task.CompletedTask;
        }

        protected virtual async Task HandleValidSubmit(EditContext editContext)
        {
            if (SubmitClick == false) return;

            if (editContext.Validate() == false) return;

            await PrepareRowForUpdate();

            if (ComponentMode == ComponentMode.Edit)
            {
                var model = await OnUpdateAsync();
                ((IEditContextBase<TModel>) DataEditContext).DataItem = model;
                NewInsertedOrderId = 0;
            }
            else
            {
                var model = await OnInsertAsync();
                ((IEditContextBase<TModel>) DataEditContext).DataItem = model;
                NewInsertedOrderId = Equals(model, default) || Equals(model, null) ? 0 : ((ITgModelBase) model).Id;
            }

            if (ApiService.IsSessionExpired)
            {
                RedirectToLoginPage();
                return;
            }

            if (ApiService.IsError)
            {
                RedirectToErrorPage(ApiService.ErrorMessage);
                return;
            }

            ToastService.ShowSuccess("Successfully save changes");

            await OnAfterInserted();

            if (SaveAndClose == false && ComponentMode == ComponentMode.Add)
            {
                DataEditContext = CreateEditContext(default);
                OnBeforeRowEditing();
                return;
            }

            // SelectedCollection = default;
            Grid.ClearSelection();
            ComponentMode = ComponentMode.List;
            // DataEditContext = default;
            IsBusy = false;
        }

        protected virtual async Task PrepareRowForUpdate()
        {
            ((IEditContextBase<TModel>) DataEditContext).UpdateDataItem();

            await Task.CompletedTask;
        }

        protected virtual void OnBeforeRowEditing(TView model = default)
        {
        }

        protected virtual Task OnAfterInserted()
        {
            return Task.CompletedTask;
        }

        protected async Task CancelUpdateClick()
        {
            IsBusy = false;
            ComponentMode = ComponentMode.List;
            // SelectedCollection = default;
            Grid.ClearSelection();
            await Grid.Refresh();
            DataEditContext = CreateEditContext(default);
        }

        protected virtual async Task<LoadResult> LoadDataAsync(string targetUrl, DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            var result = await ApiService.LoadCustomData(targetUrl, loadOptions, cancellationToken);

            if (ApiService.IsSessionExpired) RedirectToLoginPage();
            if (ApiService.IsError) RedirectToErrorPage(ApiService.ErrorMessage);

            return result;
        }

        #endregion

        #region Private methods

        private void ToggleProcessSpinner(bool isBusy)
        {
            IsComponentBusy = isBusy;
            StateHasChanged();
        }

        protected virtual IEnumerable<int> SelectedIdCollection()
        {
            return (from selected in SelectedCollection
                let propertyInfo = selected.GetType().GetProperty(KeyFieldName)
                where propertyInfo != null
                select (int) propertyInfo.GetValue(selected)).ToList();
        }

        protected TContext CreateEditContext(TView model)
        {
            TContext contextModel;
            if (Equals(model, null) || Equals(model, default))
            {
                contextModel = (TContext) Activator.CreateInstance(typeof(TContext));
            }
            else
            {
                if (typeof(TView) == typeof(TModel))
                {
                    contextModel = (TContext) Activator.CreateInstance(typeof(TContext), model);
                }
                else
                {
                    var origModel = (TModel) Activator.CreateInstance(typeof(TModel), null);
                    origModel.FromViewModel(model);
                    contextModel = (TContext) Activator.CreateInstance(typeof(TContext), origModel);
                }
            }

            ((IEditContextBase<TModel>) contextModel).StateHasChanged += () => { InvokeAsync(StateHasChanged); };

            return contextModel;
        }

        #endregion
    }
}