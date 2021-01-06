using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;

namespace Tpk.DataServices.Shared.Classes
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ISessionStorageService _sessionStorageService;

        public ApiAuthenticationStateProvider(HttpClient httpClient, ISessionStorageService sessionStorageService)
        {
            _httpClient = httpClient;
            _sessionStorageService = sessionStorageService;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var content = await _sessionStorageService.GetItemAsync<string>("userState");
            if (string.IsNullOrEmpty(content))
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));

            var userState = content.ToUserState();
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("bearer", userState.JwtToken);

            return await Task.FromResult(new AuthenticationState(
                new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(userState.JwtToken), "jwt"))));
        }
        
                public void MarkUserAsAuthenticated(string username)
        {
            var authenticatedUser =
                new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim(ClaimTypes.Name, username)}, "apiAuth"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));

            NotifyAuthenticationStateChanged(authState);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);


            // var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            using var stream = new MemoryStream(jsonBytes);
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var keyValuePairs = JsonConvert.DeserializeObject<Dictionary<string, object>>(reader.ReadToEnd());

            keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

            var roleString = roles?.ToString();
            if (string.IsNullOrEmpty(roleString) == false)
            {
                if (roleString.Trim().StartsWith("["))
                {
                    var parsedRoles = JsonConvert.DeserializeObject<string[]>(roleString);

                    claims.AddRange(parsedRoles.Select(parsedRole => new Claim(ClaimTypes.Role, parsedRole)));
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roleString));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value?.ToString() ?? string.Empty)));

            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}