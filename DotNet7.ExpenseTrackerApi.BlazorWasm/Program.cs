using DotNet7.ExpenseTrackerApi.BlazorWasm;
using DotNet7.ExpenseTrackerApi.BlazorWasm.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RestSharp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped(n =>
{
    return new RestClient("https://localhost:7185");
});

builder.Services.AddScoped<RestClientService>();

await builder.Build().RunAsync();
