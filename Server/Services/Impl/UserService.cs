using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tpk.DataServices.Shared.Data.Models;

namespace Tpk.DataServices.Server.Services.Impl
{
    public class UserService : ServiceBase<User>, IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly ITokenService _tokenService;
        private readonly IUserPermissionService _userPermissionService;

        public UserService(IConfiguration configuration, IOptions<AppSettings> options,
            IHttpContextAccessor httpContextAccessor, ITokenService tokenService,
            IUserPermissionService userPermissionService, ILogger<UserService> logger)
            : base(configuration, options, httpContextAccessor, logger)
        {
            _tokenService = tokenService;
            _userPermissionService = userPermissionService;
            _userPermissionService.SetRequester(Username);
            _appSettings = options.Value;

            ExceptColumns = new List<string> {"PasswordHash", "LastVisited"};
        }

        public User GetByIdPassword(int id, string password)
        {
            // get user
            var sql = ProcedureName("VerifyUser");
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            parameters.Add("Password", password, DbType.String);
            parameters.Add("Salt", _appSettings.Salt, DbType.String);

            var user = QueryFirstOrDefault<User>(sql, parameters, CommandType.StoredProcedure);
            return IsError ? default : user;
        }

        public async Task<User> GetByIdPasswordAsync(int id, string password)
        {
            // get user
            var sql = ProcedureName("Verify");
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            parameters.Add("Password", password, DbType.String);
            parameters.Add("Salt", _appSettings.Salt, DbType.String);

            var user = await QueryFirstOrDefaultAsync<User>(sql, parameters, CommandType.StoredProcedure);
            return IsError ? default : user;
        }

        public User GetByUsername(string username, bool isActive = true)
        {
            var sql = SelectQueryBuilder(TableName, null, isActive, "Username = @Username");
            var user = QueryFirstOrDefault<User>(sql, new {Username = username});

            return IsError ? default : user;
        }

        public async Task<User> GetByUsernameAsync(string username, bool isActive = true)
        {
            var sql = SelectQueryBuilder(TableName, null, isActive, "Username = @Username");
            var user = await QueryFirstOrDefaultAsync<User>(sql, new {Username = username});

            return IsError ? default : user;
        }

        public User GetByEmail(string email, bool isActive = true)
        {
            var sql = SelectQueryBuilder(TableName, null, isActive);
            var user = QueryFirstOrDefault<User>(sql, new {Email = email});

            return IsError ? default : user;
        }

        public async Task<User> GetByEmailAsync(string email, bool isActive = true)
        {
            var sql = SelectQueryBuilder(TableName, null, isActive);
            var user = await QueryFirstOrDefaultAsync<User>(sql, new {Email = email});

            return IsError ? default : user;
        }

        public User GetByRefreshToken(string refreshToken)
        {
            var sql = SelectQueryBuilder(TableName, null, true, "RefreshToken = @RefreshToken");
            var user = QueryFirstOrDefault<User>(sql, new {RefreshToken = refreshToken});

            return IsError ? default : user;
        }

