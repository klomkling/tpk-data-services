using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using Tpk.DataServices.Client.Classes.Impl;
using Tpk.DataServices.Shared.Classes;
using Tpk.DataServices.Shared.Data.EditContexts;
using Tpk.DataServices.Shared.Data.Models;
using Tpk.DataServices.Shared.Data.Views;

namespace Tpk.DataServices.Client.Components.Base
{
    public class ProductComponentBase : TgComponentBase<ProductWithDetail, Product, ProductEditContext>
    {
        protected readonly IEnumerable<string> StatusDropdown = new List<string>
            {"Active", "Discontinued"};

        protected readonly IEnumerable<string> SaleDropdown = new List<string>
            {"For sale", "Not for sale"};

        private TgInventoryTypes _selectedInventoryType;
        private MaterialGrade _selectedMaterialGrade;
        private MaterialSubType _selectedMaterialSubType1;
        private MaterialSubType _selectedMaterialSubType2;
        private MaterialSubType _selectedMaterialSubType3;
        private MaterialSubType _selectedMaterialSubType4;
        private MaterialType _selectedMaterialType;
        private ProductColor _selectedProductColor;
        private ProductCategory _selectedProductCategory;
        private ProductLine _selectedProductLine;
        private ProductUnit _selectedProductUnit;

        protected IEnumerable<TgInventoryTypes> InventoryTypeCollection;

        protected ProductWithDetail ProductWithDetail { get; set; }

        protected TgInventoryTypes SelectedInventoryType
        {
            get => _selectedInventoryType;
            set
            {
                if (Equals(_selectedInventoryType, value)) return;
                _selectedInventoryType = value;
                StateHasChanged();
            }
        }

        protected ProductCategory SelectedProductCategory
        {
            get => _selectedProductCategory;
            set
            {
                if (Equals(_selectedProductCategory, value)) return;
                _selectedProductCategory = value;
                StateHasChanged();
            }
        }

        protected ProductLine SelectedProductLine
        {
            get => _selectedProductLine;
            set
            {
                if (Equals(_selectedProductLine, value)) return;
                _selectedProductLine = value;
                StateHasChanged();
            }
        }

        protected ProductUnit SelectedProductUnit
        {
            get => _selectedProductUnit;
            set
            {
                if (Equals(_selectedProductUnit, value)) return;
                _selectedProductUnit = value;
                StateHasChanged();
            }
        }

        protected MaterialType SelectedMaterialType
        {
            get => _selectedMaterialType;
            set
            {
                if (Equals(_selectedMaterialType, value)) return;
                _selectedMaterialType = value;
                StateHasChanged();
            }
        }

        protected MaterialSubType SelectedMaterialSubType1
        {
            get => _selectedMaterialSubType1;
            set
            {
                if (Equals(_selectedMaterialSubType1, value)) return;
                _selectedMaterialSubType1 = value;
                StateHasChanged();
            }
        }

        protected MaterialSubType SelectedMaterialSubType2
        {
            get => _selectedMaterialSubType2;
            set
            {
                if (Equals(_selectedMaterialSubType2, value)) return;
                _selectedMaterialSubType2 = value;
                StateHasChanged();
            }
        }

        protected MaterialSubType SelectedMaterialSubType3
        {
            get => _selectedMaterialSubType3;
            set
            {
                if (Equals(_selectedMaterialSubType3, value)) return;
                _selectedMaterialSubType3 = value;
                StateHasChanged();
            }
        }

        protected MaterialSubType SelectedMaterialSubType4
        {
            get => _selectedMaterialSubType4;
            set
            {
                if (Equals(_selectedMaterialSubType4, value)) return;
                _selectedMaterialSubType4 = value;
                StateHasChanged();
            }
        }

        protected MaterialGrade SelectedMaterialGrade
        {
            get => _selectedMaterialGrade;
            set
            {
                if (Equals(_selectedMaterialGrade, value)) return;
                _selectedMaterialGrade = value;
                StateHasChanged();
            }
        }

