using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tpk.DataServices.Server.Classes.Impl;
using Tpk.DataServices.Server.Services;
using Tpk.DataServices.Shared.Data.Constants;
using Tpk.DataServices.Shared.Classes;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Controllers
{
    [AuthorizeRequired]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : TgControllerBase<User, User>
    {
        private readonly IUserService _userService;

        public UsersController(IServiceProvider serviceProvider, IUserService userService,
            ILogger<UsersController> logger)
            : base(serviceProvider, userService, logger)
        {
            _userService = userService;
        }

        [HttpGet("search")]
        public override Task<IActionResult> SearchAsync([FromQuery] string[] columns,
            [FromQuery] string[] searchStrings, [FromQuery] string[] orderColumns = null)
        {
            return base.SearchAsync(columns, searchStrings, orderColumns);
        }

        [HttpGet]
        public override async Task<IActionResult> GetAllAsync(DataSourceLoadOptions loadOptions)
        {
            return await base.GetAllAsync(loadOptions);
        }

        [HttpGet("logged-users")]
        [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
        public async Task<IActionResult> GetActiveUsersAsync(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var condition = $"CONVERT (DATE, LastVisited) = CONVERT (DATE, GETDATE ())";
                var collection = await _userService.GetAllAsync<User>(false, null, condition);
                
                if (_userService.IsError) return await ErrorResponse(_userService.Exception.Message);
                
                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpGet("{id:int}")]
        public override async Task<IActionResult> GetByIdAsync(int id)
        {
            return await base.GetByIdAsync(id);
        }

        [HttpGet("{id:int}/claims")]
        public async Task<IActionResult> GetClaimsAsync(int id)
        {
            try
            {
                var user = await _userService.GetAsync<User>(id);
                if (_userService.IsError) return await ErrorResponse(_userService.Exception.Message);

                if (user == null) return NotFound(new {message = "User not found"});

                var claims = _userService.GetUserClaims(user);
                var claimsDictionary = _userService.GetClaimsDictionary(claims);

                return Ok(claimsDictionary);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPost]
        [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
        public override async Task<IActionResult> InsertAsync([FromBody] User model)
        {
            return await base.InsertAsync(model);
        }

        [HttpPatch]
        [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
        public override Task<IActionResult> UpdateAsync([FromBody] User model)
        {
            return base.UpdateAsync(model);
        }
        
        [HttpPatch("update-password")]
        public async Task<IActionResult> UpdatePasswordAsync([FromBody] UpdatePasswordRequest model)
        {
            // Get user
            var user = await _userService.GetByIdPasswordAsync(model.UserId, model.CurrentPassword);
            if (_userService.IsError) return await ErrorResponse(_userService.Exception.Message);

            user.PasswordHash = $"NEW:{model.NewPassword}";
            await _userService.InsertUpdateAsync(user);
            if (IsError) return await ErrorResponse(_userService.Exception.Message);

            model.IsSuccess = true;
            
            return Ok(model);
        }

        [HttpDelete]
        [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
        public override async Task<IActionResult> DeleteAsync([FromBody] int id)
        {
            return await base.DeleteAsync(id);
        }

        [HttpDelete("bulk-delete")]
        [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
        public override async Task<IActionResult> BulkDeleteAsync([FromBody] IReadOnlyList<int> collection)
        {
            return await base.BulkDeleteAsync(collection);
        }

        [HttpPut]
        [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
        public override async Task<IActionResult> RestoreAsync([FromBody] int id)
        {
            return await base.RestoreAsync(id);
        }

        [HttpPut("bulk-restore")]
        [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
        public override async Task<IActionResult> BulkRestoreAsync([FromBody] IReadOnlyList<int> collection)
        {
            return await base.BulkRestoreAsync(collection);
        }

        [HttpPost("unique-validation")]
        public override async Task<IActionResult> IsUniqueAsync([FromBody] ValidationRequest model)
        {
            return await base.IsUniqueAsync(model);
        }

        #region UserPermissions

        [HttpGet("{id:int}/permissions")]
        [ClaimRequired(RestrictItems.Users, RestrictActions.View)]
        public async Task<IActionResult> GetUserPermissionsByUserIdAsync(int id,
            [FromQuery] DataSourceLoadOptions loadOptions)
        {
            try
            {
                var collection = await _userService.GetAllUserPermissionByUserIdAsync(id, IsAdmin == false);

                if (_userService.IsError) return await ErrorResponse(_userService.Exception.Message);

                var result = await Task.Run(() => DataSourceLoader.Load(collection.AsQueryable(), loadOptions));
                return Ok(result);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPost("permissions")]
        [ClaimRequired(RestrictItems.Users, RestrictActions.Create)]
        public async Task<IActionResult> InsertUserPermissionAsync([FromBody] UserPermission model)
        {
            try
            {
                var result = await _userService.InsertUpdateUserPermissionAsync(model);

                if (_userService.IsError == false) return Ok(result);

                return await ErrorResponse(_userService.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPatch("permissions")]
        [ClaimRequired(RestrictItems.Users, RestrictActions.Update)]
        public async Task<IActionResult> UpdateUserPermissionAsync([FromBody] UserPermission model)
        {
            try
            {
                var result = await _userService.InsertUpdateUserPermissionAsync(model);

                if (_userService.IsError == false) return Ok(result);

                return await ErrorResponse(_userService.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpDelete("permissions")]
        [ClaimRequired(RestrictItems.Users, RestrictActions.Delete)]
        public async Task<IActionResult> DeleteUserPermissionAsync([FromBody] int id)
        {
            try
            {
                if (IsAdmin == false)
                {
                    var result = await _userService.SoftDeleteUserPermissionAsync(id);

                    if (_userService.IsError == false) return Ok(result);

                    return await ErrorResponse(_userService.Exception.Message);
                }
                else
                {
                    var result = await _userService.DeleteUserPermissionAsync(id);
                    if (_userService.IsError) return await ErrorResponse(_userService.Exception.Message);

                    if (result > 0) return Ok(result);

                    var model = await _userService.SoftDeleteUserPermissionAsync(id);

                    if (_userService.IsError == false) return Ok(model);

                    return await ErrorResponse(_userService.Exception.Message);
                }
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpDelete("permissions/bulk-delete")]
        [ClaimRequired(RestrictItems.Users, RestrictActions.Delete)]
        public async Task<IActionResult> BulkDeleteUserPermissionAsync([FromBody] IReadOnlyList<int> collection)
        {
            try
            {
                if (IsAdmin == false)
                {
                    var result = await _userService.BulkSoftDeleteUserPermissionAsync(collection);

                    if (_userService.IsError == false) return Ok(result);

                    return await ErrorResponse(_userService.Exception.Message);
                }
                else
                {
                    var result = await _userService.BulkDeleteUserPermissionAsync(collection);

                    if (_userService.IsError) return await ErrorResponse(_userService.Exception.Message);

                    if (result == collection.Count) return Ok(result);

                    result = await _userService.BulkSoftDeleteUserPermissionAsync(collection);

                    if (_userService.IsError == false) return Ok(result);

                    return await ErrorResponse(_userService.Exception.Message);
                }
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPut("permissions/{id:int}")]
        [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
        public async Task<IActionResult> RestoreUserPermissionAsync([FromBody] int id)
        {
            try
            {
                var result = await _userService.RestoreUserPermissionAsync(id);

                if (_userService.IsError == false) return Ok(result);

                return await ErrorResponse(_userService.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPut("permissions/bulk-restore")]
        [AuthorizeRequired(RestrictRoles.Administrator, RestrictRoles.Director)]
        public async Task<IActionResult> BulkRestoreUserPermissionAsync([FromBody] IReadOnlyList<int> collection)
        {
            try
            {
                var result = await _userService.BulkRestoreUserPermissionAsync(collection);

                if (_userService.IsError == false) return Ok(result);

                return await ErrorResponse(_userService.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        [HttpPost("permissions/unique-validation")]
        public async Task<IActionResult> IsUniqueUserPermissionAsync([FromBody] ValidationRequest model)
        {
            try
            {
                var sql =
                    $"SELECT COUNT (1) FROM dbo.UserPermissions WHERE {model.KeyColumn.ColumnName} <> @{model.KeyColumn.ColumnName}";
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

                var found = await _userService.QueryFirstOrDefaultAsync<int>(sql, parameters);

                if (_userService.IsError == false) return Ok(new ValidationResponse {IsUnique = found == 0});

                return await ErrorResponse(_userService.Exception.Message);
            }
            catch (Exception e)
            {
                return await ErrorResponse(e.Message);
            }
        }

        #endregion
    }
}