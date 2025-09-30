using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RecordManagementSystemClientSide.DTO;
using System.Net.Http.Json;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.WebAssembly.Http;


namespace RecordManagementSystemClientSide.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task<string> login(LoginDTO loginDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/LoginRegister/Login", loginDto);

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
    

        public async Task<(string sessionId, DateTime expiry)> RegisterAccount(RegisterAccountDTO registerAccountDTO)
        {
            var response = await _httpClient.PostAsJsonAsync("api/LoginRegister/AddStudentAccount", registerAccountDTO);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OTPResponseDTO>();
            return (result.SessionId, result.ExpiryTime);

        }

        public async Task<bool> VerifyOTP(VerifyOtpDTO verifyOtpDTO)
        {
            var response = await _httpClient.PostAsJsonAsync("api/LoginRegister/VerifyOTP", verifyOtpDTO);
            return response.IsSuccessStatusCode;
        }


        public async Task EnsureValidToken()
        {
            var expiryStr = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "refreshToken");
            if (!DateTime.TryParse(expiryStr, out var expiry)) return;
            if (DateTime.UtcNow >= expiry)
            {
                var refreshToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "refreshToken");
                if (string.IsNullOrWhiteSpace(refreshToken)) return;

                var refreshResponse = await _httpClient.PostAsJsonAsync("api/LoginRegister/Refresh Token", new { RefreshToken = refreshToken });

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
            await _httpClient.PostAsync("api/Account/Logout", null);
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        }
        

    }
}