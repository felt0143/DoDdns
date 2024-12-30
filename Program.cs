using DoDdns.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using HttpClient httpClient = new();
var builder = Host.CreateApplicationBuilder();

builder.Logging.AddConsole();
builder.Services.AddSingleton(httpClient);
builder.Services.AddHostedService<UpdaterService>();

using var host = builder.Build();

host.Run();
