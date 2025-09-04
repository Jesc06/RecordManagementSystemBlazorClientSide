using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RecordManagementSystemClientSide.DTO;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using System.Net.Http.Headers;


namespace RecordManagementSystemClientSide.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJSRuntime _jsRuntime;
        public AuthService(HttpClient httpClient, IHttpClientFactory httpClientFactory, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _httpClientFactory = httpClientFactory;
            _jsRuntime = jsRuntime;
        }

        public async Task<string> login(LoginDTO loginDto)
        {
            var http = _httpClientFactory.CreateClient("API");
            var response = await http.PostAsJsonAsync("api/LoginRegister/Login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<JwtToken>();

                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "refreshToken", result.RefreshToken);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "tokenExpiry", DateTime.UtcNow.AddSeconds(result.ExpiresIn).ToString("o"));

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
                return result.Token;
            }
            return null;
        }


        
        public async Task EnsureValidToken()
        {
            var expiryStr = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "refreshToken");
            if (!DateTime.TryParse(expiryStr, out var expiry)) return;
            if (DateTime.UtcNow >= expiry)
            {
                var refreshToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "refreshToken");
                if (string.IsNullOrWhiteSpace(refreshToken)) return;

                var http = _httpClientFactory.CreateClient("API");
                var refreshResponse = await http.PostAsJsonAsync("api/LoginRegister/Refresh Token", new { RefreshToken = refreshToken });

                if (refreshResponse.IsSuccessStatusCode)
                {
                    var result = await refreshResponse.Content.ReadFromJsonAsync<JwtToken>();
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "refreshToken", result.RefreshToken);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "tokenExpiry", DateTime.UtcNow.AddSeconds(result.ExpiresIn).ToString("o"));
                    
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
                }
                else
                {
                    await Logout();
                }

            }
        }
        

        public async Task Logout()
        {
            var http = _httpClientFactory.CreateClient("API");
            await http.PostAsync("api/Account/Logout", null);
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        }


    }
}