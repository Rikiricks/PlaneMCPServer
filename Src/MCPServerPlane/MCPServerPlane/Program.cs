
using MCPServerPlane.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.Sources.Clear();

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

var planeApiKey = builder.Configuration["PlaneAPIKey"];
var baseUrl = builder.Configuration["BaseUrl"];
var workspace = builder.Configuration["Workspace"];
var projectId = builder.Configuration["ProjectId"];

if (string.IsNullOrEmpty(planeApiKey))
    throw new InvalidOperationException("PlaneAPIKey is missing from configuration");
if (string.IsNullOrEmpty(baseUrl))
    throw new InvalidOperationException("BaseUrl is missing from configuration");
if (string.IsNullOrEmpty(workspace))
    throw new InvalidOperationException("Workspace is missing from configuration");
if (string.IsNullOrEmpty(projectId))
    throw new InvalidOperationException("ProjectId is missing from configuration");

builder.Services.AddHttpClient("PlaneClient", client =>
{
    client.BaseAddress = new Uri($"{baseUrl.TrimEnd('/')}/workspaces/{workspace}/");
    client.DefaultRequestHeaders.Add("x-api-key", planeApiKey);
});

builder.Services.AddTransient<PlaneAPIService>(sp =>
{
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    return new PlaneAPIService(httpClientFactory, projectId);
});

builder.Services.AddMcpServer().WithStdioServerTransport().WithToolsFromAssembly();

await builder.Build().RunAsync();


//var planService = builder.Services.BuildServiceProvider().GetRequiredService<PlaneAPIService>();
//var test = planService.GetProjectStatusesAsync().GetAwaiter().GetResult();
//Console.WriteLine(test);