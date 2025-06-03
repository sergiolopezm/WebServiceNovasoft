using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebServiceNovasoft;
using WebServiceNovasoft.Services.Interface;
using WebServiceNovasoft.Services.Api;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// CAMBIO IMPORTANTE: Ahora apunta al servidor local (proxy)
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// Registrar servicios (igual que antes)
builder.Services.AddScoped<INovaSoftAuthService, NovaSoftAuthService>();
builder.Services.AddScoped<INovaSoftAccountService, NovaSoftAccountService>();

await builder.Build().RunAsync();