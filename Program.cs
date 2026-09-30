using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BabylonKnowledgeIncremental;
using BabylonKnowledgeIncremental.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Core application state — scoped so all components share the same instance per session
builder.Services.AddScoped<StudySessionState>();

await builder.Build().RunAsync();
