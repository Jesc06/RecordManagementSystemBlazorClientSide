using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace RecordManagementSystemClientSide.Security
{
    public class CustomAuthProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        public CustomAuthProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "Jwt");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }


        public void NotifyUserAuthentication(string token)
        {
            var claims = ParseClaimsFromJwt(token);
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "Jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyLogout()
        {
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            NotifyAuthenticationStateChanged(authState);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);

            var claims = new List<Claim>();
            using var doc = JsonDocument.Parse(jsonBytes);
            foreach (var kvp in doc.RootElement.EnumerateObject())
            {
                // Kung number, gawin string representation
                if (kvp.Value.ValueKind == JsonValueKind.Number)
                {
                    claims.Add(new Claim(kvp.Name, kvp.Value.GetRawText()));
                }
                else
                {
                    claims.Add(new Claim(kvp.Name, kvp.Value.ToString()));
                }
            }

            return claims;
        }



        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        
        
    }
}