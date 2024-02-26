using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Graph;
using WASM.UI;
using WASM.UI.Graph;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient for passing into GraphServiceClient constructor
builder.Services.AddScoped(
    sp => new HttpClient { BaseAddress = new Uri("https://graph.microsoft.com/v1.0") });


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMsalAuthentication<RemoteAuthenticationState, RemoteUserAccount>(options =>
{
    var scopes = builder.Configuration.GetValue<string>("GraphScopes");
    if (string.IsNullOrEmpty(scopes))
    {
        Console.WriteLine("WARNING: No permission scopes were found in the GraphScopes app setting. Using default User.Read.");
        scopes = "User.Read";
    }
    foreach (var scope in scopes.Split(';'))
    {
        Console.WriteLine($"Adding {scope} to requested permissions");
        options.ProviderOptions.DefaultAccessTokenScopes.Add(scope);
    }

    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
})
.AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, RemoteUserAccount, GraphUserAccountFactory>();

builder.Services.AddScoped<WASM.UI.Graph.GraphClientFactory>();

await builder.Build().RunAsync();
