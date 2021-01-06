using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tpk.DataServices.Server.Services;
using Tpk.DataServices.Shared.Classes;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Classes.Impl
{
    public class TgControllerBase<TView, TModel> : ControllerBase, ITgControllerBase<TModel>
    {
        private readonly ILogger<TgControllerBase<TView, TModel>> _logger;
        private readonly IServiceBase<TModel> _serviceBase;

        public TgControllerBase(IServiceProvider serviceProvider, IServiceBase<TModel> serviceBase,
            ILogger<TgControllerBase<TView, TModel>> logger)
        {
            var roleService = (IRoleService) serviceProvider.GetService(typeof(IRoleService));
            _serviceBase = serviceBase;
            _logger = logger;
            IsAdmin = roleService.IsAdmin(serviceBase.CurrentUser);
        }

        protected bool IsAdmin { get; }

        public bool IsError { get; protected set; }
        public string ErrorMessage { get; protected set; }

        public virtual async Task<IActionResult> SearchAsync(string[] columns, string[] searchStrings,
            [FromQuery] string[] orderColumns = null)
        {
            try
            {
                var conditions = new List<string>();
                var idx = 0;
                foreach (var column in columns)
                {
                    string condition = null;
                    string columnName = null;
                    var value = searchStrings[idx];
                    var i = column.IndexOf('_');
                    if (i >= 0)
                    {
                        var opr = column.Substring(0, i).ToLower();
                        columnName = column.Substring(i + 1);

                        condition = opr switch
                        {
                            // In
                            "i" => column.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase) ||
                                   column.EndsWith("Number", StringComparison.InvariantCultureIgnoreCase)
                                ? $"{columnName} IN ({(string.IsNullOrEmpty(value) ? "0" : value.Replace("_", ","))})"
                                : $"{columnName} IN ('{value.Replace("_", "','")}')",
                            // Not in
                            "ni" => column.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase) ||
                                    column.EndsWith("Number", StringComparison.InvariantCultureIgnoreCase)
                                ? $"{columnName} NOT IN ({(string.IsNullOrEmpty(value) ? "0" : value.Replace("_", ","))})"
                                : $"{columnName} NOT IN ('{value.Replace("_", "','")}')",
                            // Match
                            "m" => column.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase) ||
                                   column.EndsWith("Number", StringComparison.InvariantCultureIgnoreCase)
                                ? $"{columnName} = {(string.IsNullOrEmpty(value) ? "0" : value)}"
                                : $"{columnName} = '{value}'",
                            // Not match
                            "nm" => column.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase) ||
                                    column.EndsWith("Number", StringComparison.InvariantCultureIgnoreCase)
                                ? $"{columnName} <> {(string.IsNullOrEmpty(value) ? "0" : value)}"
                                : $"{columnName} <> '{value}'",
                            // Not equal
                            "neq" => column.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase) ||
                                     column.EndsWith("Number", StringComparison.InvariantCultureIgnoreCase)
                                ? $"{columnName} <> {(string.IsNullOrEmpty(value) ? "0" : value)}"
                                : $"{columnName} NOT LIKE '%{value}%'",
                            // Greater than
                            "gt" => column.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase) ||
                                    column.EndsWith("Number", StringComparison.InvariantCultureIgnoreCase)
                                ? $"{columnName} > {(string.IsNullOrEmpty(value) ? "0" : value)}"
                                : $"{columnName} LIKE '{value}%'",
                            // Greater than or equal
                            "ge" => column.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase) ||
                                    column.EndsWith("Number", StringComparison.InvariantCultureIgnoreCase)
                                ? $"{columnName} >= {(string.IsNullOrEmpty(value) ? "0" : value)}"
                                : $"{columnName} LIKE '{value}%'",
                            // Less than
                            "lt" => column.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase) ||
                                    column.EndsWith("Number", StringComparison.InvariantCultureIgnoreCase)
                                ? $"{columnName} < {(string.IsNullOrEmpty(value) ? "0" : value)}"
                                : $"{columnName} LIKE '%{value}'",
                            // Less than or equal
                            "le" => column.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase) ||
                                    column.EndsWith("Number", StringComparison.InvariantCultureIgnoreCase)
                                ? $"{columnName} <> {(string.IsNullOrEmpty(value) ? "0" : value)}"
                                : $"{columnName} LIKE '%{value}'",
                            // Otherwise
                            _ => column.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase) ||
                                 column.EndsWith("Number", StringComparison.InvariantCultureIgnoreCase)
                                ? $"{columnName} = {(string.IsNullOrEmpty(value) ? "0" : value)}"
                                : $"{columnName} LIKE '%{value}%'"
                        };
                    }
                    else
                    {
                        columnName = column;
                        var searchText = searchStrings[idx];
                        if (string.IsNullOrEmpty(searchText) == false &&
                            searchText.Equals("NULL", StringComparison.InvariantCultureIgnoreCase))
                        {
                            condition = $"{columnName} IS NULL";
                        }
                        else
                        {
                            condition = column.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase) ||
                                        column.EndsWith("Number", StringComparison.InvariantCultureIgnoreCase)
                                ? $"{columnName} = {searchStrings[idx]}"
                                : $"{columnName} LIKE '%{searchStrings[idx]}%'";
                        }
                    }

                    conditions.Add(condition);
                    idx++;
                }

                string orderClause = null;
                if (orderColumns != null)
                {
                    var orders = new List<string>();
                    foreach (var column in orderColumns)
                    {
                        var direction = column.Substring(0, 2);
                        if (direction.EndsWith("_", StringComparison.InvariantCultureIgnoreCase) == false)
                            orders.Add(column);
                        else
                            orders.Add(direction.Equals("D_", StringComparison.InvariantCultureIgnoreCase)
                                ? $"{column.Substring(2)} DESC"
                                : $"{column.Substring(2)}");
                    }

                    orderClause = string.Join(", ", orders);
                }

                // var condition = $"{column} LIKE '{search}%'";
                var collection =
                    await _serviceBase.SearchAsync<TView>(IsAdmin == false, orderClause, conditions.ToArray());

                if (_serviceBase.IsError == false) return Ok(collection);

                return await ErrorResponse(_serviceBase.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        public virtual async Task<IActionResult> GetAllAsync([FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var collection = await _serviceBase.GetAllAsync<TModel>(IsAdmin == false);

                if (_serviceBase.IsError) return await ErrorResponse(_serviceBase.Exception.Message);

                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);
            }
            catch (Exception e)
            {
                IsError = true;
                ErrorMessage = e.Message;
                _logger.LogError(ErrorMessage);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new {message = ErrorMessage});
            }
        }

        public virtual async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var result = await _serviceBase.GetAsync<TModel>(id, IsAdmin == false);

                if (_serviceBase.IsError == false) return result == null ? NotFound() : (IActionResult) Ok(result);

                return await ErrorResponse(_serviceBase.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        public virtual async Task<IActionResult> InsertAsync(TModel model)
        {
            try
            {
                if (TryValidateModel(model) == false) return await ErrorResponse("Validation fail!");

                var result = await _serviceBase.InsertUpdateAsync(model);

                if (_serviceBase.IsError == false) return Ok(result);

                return await ErrorResponse(_serviceBase.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        public virtual async Task<IActionResult> UpdateAsync(TModel model)
        {
            try
            {
                if (TryValidateModel(model) == false) return await ErrorResponse("Validation fail!");

                var result = await _serviceBase.InsertUpdateAsync(model);

                if (_serviceBase.IsError == false) return Ok(result);

                return await ErrorResponse(_serviceBase.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        public virtual async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                if (IsAdmin == false)
                {
                    var result = await _serviceBase.SoftDeleteAsync(id);

                    if (_serviceBase.IsError == false) return Ok(result);

                    return await ErrorResponse(_serviceBase.Exception.Message);
                }
                else
                {
                    var result = await _serviceBase.DeleteAsync(id);
                    if (_serviceBase.IsError) return await ErrorResponse(_serviceBase.Exception.Message);

                    if (result > 0) return Ok(result);

                    var model = await _serviceBase.SoftDeleteAsync(id);

                    if (_serviceBase.IsError == false) return Ok(model);

                    return await ErrorResponse(_serviceBase.Exception.Message);
                }
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        public virtual async Task<IActionResult> BulkDeleteAsync([FromBody] IReadOnlyList<int> collection)
        {
            try
            {
                int result;
                if (IsAdmin == false)
                {
                    // If not Admin just perform BulkSoftDelete only
                    result = await _serviceBase.BulkSoftDeleteAsync(collection);

                    if (_serviceBase.IsError == false) return Ok(result);

                    return await ErrorResponse(_serviceBase.Exception.Message);
                }

                // Try to delete
                result = await _serviceBase.BulkDeleteAsync(collection);

                if (_serviceBase.IsError) return await ErrorResponse(_serviceBase.Exception.Message);

                // Still remain just perform bulk soft delete
                if (result == collection.Count) return Ok(result);

                result = await _serviceBase.BulkSoftDeleteAsync(collection);

                if (_serviceBase.IsError == false) return Ok(result);

                return await ErrorResponse(_serviceBase.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        public virtual async Task<IActionResult> RestoreAsync(int id)
        {
            try
            {
                var result = await _serviceBase.RestoreAsync(id);

                if (_serviceBase.IsError == false) return Ok(result);

                return await ErrorResponse(_serviceBase.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        public virtual async Task<IActionResult> BulkRestoreAsync([FromBody] IReadOnlyList<int> collection)
        {
            try
            {
                var result = await _serviceBase.BulkRestoreAsync(collection);

                if (_serviceBase.IsError == false) return Ok(result);

                return await ErrorResponse(_serviceBase.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        public virtual async Task<IActionResult> IsUniqueAsync(ValidationRequest model)
        {
            try
            {
                var sql = $"SELECT COUNT (1) FROM dbo.{_serviceBase.TableName} " +
                          $"WHERE {model.KeyColumn.ColumnName} <> @{model.KeyColumn.ColumnName}";
                sql = model.ValidateColumns.Aggregate(sql,
                    (current, column) => current + $" AND {column.ColumnName} = @{column.ColumnName}");

                /*
                 * Cannot direct convert to dynamic parameters because of jsonelement error
                 */
                var parameters = new DynamicParameters();
                foreach (var column in model.ValidateColumns)
                {
                    var value = PropertyInfoExtensions.CreateValue(column.ColumnTypeName, column.ColumnValue);
                    parameters.Add(column.ColumnName, value);
                }

                var keyValue =
                    PropertyInfoExtensions.CreateValue(model.KeyColumn.ColumnTypeName, model.KeyColumn.ColumnValue);
                parameters.Add(model.KeyColumn.ColumnName, keyValue);

                var found = await _serviceBase.QueryFirstOrDefaultAsync<int>(sql, parameters);

                if (_serviceBase.IsError == false) return Ok(new ValidationResponse {IsUnique = found == 0});

                return await ErrorResponse(_serviceBase.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        protected async Task<IActionResult> ErrorResponse(string errorMessage)
        {
            IsError = true;
            ErrorMessage = errorMessage;
            _logger.LogError(ErrorMessage);
            return await Task.FromResult(StatusCode(StatusCodes.Status500InternalServerError,
                new {message = ErrorMessage}));
        }
    }
}