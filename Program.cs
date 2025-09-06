using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RecordManagementSystemClientSide;
using RecordManagementSystemClientSide.Services;
using RecordManagementSystemClientSide.Security;


var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddHttpClient("API", option =>
{ 
    option.BaseAddress = new Uri("https://localhost:7253");
} );

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthProvider>();
builder.Services.AddAuthorizationCore();



await builder.Build().RunAsync();