        public async Task<User> GetByRefreshTokenAsync(string refreshToken)
        {
            var sql = SelectQueryBuilder(TableName, null, true, "RefreshToken = @RefreshToken");
            var user = await QueryFirstOrDefaultAsync<User>(sql, new {RefreshToken = refreshToken});

            return IsError ? default : user;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            // get user
            var sql = ProcedureName("Authenticate");
            var parameters = new DynamicParameters();
            parameters.Add("Username", model.Username, DbType.String);
            parameters.Add("Password", model.Password, DbType.String);
            parameters.Add("Salt", _appSettings.Salt, DbType.String);

            var user = QueryFirstOrDefault<User>(sql, parameters, CommandType.StoredProcedure);
            if (user == null) return null;

            // authentication successful so get claims from database and generate jwt token together with refresh token
            var claims = GetUserClaims(user)?.ToList();

            // generate jwt token and refresh token
            var jwtToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();


            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            InsertUpdate(user);
            return IsError
                ? null
                : new AuthenticateResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    RoleId = user.RoleId,
                    JwtToken = jwtToken,
                    RefreshToken = refreshToken
                };
        }

        public AuthenticateResponse RefreshToken(TokenRequest model)
        {
            var user = GetByRefreshToken(model.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow) return null;

            var claims = GetUserClaims(user)?.ToList();

            var newToken = _tokenService.GenerateAccessToken(claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            InsertUpdate(user);
            return IsError
                ? null
                : new AuthenticateResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    RoleId = user.RoleId,
                    JwtToken = newToken,
                    RefreshToken = newRefreshToken
                };
        }

        public bool RevokeToken()
        {
            CurrentUser.RefreshToken = null;
            CurrentUser.RefreshTokenExpiryTime = null;

            InsertUpdate(CurrentUser);
            return !IsError;
        }

        public override User InsertUpdate(User model)
        {
            var sql = ProcedureName("AddEdit");
            var parameters = CompactParameters(model);
            parameters.Add(Requester, Username);
            parameters.Add("Salt", _appSettings.Salt);
            parameters.Add("Password", model.PasswordHash);

            using var connection = GetOpenConnection();
            var transaction = connection.BeginTransaction();

            try
            {
                var result = connection.QueryFirstOrDefault<User>(sql, parameters, transaction,
                    commandType: CommandType.StoredProcedure);

                IsError = false;
                transaction.Commit();

                return result;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                IsError = true;
                Exception = ex;

                return default;
            }
            finally
            {
                connection.Close();
            }
        }

        public override async Task<User> InsertUpdateAsync(User model)
        {
            var sql = ProcedureName("AddEdit");
            var parameters = CompactParameters(model);
            parameters.Add(Requester, Username);
            parameters.Add("Salt", _appSettings.Salt);
            parameters.Add("Password", model.PasswordHash);

            await using var connection = GetOpenConnection();
            var transaction = await connection.BeginTransactionAsync();

            try
            {
                var result = await connection.QueryFirstOrDefaultAsync<User>(sql, parameters, transaction,
                    commandType: CommandType.StoredProcedure);

                IsError = false;
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                IsError = true;
                Exception = ex;

                return default;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }

        public IEnumerable<Claim> GetUserClaims(User user, bool isActive = true, params string[] conditions)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            // get claim from database
            var collection = _userPermissionService.GetClaims(user.Id, isActive, conditions);
            if (collection != null) claims.AddRange(collection);

            return claims;
        }

        public IEnumerable<UserClaim> GetClaimsDictionary(IEnumerable<Claim> claims)
        {
            if (claims == null) return default;

            var exceptions = new List<string> {"UserId", "Name"};
            var collection = claims.Where(c =>
                    exceptions.Contains(c.Type, StringComparer.InvariantCultureIgnoreCase) == false)
                .ToList();
            if (collection.Count == 0) return default;

            var claimList = new List<UserClaim>();
            foreach (var claim in collection)
            {
                if (int.TryParse(claim.Type, out var restrictId) == false) continue;
                if (int.TryParse(claim.Value, out var permission) == false) continue;

                claimList.Add(new UserClaim {ClaimId = restrictId, Permission = permission});
            }

            return claimList;
        }

        public IEnumerable<UserPermission> GetAllUserPermissionByUserId(int id, bool isActive = true,
            params string[] conditions)
        {
            var result = _userPermissionService.GetByUserId(id, isActive, conditions);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public async Task<IEnumerable<UserPermission>> GetAllUserPermissionByUserIdAsync(int id,
            bool isActive = true, params string[] conditions)
        {
            var result = await _userPermissionService.GetByUserIdAsync(id, isActive, conditions);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public UserPermission InsertUpdateUserPermission(UserPermission model)
        {
            _userPermissionService.SetRequester(CurrentUser.Username);
            var result = _userPermissionService.InsertUpdate(model);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public async Task<UserPermission> InsertUpdateUserPermissionAsync(UserPermission model)
        {
            _userPermissionService.SetRequester(CurrentUser.Username);
            var result = await _userPermissionService.InsertUpdateAsync(model);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public int RestoreUserPermission(int id)
        {
            var result = _userPermissionService.Restore(id);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public async Task<int> RestoreUserPermissionAsync(int id)
        {
            var result = await _userPermissionService.RestoreAsync(id);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public int SoftDeleteUserPermission(int id)
        {
            _userPermissionService.SetRequester(CurrentUser.Username);
            var result = _userPermissionService.SoftDelete(id);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public async Task<int> SoftDeleteUserPermissionAsync(int id)
        {
            _userPermissionService.SetRequester(CurrentUser.Username);
            var result = await _userPermissionService.SoftDeleteAsync(id);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public int BulkDeleteUserPermission(IEnumerable<int> idCollection)
        {
            var result = _userPermissionService.BulkDelete(idCollection);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public async Task<int> BulkDeleteUserPermissionAsync(IEnumerable<int> idCollection)
        {
            var result = await _userPermissionService.BulkDeleteAsync(idCollection);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public int BulkRestoreUserPermission(IEnumerable<int> idCollection)
        {
            var result = _userPermissionService.BulkRestore(idCollection);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public async Task<int> BulkRestoreUserPermissionAsync(IEnumerable<int> idCollection)
        {
            var result = await _userPermissionService.BulkRestoreAsync(idCollection);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public int BulkSoftDeleteUserPermission(IEnumerable<int> idCollection)
        {
            var result = _userPermissionService.BulkSoftDelete(idCollection);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public async Task<int> BulkSoftDeleteUserPermissionAsync(IEnumerable<int> idCollection)
        {
            var result = await _userPermissionService.BulkSoftDeleteAsync(idCollection);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public int DeleteUserPermission(int id)
        {
            var result = _userPermissionService.Delete(id);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public async Task<int> DeleteUserPermissionAsync(int id)
        {
            var result = await _userPermissionService.DeleteAsync(id);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public bool SoftDeleteUserPermissionByUserId(int id)
        {
            var result = _userPermissionService.SoftDeleteByUserId(id);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public async Task<bool> SoftDeleteUserPermissionByUserIdAsync(int id)
        {
            var result = await _userPermissionService.SoftDeleteByUserIdAsync(id);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public bool RestoreUserPermissionByUserId(int id)
        {
            var result = _userPermissionService.RestoreByUserId(id);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public async Task<bool> RestoreUserPermissionByUserIdAsync(int id)
        {
            var result = await _userPermissionService.RestoreByUserIdAsync(id);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public bool DeleteUserPermissionByUserId(int id)
        {
            var result = _userPermissionService.DeleteByUserId(id);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public async Task<bool> DeleteUserPermissionByUserIdAsync(int id)
        {
            var result = await _userPermissionService.DeleteByUserIdAsync(id);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public bool CloneUserPermission(int fromUserId, int toUserId)
        {
            var result = _userPermissionService.ClonePermissions(fromUserId, toUserId);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }

        public async Task<bool> CloneUserPermissionsAsync(int fromUserId, int toUserId)
        {
            var result = await _userPermissionService.ClonePermissionsAsync(fromUserId, toUserId);
            IsError = _userPermissionService.IsError;
            Exception = _userPermissionService.Exception;
            return result;
        }
    }
}