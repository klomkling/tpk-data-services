using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Server.Classes.Impl;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class UserPermissionService : ServiceBase<UserPermission>, IUserPermissionService
    { 
        public UserPermissionService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ILogger<UserPermissionService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
        }

        public IEnumerable<Claim> GetClaims(int userId, bool isActive = true, params string[] conditions)
        {
            var query = $"UserId = {userId}";
            conditions = conditions.Length > 0
                ? (string[]) conditions.Append(query)
                : new[] {$"UserId = {userId}"};

            var collection = GetAll<UserPermission>(isActive, null, conditions);

            return IsError ? null : collection?.ToClaims();
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync(int userId, bool isActive = true,
            params string[] conditions)
        {
            var query = $"UserId = {userId}";
            conditions = conditions.Length > 0
                ? (string[]) conditions.Append(query)
                : new[] {$"UserId = {userId}"};

            var collection = await GetAllAsync<UserPermission>(isActive, null, conditions);

            return IsError ? null : collection?.ToClaims();
        }

        public IEnumerable<UserPermission> GetByUserId(int id, bool isActive = true, params string[] conditions)
        {
            var sql = SelectQueryBuilder(TableName, null, isActive, $"UserId = {id}");
            sql = conditions.Where(condition => string.IsNullOrEmpty(condition) == false)
                .Aggregate(sql, (current, condition) => $"{current} AND {condition}");

            var result = Query<UserPermission>(sql);

            return IsError ? default : result;
        }

        public async Task<IEnumerable<UserPermission>> GetByUserIdAsync(int id, bool isActive = true,
            params string[] conditions)
        {
            var sql = SelectQueryBuilder(TableName, null, isActive, $"UserId = {id}");
            sql = conditions.Where(condition => string.IsNullOrEmpty(condition) == false)
                .Aggregate(sql, (current, condition) => $"{current} AND {condition}");

            var result = await QueryAsync<UserPermission>(sql);

            return IsError ? default : result;
        }

        public UserPermission GetByRestrictObjectId(int id, bool isActive = true)
        {
            var sql = SelectQueryBuilder(TableName, null, isActive, $"RestrictObjectId = {id}");

            var result = QueryFirstOrDefault<UserPermission>(sql);

            return IsError ? default : result;
        }

        public bool SoftDeleteByUserId(int id)
        {
            var sql = $"UPDATE dbo.{TableName} SET " +
                      @"DeletedAt = GETDATE (), " +
                      $"DeletedBy = '{Username}' " +
                      $"WHERE UserId = {id}";

            var result = Execute(sql);

            return !IsError && result >= 0;
        }

        public async Task<bool> SoftDeleteByUserIdAsync(int id)
        {
            var sql = $"UPDATE dbo.{TableName} SET " +
                      @"DeletedAt = GETDATE (), " +
                      $"DeletedBy = '{Username}' " +
                      $"WHERE UserId = {id}";

            var result = await ExecuteAsync(sql);

            return !IsError && result >= 0;
        }

        public bool RestoreByUserId(int id)
        {
            var sql = $"UPDATE dbo.{TableName} SET " +
                      "DeletedAt = NULL, " +
                      "DeletedBy = NULL " +
                      $"WHERE UserId = {id}";

            var result = Execute(sql);

            return !IsError && result >= 0;
        }

        public async Task<bool> RestoreByUserIdAsync(int id)
        {
            var sql = $"UPDATE dbo.{TableName} SET " +
                      "DeletedAt = NULL, " +
                      "DeletedBy = NULL " +
                      $"WHERE UserId = {id}";

            var result = await ExecuteAsync(sql);

            return !IsError && result >= 0;
        }

        public bool DeleteByUserId(int id)
        {
            var sql = $"DELETE FROM dbo.{TableName} " +
                      @"WHERE DeletedAt IS NOT NULL " +
                      @"AND DeletedBy IS NOT NULL " +
                      $"AND UserId = {id}";

            var result = Execute(sql);

            return !IsError && result >= 0;
        }

        public async Task<bool> DeleteByUserIdAsync(int id)
        {
            var sql = $"DELETE FROM dbo.{TableName} " +
                      @"WHERE DeletedAt IS NOT NULL " +
                      @"AND DeletedBy IS NOT NULL " +
                      $"AND UserId = {id}";

            var result = await ExecuteAsync(sql);

            return !IsError && result >= 0;
        }

        public bool ClonePermissions(int fromUserId, int toUserId)
        {
            var sql = ProcedureName("Clone");
            var parameters = new
            {
                SourceUserId = fromUserId,
                TargetUserId = toUserId,
                Requester = Username
            };

            var result = Execute(sql, parameters, CommandType.StoredProcedure);

            return !IsError && result >= 0;
        }

        public async Task<bool> ClonePermissionsAsync(int fromUserId, int toUserId)
        {
            var sql = ProcedureName("Clone");
            var parameters = new
            {
                SourceUserId = fromUserId,
                TargetUserId = toUserId,
                Requester = Username
            };

            var result = await ExecuteAsync(sql, parameters, CommandType.StoredProcedure);

            return !IsError && result >= 0;
        }

        public async Task<UserPermission> GetByRestrictObjectIdAsync(int id, bool isActive = true)
        {
            var sql = SelectQueryBuilder(TableName, null, isActive, $"RestrictObjectId = {id}");

            var result = await QueryFirstOrDefaultAsync<UserPermission>(sql);

            return IsError ? default : result;
        }
    }
}