        protected ProductColor SelectedProductColor
        {
            get => _selectedProductColor;
            set
            {
                if (Equals(_selectedProductColor, value)) return;
                _selectedProductColor = value;
                StateHasChanged();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await InitComponent(TgClaimTypes.Products, "api/products");

            InventoryTypeCollection = Enumeration.GetAll<TgInventoryTypes>();
        }

        protected override void OnBeforeRowEditing(ProductWithDetail model = default)
        {
            DataEditContext.IsActive = true;

            SelectedInventoryType = null;
            SelectedProductUnit = null;
            SelectedProductCategory = null;
            SelectedProductLine = null;
            SelectedMaterialType = null;
            SelectedMaterialSubType1 = null;
            SelectedMaterialSubType2 = null;
            SelectedMaterialSubType3 = null;
            SelectedMaterialSubType4 = null;
            SelectedMaterialGrade = null;
            SelectedProductColor = null;

            if (DataEditContext.Id == 0) return;

            SelectedInventoryType =
                InventoryTypeCollection.FirstOrDefault(i => i.Value.Equals(DataEditContext.DataItem.InventoryTypeId));

            Task.Run(async () =>
            {
                SelectedProductUnit =
                    await ApiService.GetAsync<ProductUnit>(DataEditContext.DataItem.ProductUnitId, "api/product-units");
            });

            if (DataEditContext.DataItem?.ProductCategoryId > 0)
                Task.Run(async () =>
                {
                    SelectedProductCategory =
                        await ApiService.GetAsync<ProductCategory>(DataEditContext.DataItem.ProductCategoryId,
                            "api/product-categories");
                });

            if (DataEditContext.DataItem?.ProductLineId > 0)
                Task.Run(async () =>
                {
                    SelectedProductLine =
                        await ApiService.GetAsync<ProductLine>(DataEditContext.DataItem.ProductLineId,
                            "api/product-lines");
                });

            if (DataEditContext.DataItem?.MaterialTypeId > 0)
                Task.Run(async () =>
                {
                    SelectedMaterialType =
                        await ApiService.GetAsync<MaterialType>(DataEditContext.DataItem.MaterialTypeId,
                            "api/material-types");
                });

            if (DataEditContext.DataItem?.MaterialSubType1Id > 0)
                Task.Run(async () =>
                {
                    SelectedMaterialSubType1 =
                        await ApiService.GetAsync<MaterialSubType>(DataEditContext.DataItem.MaterialSubType1Id,
                            "api/material-sub-types");
                });

            if (DataEditContext.DataItem?.MaterialSubType2Id > 0)
                Task.Run(async () =>
                {
                    SelectedMaterialSubType2 =
                        await ApiService.GetAsync<MaterialSubType>(DataEditContext.DataItem.MaterialSubType2Id,
                            "api/material-sub-types");
                });

            if (DataEditContext.DataItem?.MaterialSubType3Id > 0)
                Task.Run(async () =>
                {
                    SelectedMaterialSubType3 =
                        await ApiService.GetAsync<MaterialSubType>(DataEditContext.DataItem.MaterialSubType3Id,
                            "api/material-sub-types");
                });

            if (DataEditContext.DataItem?.MaterialSubType4Id > 0)
                Task.Run(async () =>
                {
                    SelectedMaterialSubType4 =
                        await ApiService.GetAsync<MaterialSubType>(DataEditContext.DataItem.MaterialSubType4Id,
                            "api/material-sub-types");
                });

            if (DataEditContext.DataItem?.MaterialGradeId > 0)
                Task.Run(async () =>
                {
                    SelectedMaterialGrade =
                        await ApiService.GetAsync<MaterialGrade>(DataEditContext.DataItem.MaterialGradeId,
                            "api/material-grades");
                });

            if (DataEditContext.DataItem?.ProductColorId > 0)
                Task.Run(async () =>
                {
                    SelectedProductColor =
                        await ApiService.GetAsync<ProductColor>(DataEditContext.DataItem.ProductColorId,
                            "api/product-colors");
                });
        }

        protected override Task PrepareRowForUpdate()
        {
            DataEditContext.UpdateDataItem();
            DataEditContext.DataItem.InventoryTypeId = SelectedInventoryType.Value;
            DataEditContext.DataItem.ProductUnitId = SelectedProductUnit.Id;
            DataEditContext.DataItem.ProductCategoryId = SelectedProductCategory?.Id;
            DataEditContext.DataItem.ProductLineId = SelectedProductLine?.Id;
            DataEditContext.DataItem.MaterialTypeId = SelectedMaterialType?.Id;
            DataEditContext.DataItem.MaterialSubType1Id = SelectedMaterialSubType1?.Id;
            DataEditContext.DataItem.MaterialSubType2Id = SelectedMaterialSubType2?.Id;
            DataEditContext.DataItem.MaterialSubType3Id = SelectedMaterialSubType3?.Id;
            DataEditContext.DataItem.MaterialSubType4Id = SelectedMaterialSubType4?.Id;
            DataEditContext.DataItem.MaterialGradeId = SelectedMaterialGrade?.Id;
            DataEditContext.DataItem.ProductColorId = SelectedProductColor?.Id;

            return Task.CompletedTask;
        }

        protected async Task<LoadResult> LoadProducts(DataSourceLoadOptionsBase loadOptions,
            CancellationToken cancellationToken)
        {
            return await LoadDataAsync(ApiUrl, loadOptions, cancellationToken);
        }

        protected Task<IEnumerable<TgInventoryTypes>> SearchInventoryTypes(string searchText)
        {
            var response = InventoryTypeCollection.Where(i => i.DisplayName.Contains(searchText));
            return Task.FromResult(response);
        }

        protected async Task<IEnumerable<ProductUnit>> SearchProductUnits(string searchText)
        {
            var response =
                await ApiService.GetAllAsync<ProductUnit>(
                    $"api/product-units/search?columns=code&searchStrings={searchText}");
            return response;
        }

        protected async Task<IEnumerable<ProductCategory>> SearchProductCategories(string searchText)
        {
            var response =
                await ApiService.GetAllAsync<ProductCategory>(
                    $"api/product-categories/search?columns=name&searchStrings={searchText}");
            return response;
        }

        protected async Task<IEnumerable<ProductLine>> SearchProductLines(string searchText)
        {
            var response =
                await ApiService.GetAllAsync<ProductLine>(
                    $"api/product-lines/search?columns=name&searchStrings={searchText}");
            return response;
        }

        protected async Task<IEnumerable<MaterialType>> SearchMaterialTypes(string searchText)
        {
            var response =
                await ApiService.GetAllAsync<MaterialType>(
                    $"api/material-types/search?columns=code&searchStrings={searchText}");
            return response;
        }

        protected async Task<IEnumerable<MaterialSubType>> SearchMaterialSubTypes(string searchText)
        {
            var response =
                await ApiService.GetAllAsync<MaterialSubType>(
                    $"api/material-sub-types/search?columns=code&searchStrings={searchText}");
            return response;
        }

        protected async Task<IEnumerable<MaterialGrade>> SearchMaterialGrades(string searchText)
        {
            var response =
                await ApiService.GetAllAsync<MaterialGrade>(
                    $"api/material-grades/search?columns=code&searchStrings={searchText}");
            return response;
        }

        protected async Task<IEnumerable<ProductColor>> SearchProductColors(string searchText)
        {
            var response =
                await ApiService.GetAllAsync<ProductColor>(
                    $"api/product-colors/search?columns=description&searchStrings={searchText}");
            return response;
        }
    }
}