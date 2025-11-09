using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SkillSnap.Client;
using SkillSnap.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient to use HTTP API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5064") });

// Register services
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserSessionService>(); // State management for user session
builder.Services.AddScoped<PortfolioUserService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<SkillService>();

await builder.Build().RunAsync();
