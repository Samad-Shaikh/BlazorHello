using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WasmJWTAuth.Services;

namespace WasmJWTAuth;

public class Program
{
  public static async Task Main(string[] args)
  {
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");

    builder.Services
      .AddScoped<IAuthenticationService, AuthenticationService>()
      .AddScoped<IUserService, UserService>()
      .AddScoped<IHttpService, HttpService>()
      .AddScoped<ILocalStorageService, LocalStorageService>();

    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://127.0.0.1:4000") });

    var host = builder.Build();

    var authenticationService = host.Services
      .GetRequiredService<IAuthenticationService>();
    await authenticationService.Initialize();

    await host.RunAsync();
  }
}