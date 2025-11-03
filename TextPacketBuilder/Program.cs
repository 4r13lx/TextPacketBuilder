using Application.Services;
using Polly;
using Polly.Extensions.Http;
using Domain.Services;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

// Configuration: appsettings.json + environment variables
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

services.AddSingleton<IConfiguration>(configuration);

// Logging + dependencias
services.AddLogging(cfg => cfg.AddConsole());
services.AddSingleton<IPacketValidator, PacketValidator>();
services.AddSingleton<TextAssembler>();

// Read packet source config
var packetSourceType = configuration["PacketSource:Type"] ?? "http";
var packetSourceBaseUrl = configuration["PacketSource:BaseUrl"] ?? "http://localhost:8080";

// Configure HttpClient for HttpPacketSource and register implementations
services.AddHttpClient<InfraStructure.Sources.HttpPacketSource>(client =>
{
    client.BaseAddress = new Uri(packetSourceBaseUrl);
})
// Polly retry: retry 3 times with exponential backoff
.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

// Register IPacketSource depending on configuration
if (packetSourceType.Equals("mock", StringComparison.OrdinalIgnoreCase))
{
    services.AddSingleton<Domain.Interfaces.IPacketSource, InfraStructure.Sources.MockPacketSource>();
}
else
{
    services.AddTransient<Domain.Interfaces.IPacketSource, InfraStructure.Sources.HttpPacketSource>();
}

// Caso de uso principal
services.AddTransient<TextRetrievalService>();

var provider = services.BuildServiceProvider();

var service = provider.GetRequiredService<TextRetrievalService>();

try
{
    var text = await service.GetFullTextAsync(restart: !packetSourceType.Equals("mock", StringComparison.OrdinalIgnoreCase));
    Console.WriteLine("\n📘 Texto reconstruido:");
    Console.WriteLine(text);
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
}
